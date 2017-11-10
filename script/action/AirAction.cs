using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirAction : MonoBehaviour {

	public bool moniNavStopped = false;
	public UnityEngine.AI.NavMeshAgent nav;
	public A10aPlan a10aPlan;
	public GameObject navCube;

	// Use this for initialization
	void Start () {
		a10aPlan = this.gameObject.GetComponent<A10aPlan> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(nav == null){
			return;
		}
		//Debug.Log (nav.remainingDistance + "---" + nav.hasPath);
		if(nav != null && moniNavStopped && nav.hasPath && Mathf.Abs(nav.remainingDistance) < 100){
			moniNavStopped = false;
			onNavFinished ();
		}
	}

	public void startNav(Vector3 navPoint){
		if(navCube == null){
			createNavCube ();
		}
		nav.destination = navPoint;//target.transform.position;
		nav.speed = 2000;
		nav.acceleration = 2000;
		//nav.baseOffset = 50;
		//nav.angularSpeed = 100;
		moniNavStopped = true;
	}

	void onNavFinished (){
		Destroy (navCube);
		Destroy (this.gameObject);
	}

	public void createNavCube(){
		navCube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		navCube.transform.position = transform.position;
		navCube.transform.localScale = new Vector3 (100, 100, 100);
		navCube.AddComponent<UnityEngine.AI.NavMeshAgent>();
		transform.parent = navCube.transform;

		MeshRenderer m = navCube.GetComponent<MeshRenderer>();
		m.enabled = false;

		nav= navCube.GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

}
