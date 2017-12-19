using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileType3 : PojulObject {

	private AudioSource mAudioSource;

	public Transform target;
	private GameObject blaze;
	private Transform blazePos;

	private bool isForward = false;
	private bool isDecay = false;

	private float maxSpeed = GameInit.mach * 3.0f;
	private float acceleration = 25;
	private float speed = 0;
	private float aimSpeed = 4.3f;

	private Rigidbody mRigidbody;
	private BoxCollider mBoxCollider;

	// Use this for initialization
	void Start () {
		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();
		mBoxCollider = transform.gameObject.GetComponent<BoxCollider> ();
		mBoxCollider.enabled = false;

		blazePos = transform.FindChild ("blazePos");
	}
	
	// Update is called once per frame
	void Update () {
		if(target != null){
			Quaternion rawRotation = transform.rotation;
			Quaternion newRotation = new Quaternion();
			if (!isForward && !missTarget) {
				transform.rotation = Quaternion.Slerp(transform.rotation, 
					Quaternion.LookRotation(target.position - transform.position), aimSpeed * Time.deltaTime);
				if(blaze != null){
					blaze.transform.localEulerAngles = new Vector3 (0,0,0);
				}
				newRotation = transform.rotation;
			}
			if(transform.parent != null){
				transform.parent = null;
			}

			transform.position = transform.position + transform.forward * speed * Time.deltaTime;
			if(!missTarget){
				if(speed < maxSpeed && !isDecay){
					speed = speed + acceleration;
				}
			}

			if(isDecay){
				GameObject g = new GameObject ();
				g.transform.rotation = rawRotation;
				Vector3 rawForward = g.transform.forward;
				g.transform.rotation = newRotation;
				Vector3 newForward = g.transform.forward;
				float angle = Mathf.Acos (Vector3.Dot (rawForward.normalized, newForward.normalized)) * Mathf.Rad2Deg;
				//Debug.Log (speed + "gqb------>angle: " + angle);
				if(angle < 90 && speed > 400){
					speed = speed - 2.2f - angle *3.0f;
				}

				if(speed <= 400){
					missTarget = true;
					transform.rotation = Quaternion.Slerp(transform.rotation, 
						Quaternion.Euler(90, 
							transform.rotation.eulerAngles.y,
							transform.rotation.eulerAngles.z), 0.02f * Time.deltaTime);
				}

				/*if(missTarget && !mRigidbody.useGravity){
					mRigidbody.useGravity = true;
				}*/

				Destroy (g);
			}
		}
	}

	public void fireInit(string tag, Transform target, float startSpeed){
		transform.tag = tag;
		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];
		this.target = target;
		this.speed = startSpeed;

		initBlaze ();

		Invoke ("startDecay", 5);
		Invoke ("destoryMissile", 35);

	}

	void startDecay(){
		isDecay = true;
		//Destroy (blaze);
	}

	void destoryMissile(){
		destory ();
	}

	void initBlaze(){
		blaze = (GameObject)Instantiate(Resources.Load((string)GameInit.modelpaths["missile_blaze1"]), 
			blazePos.position, Quaternion.Euler(0, 0, 0)) as GameObject;
		blaze.transform.parent = transform;
		Invoke ("addRigidbody", 0.8f);
		if(mAudioSource != null && !mAudioSource.isPlaying){
			mAudioSource.Play ();
		}

	}

	void addRigidbody(){
		mBoxCollider.enabled = true;

		transform.gameObject.AddComponent<Rigidbody> ();
		transform.gameObject.GetComponent<Rigidbody> ().useGravity = false;
		mRigidbody = transform.GetComponent<Rigidbody> ();
	}

	void OnCollisionEnter(Collision collision){
		//Debug.Log ("gqb------>OnCollisionEnter: ");
		ContactPoint contact = collision.contacts[0];
		GameObject bomb2 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bomb2"), 
			contact.point, Quaternion.FromToRotation(Vector3.up, contact.normal)) as GameObject;
		bomb2.tag = "bomb2";
		bomb2.transform.parent = collision.gameObject.transform;
		Transform root = collision.gameObject.transform.root.GetChild(0);
		PojulObject mPojulObject = root.GetComponent<PojulObject> ();

		if(mPojulObject == null && collision.gameObject.transform != null){
			root = collision.gameObject.transform;
			mPojulObject = root.gameObject.GetComponent<PojulObject> ();
		}
		if(mPojulObject != null){
			mPojulObject.isFired(collision, 3);
		}
		destory ();
	}

	void destory(){
		lock(GameInit.locker){
			if(!GameInit.currentInstance.ContainsKey((string)tag)){
				return;
			}
			if(GameInit.currentInstance.ContainsKey((string)tag)){
				GameInit.currentInstance[tag] = GameInit.currentInstance[tag] - 1;
			}
			if(GameInit.gameObjectInstance.Contains(transform.gameObject)){
				GameInit.gameObjectInstance.Remove (transform.gameObject);
			}
			Destroy (this.gameObject);
		}
	}

}
