using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lookat : MonoBehaviour {

	public GameObject lookat;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Slerp(transform.rotation, 
			Quaternion.LookRotation(lookat.transform.position - transform.position), 10 * Time.deltaTime);
	}
}
