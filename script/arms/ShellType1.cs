using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellType1 : PojulObject {

	public float startSpeed;
	public float decaySpeed;
	public bool isShoot = false;
	private float startTime;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(isShoot){
			transform.GetComponent<Rigidbody> ().AddForce (transform.up*125.0f);
			transform.position = transform.position + transform.forward * startSpeed * Time.deltaTime;
			startSpeed = startSpeed - decaySpeed * Time.deltaTime;
		}
		if((Time.time - startTime) > 6){
			destory ();
		}
	}

	public void shoot(float startSpeed, float decaySpeed){
		this.startSpeed = startSpeed;
		this.decaySpeed = decaySpeed;
		this.isShoot = true;
	}

	void OnCollisionEnter(Collision collision){
		ContactPoint contact = collision.contacts[0];
		//GameObject bomb2 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bomb2"), 
			//(contact.point - transform.forward * 10), Quaternion.FromToRotation(Vector3.up, contact.normal)) as GameObject;
		GameObject bomb2 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bomb2"), 
			(transform.position - transform.forward * 10), Quaternion.FromToRotation(Vector3.up, contact.normal)) as GameObject;
		bomb2.tag = "bomb2";
		bomb2.transform.parent = collision.gameObject.transform;
		Transform root = collision.gameObject.transform.root.GetChild(0);
		PojulObject mPojulObject = root.GetComponent<PojulObject> ();

		if(mPojulObject == null && collision.gameObject.transform != null){
			root = collision.gameObject.transform;
			mPojulObject = root.gameObject.GetComponent<PojulObject> ();
		}

		if(mPojulObject != null){
			mPojulObject.isFired(new RaycastHit(), collision, 2);
		}

		destory ();
	}

	void destory(){
		Destroy (this.gameObject);
	}

}
