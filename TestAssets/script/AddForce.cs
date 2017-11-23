using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForce : PojulObject {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.GetComponent<Rigidbody> ().AddForce (transform.up*160.2f);
	}
}
