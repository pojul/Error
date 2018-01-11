using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellType1 : PojulObject {

	public float startSpeed;
	public float decaySpeed;
	public bool isShoot = false;
	private float startTime;
	private bool isHit = false;
	private float hitDistance = 0;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(isShoot){
			if(isHit){
				return;
			}
			RaycastHit hit;
			if (!isHit && Physics.Raycast ((transform.position + transform.forward*5), transform.forward, out hit, hitDistance)) {
				if (!hit.transform.root.CompareTag ("Untagged") ||
				    (hit.transform.root.childCount > 0 && !hit.transform.root.GetChild (0).CompareTag ("Untagged"))) {
					GameObject bomb2 = (GameObject)Instantiate (Resources.Load ("Prefabs/Particle/bomb2"), 
						(transform.position - transform.forward * 10), Quaternion.FromToRotation (Vector3.up, hit.normal)) as GameObject;
					bomb2.tag = "bomb2";
					//Debug.Log ("gqb------>ShellType11111: " + hit.transform.root.tag);
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
						mPojulObject.isFired(hit, null, 2);
						bomb2.transform.parent = root;
					}
				} else {
					//Debug.Log ("gqb------>ShellType22222: " + hit.transform.root.tag);
					GameObject bomb2 = (GameObject)Instantiate (Resources.Load ("Prefabs/Particle/bomb2"), 
						(transform.position - transform.forward * 10), Quaternion.FromToRotation (Vector3.up, hit.normal)) as GameObject;
					bomb2.tag = "bomb2";	
					bomb2.transform.parent = hit.transform.root;
				}
				isHit = true;
				destory ();
			}
			transform.GetComponent<Rigidbody> ().AddForce (transform.up*136.0f);
			transform.position = transform.position + transform.forward * startSpeed * Time.deltaTime;
			startSpeed = startSpeed - decaySpeed * Time.deltaTime;
		}
		if((Time.time - startTime) > 6){
			isHit = true;
			destory ();
		}
	}

	public void shoot(float startSpeed, float decaySpeed){
		this.startSpeed = startSpeed;
		this.decaySpeed = decaySpeed;
		this.isShoot = true;
		hitDistance = startSpeed * 1.0f / 20;
		isHit = false;
	}

	void OnCollisionEnter(Collision collision){
		ContactPoint contact = collision.contacts[0];
		//GameObject bomb2 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bomb2"), 
			//(contact.point - transform.forward * 10), Quaternion.FromToRotation(Vector3.up, contact.normal)) as GameObject;
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
			mPojulObject.isFired(new RaycastHit(), collision, 2);
		}

		isHit = true;
		destory ();
	}

	void destory(){
		Destroy (this.gameObject);
	}

}
