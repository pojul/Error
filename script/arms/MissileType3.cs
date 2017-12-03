using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileType3 : PojulObject {

	private AudioSource mAudioSource;

	public Transform target;
	private GameObject blaze;
	private Transform blazePos;

	private bool isForward = false;
	private float maxSpeed = GameInit.mach * 2.5f;
	private float acceleration = 20;
	private float speed = 0;
	private float aimSpeed = 20;

	// Use this for initialization
	void Start () {
		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		blazePos = transform.FindChild ("blazePos");
	}
	
	// Update is called once per frame
	void Update () {
		if(target != null){
			if (!isForward) {
				transform.rotation = Quaternion.Slerp(transform.rotation, 
					Quaternion.LookRotation(target.position - transform.position), aimSpeed * Time.deltaTime);
			}
			if(transform.parent != null){
				transform.parent = null;
			}
			transform.position = transform.position + transform.forward * speed * Time.deltaTime;
			if(speed < maxSpeed){
				speed = speed + acceleration;
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
		transform.gameObject.AddComponent<Rigidbody> ();
		transform.gameObject.GetComponent<Rigidbody> ().useGravity = false;
	}

	void OnCollisionEnter(Collision collision){
		Debug.Log ("gqb------>OnCollisionEnter: ");
		ContactPoint contact = collision.contacts[0];
		GameObject bomb2 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bomb2"), 
			contact.point, Quaternion.FromToRotation(Vector3.up, contact.normal)) as GameObject;
		bomb2.tag = "bomb2";
		bomb2.transform.parent = collision.gameObject.transform;
		Transform root = collision.gameObject.transform.root.GetChild(0);
		if(root.GetComponent<PojulObject> ()){
			((PojulObject)root.GetComponent<PojulObject> ()).isFired(collision, 3);
		}

		destory ();
	}

	void destory(){
		Destroy (this.gameObject);
	}


}
