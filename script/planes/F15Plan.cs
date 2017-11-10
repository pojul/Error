using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F15Plan : MonoBehaviour {

	public static float planeHeight = 11.2f;
	public static float cameraX = 1.7f;
	public static float cameraY = 10.74f;
	public static float cameraZ = 18.51f;

	private bool isPlayer = false;

	// Use this for initialization
	void Start () {
		if(isPlayer){
			transform.position = new Vector3 (8400, 20, 86000);
			MainCamera.camera.transform.position = new Vector3((transform.position.x + cameraX), 
				(transform.position.y + cameraY), 
				(transform.position.z+ cameraZ));
			MainCamera.camera.transform.parent = transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
