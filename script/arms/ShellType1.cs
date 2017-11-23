﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellType1 : PojulObject {

	public float startSpeed;
	public float decaySpeed;
	public bool isShoot = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(isShoot){
			transform.GetComponent<Rigidbody> ().AddForce (transform.up*60.0f);
			transform.position = transform.position + transform.forward * startSpeed * Time.deltaTime;
			startSpeed = startSpeed - decaySpeed * Time.deltaTime;
		}
	}

	public void shoot(float startSpeed, float decaySpeed){
		this.startSpeed = startSpeed;
		this.decaySpeed = decaySpeed;
		this.isShoot = true;
	}

	void OnCollisionEnter(Collision collision){
		ContactPoint contact = collision.contacts[0];
		GameObject bomb2 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bomb2"), 
			contact.point, Quaternion.FromToRotation(Vector3.up, contact.normal)) as GameObject;
		bomb2.tag = "bomb2";
		bomb2.transform.parent = collision.gameObject.transform;
		((PojulObject)collision.gameObject.transform.root.GetComponent<PojulObject> ()).isFired(collision, 2);
		destory ();
	}

	void destory(){
		Destroy (this.gameObject);
	}

}