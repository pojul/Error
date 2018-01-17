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
		if(target != null && !isDestoryed){
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
					speed = speed - 2.8f - angle *3.2f;
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
		this.target = MissileType1.getMissileTarget(transform, target);
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
		if(isDestoryed){
			return;
		}
		isDestoryed = true;
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
		if(isDestoryed){
			return;
		}
		//Debug.Log ("gqb------>OnCollisionEnter MissileType3: ");
		ContactPoint contact = collision.contacts[0];
		GameObject bomb2 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bomb2"), 
			contact.point, Quaternion.FromToRotation(Vector3.up, contact.normal)) as GameObject;
		bomb2.tag = "bomb2";
		bomb2.transform.parent = collision.gameObject.transform;

		Transform root = null;
		PojulObject mPojulObject = null;
		if(collision.gameObject.transform.root.childCount > 0){
			root = collision.gameObject.transform.root.GetChild(0);
			mPojulObject = root.GetComponent<PojulObject> ();
		}

		if(mPojulObject == null && collision.gameObject.transform != null){
			root = collision.gameObject.transform.root;
			mPojulObject = root.gameObject.GetComponent<PojulObject> ();
		}
		if(mPojulObject != null){
			if(mPojulObject.type.Equals("transport1")){
				((TransportType1)mPojulObject).setBombParent (bomb2);
			}
			mPojulObject.isFired(new RaycastHit(), collision, 3);
		}
		isDestoryed = true;
		destory ();
	}

	public override void isFired(RaycastHit hit, Collision collision, int type){
		if(isDestoryed){
			return;
		}

		if (type == 2) {
			Vector3 hitPoint;
			if (collision != null) {
				hitPoint = collision.contacts[0].point;
			} else {
				hitPoint = hit.point;
			}
			isDestoryed = true;
			DestoryAll (hitPoint, 30000.0f);
			return;
		}
	}

	void DestoryAll(Vector3 point, float force){
		if(!transform.gameObject.GetComponent<Rigidbody>()){
			transform.gameObject.AddComponent<Rigidbody>();
		}
		Rigidbody mRigidbody = transform.gameObject.GetComponent<Rigidbody>();
		mRigidbody.useGravity = true;
		Vector3 explosionPos = new Vector3 (point.x, 
			(point.y - 10), 
			point.z);
		Collider[] colliders = Physics.OverlapSphere(explosionPos, 200.0f);
		foreach (Collider hit in colliders){
			if (hit.GetComponent<Rigidbody>() && hit.transform != Camera.main.transform){
				hit.GetComponent<Rigidbody>().AddExplosionForce(force, explosionPos, 200.0f);
			}  
		}
		Invoke ("destory", 10);
	}
		
	void destory(){
		lock(GameInit.locker){
			if(!GameInit.currentInstance.ContainsKey((string)tag)){
				return;
			}
			if(GameInit.currentInstance.ContainsKey((string)tag)){
				GameInit.currentInstance[tag] = GameInit.currentInstance[tag] - 1;
				//Debug.Log ("gqb----->destory: " + tag);
			}
			if(GameInit.gameObjectInstance.Contains(transform.gameObject)){
				GameInit.gameObjectInstance.Remove (transform.gameObject);
			}
			Destroy (this.gameObject);
		}
	}

}
