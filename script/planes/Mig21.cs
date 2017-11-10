using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mig21 : MonoBehaviour {

	public static float planeHeight = 16.3f;
	public static float cameraX = 2.96f;
	public static float cameraY = 2.03f;
	public static float cameraZ = 29.72f;

	public GameObject mainCamera;
	
	private bool isPlayer = true;
	
	// Use this for initialization
	void Start () {
		if(isPlayer){
			transform.position = new Vector3 (1000, 40, 1000);
			mainCamera.transform.position = new Vector3((transform.position.x + cameraX), 
				(transform.position.y + cameraY), 
				(transform.position.z+ cameraZ));
			mainCamera.transform.parent = transform;
		}
	}
	
}
