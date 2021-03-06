﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileType1 : PojulObject {

	private AudioSource mAudioSource;

	public Transform target;
	private GameObject blaze;
	private Transform blazePos;

	private bool isForward = false;
	private bool isDecay = false;

	private float maxSpeed = 3400 * 3f;
	private float acceleration = 50;
	private float speed = 0;
	private float aimSpeed = 180  *200;

	private Rigidbody mRigidbody;

	private ParticleSystem mParticleSystem;
	private Transform mFire;
	private float moreScale = 0.04f;



	// Use this for initialization
	void Start () {
		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		blazePos = transform.Find ("blazePos");

		//InvokeRepeating ("velocityOverLife", 1f, 1f);

	}

	// Update is called once per frame
	void Update () {
		if(GameInit.gameStatus != 0){
			return;	
		}
		if(isDestoryed){
			transform.position = transform.position + transform.forward * GameInit.mach * Time.deltaTime;
			return;
		}
		if(target != null){
			Quaternion rawRotation = transform.rotation;
			Quaternion newRotation = new Quaternion();
			if (!isForward && !missTarget) {
				//transform.rotation = Quaternion.Slerp(transform.rotation, 
					//Quaternion.LookRotation(target.position - transform.position), aimSpeed * Time.deltaTime);

				Quaternion fromRol = transform.rotation;
				Quaternion toRol = Quaternion.LookRotation(target.position - transform.position);
				float dRolX = Util.getDirectDRol(fromRol.eulerAngles.x, toRol.eulerAngles.x, aimSpeed * Time.deltaTime, moreScale);
				float dRolY = Util.getDirectDRol(fromRol.eulerAngles.y, toRol.eulerAngles.y, aimSpeed * Time.deltaTime, moreScale);
				float dRolZ = Util.getDirectDRol(fromRol.eulerAngles.z, toRol.eulerAngles.z, aimSpeed * Time.deltaTime, moreScale);
				transform.rotation = Quaternion.Euler (new Vector3((transform.eulerAngles.x + dRolX), 
					(transform.eulerAngles.y + dRolY), 
					(transform.eulerAngles.z + dRolZ))
				);

				newRotation = transform.rotation;
			}
			if(transform.parent != null){
				transform.parent = null;
			}

			if(blaze != null){
				blaze.transform.localEulerAngles = new Vector3 (0,0,0);
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
					speed = speed - 20 - angle *30f;
				}
				if(speed <= 400){
					missTarget = true;
					transform.rotation = Quaternion.Slerp(transform.rotation, 
						Quaternion.Euler(90, 
							transform.rotation.eulerAngles.y,
							transform.rotation.eulerAngles.z), 0.02f * Time.deltaTime);
				}
				Destroy (g);
			}
		}
	}

	public void fireInit(string tag, Transform target, float startSpeed){
		transform.tag = tag;
		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];
		this.target = getMissileTarget(transform, target);
		this.speed = startSpeed + 200;
		isForward = true;


		initBlaze ();

		Invoke ("startDecay", 10);
		Invoke ("destoryMissile", 28);

	}

	void startDecay(){
		isDecay = true;
		//Destroy (blaze);
	}

	void destoryMissile(){
		destory ();
	}


	void initBlaze(){
		blaze = (GameObject)Instantiate(Resources.Load((string)GameInit.modelpaths["missile_blaze2"]), 
			blazePos.position, blazePos.rotation) as GameObject;
		blaze.transform.parent = transform;
		Invoke ("addRigidbody", 0.8f);
		if(mAudioSource != null && !mAudioSource.isPlaying){
			mAudioSource.Play ();
		}
		mParticleSystem = blaze.transform.Find ("smoke").gameObject.GetComponent<ParticleSystem>();
		mFire = blaze.transform.Find ("fire");
	}

	void addRigidbody(){
		isForward = false;
		transform.gameObject.AddComponent<Rigidbody> ();
		transform.gameObject.GetComponent<Rigidbody> ().useGravity = false;
		mRigidbody = transform.gameObject.GetComponent<Rigidbody> ();
	}

	void OnCollisionEnter(Collision collision){
		//Debug.Log ("gqb------>OnCollisionEnter: ");
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
			mPojulObject.isFired(new RaycastHit() ,collision, 3);
		}

		Destroy(transform.gameObject.GetComponent<Rigidbody> ());
		Destroy(transform.gameObject.GetComponent<BoxCollider> ());
		if(transform.gameObject.GetComponent<MeshRenderer> ()){
			transform.gameObject.GetComponent<MeshRenderer> ().enabled = false;
		}

		isDestoryed = true;
		if(mFire != null){
			Destroy (mFire.gameObject);
		}
		if(mParticleSystem != null){
			System.Type mType = mParticleSystem.main.GetType ();
			System.Reflection.PropertyInfo property = mType.GetProperty("loop");
			//Debug.Log ("gqb------>property1: " + property.GetValue(mParticleSystem.main, null));
			property.SetValue (mParticleSystem.main, false, null);
			//Debug.Log ("gqb------>property1: " + property.GetValue(mParticleSystem.main, null));
		}
		Invoke ("destory", 15f);
		//destory ();
	}

	void destory(){
		if(!GameInit.currentInstance.ContainsKey((string)tag)){
			return;
		}
		if(GameInit.currentInstance.ContainsKey((string)tag)){
			GameInit.currentInstance[tag] = (int)GameInit.currentInstance[tag] - 1;
		}
		if(GameInit.gameObjectInstance.Contains(transform.gameObject)){
			GameInit.gameObjectInstance.Remove (transform.gameObject);
		}
		Destroy (this.gameObject);

	}

	public static Transform getMissileTarget(Transform self, Transform rawTarget){
		if(rawTarget.parent != null && (rawTarget.parent.CompareTag("0_transport1") || rawTarget.parent.CompareTag("1_transport1"))){
			Transform pos1 = rawTarget.parent.Find ("aimpos1");
			Transform pos2 = rawTarget.parent.Find ("aimpos1");
			if (pos1 != null || pos2 != null) {
				float ds1 = 100000;
				float ds2 = 100000;
				if(pos1 != null){
					ds1 = (self.position - pos1.position).magnitude;
				}
				if(pos2 != null){
					ds2 = (self.position - pos2.position).magnitude;
				}
				if(ds1 < ds2 && pos1 != null){
					rawTarget = pos1;
				}else if(pos2 != null){
					rawTarget = pos2;
				}
			}
		}
		return rawTarget;
	}


}
