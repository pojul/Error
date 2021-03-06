﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBullet : MonoBehaviour {

	private Transform[] fires = new Transform[8];
	int fireIndex = 0;

	// Use this for initialization
	void Start () {
		
		fires[0] = transform.Find ("fire1");
		fires[1] = transform.Find ("fire2");
		fires[2] = transform.Find ("fire3");
		fires[3] = transform.Find ("fire4");
		fires[4] = transform.Find ("fire5");
		fires[5] = transform.Find ("fire6");
		fires[6] = transform.Find ("fire7");
		fires[7] = transform.Find ("fire8");

		InvokeRepeating ("fire", 0.12f, 0.12f);
		//InvokeRepeating ("fire", 6f, 6f);
	}

	void fire(){
		if(fireIndex > 7){
			fireIndex = 0;
		}
		if(fires[fireIndex] == null){
			return;
		}
		GameObject fireEffect = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bulletFire"), 
			fires[fireIndex].position , fires[fireIndex].rotation ) as GameObject;
		fireEffect.tag = "bulletFire";
		fireEffect.transform.parent = transform;

		GameObject bullet = (GameObject)Instantiate(Resources.Load("Prefabs/arms/bullet"),
			fires[fireIndex].position  + fires[fireIndex].forward * 0.5f, fires[fireIndex].rotation ) as GameObject;
		Bullet mBullet = bullet.GetComponent<Bullet> ();
		mBullet.shoot (0);

		fireIndex = fireIndex + 1;
	}

	// Update is called once per frame
	void Update () {
		if(fireIndex > 7){
			fireIndex = 0;
		}
		Debug.DrawRay(fires[fireIndex].position, fires[fireIndex].forward*12000, Color.white);
	}
}
