using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PojulObject {

	private float speed = 5600;
	private float hitDistance = 0;
	private bool isHit = false;

	// Use this for initialization
	void Start () {
		Invoke ("destory", 1.2f);
	}
	
	// Update is called once per frame
	void Update () {
		if(isHit){
			return;
		}//collision.contacts[0].point
		RaycastHit hit;
		if(!isHit && Physics.Raycast (transform.position, transform.forward, out hit, hitDistance)){
			//Debug.Log (transform + "gqb------>distance: " + (hit.point - transform.position).magnitude + "; hit: " + hit.transform);
			if (!hit.transform.root.CompareTag ("Untagged") ||
			   (hit.transform.root.childCount > 0 && !hit.transform.root.GetChild (0).CompareTag ("Untagged"))) {
				GameObject bulletBomb2 = (GameObject)Instantiate (Resources.Load ("Prefabs/Particle/bulletBomb2"), 
					                         hit.point, Quaternion.LookRotation (hit.normal)) as GameObject;
				bulletBomb2.tag = "bulletBomb2";

				Transform root = null;
				PojulObject mPojulObject = null;
				if(hit.transform.root.childCount > 0){
					root = hit.transform.root.GetChild(0);
					mPojulObject = root.GetComponent<PojulObject> ();
				}

				if(mPojulObject == null && hit.transform != null){
					root = hit.transform;
					mPojulObject = root.gameObject.GetComponent<PojulObject> ();
				}

				if(mPojulObject != null){
					mPojulObject.isFired(hit, null, 4);
					bulletBomb2.transform.parent = root;
				}

				//bulletBomb2.transform.parent = 
				//Debug.Log ("gqb------>Update2: " + hit.transform.tag);
			} else {
				GameObject bulletBomb1 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bulletBomb1"), 
					hit.point , Quaternion.LookRotation(hit.normal) ) as GameObject;
				bulletBomb1.tag = "bulletBomb1";
				//Debug.Log ("gqb------>Update1: " + hit.transform.tag);
			}
			isHit = true;
			destory ();
		}
		transform.position = transform.position + transform.forward * speed * Time.deltaTime;
	}

	public void shoot(float startSpeed){
		this.speed = this.speed + startSpeed;
		hitDistance = speed * 1.0f / 20;
		isHit = false;
	}

	void destory (){
		Destroy (this.gameObject);
	}

}
