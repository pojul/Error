using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A10Plane : MonoBehaviour {

	public static float planeHeight = 21.8f;
	public static float cameraX = -4.4f;
	public static float cameraY = 10f;
	public static float cameraZ = 53f;

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
}
