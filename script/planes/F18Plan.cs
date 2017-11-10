using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F18Plan : MonoBehaviour {

	public static float planeHeight = 20.3f;
	public static float cameraX = -0.385f;
	public static float cameraY = 6.04f;
	public static float cameraZ = 30.37f;

	private bool isPlayer = true;

	// Use this for initialization
	void Start () {
		if(isPlayer){
			transform.position = new Vector3 (55000, 50, 53000);
			MainCamera.camera.transform.position = new Vector3((transform.position.x + cameraX), 
				(transform.position.y + cameraY), 
				(transform.position.z+ cameraZ));
			MainCamera.camera.transform.parent = transform;
		}
	}
}
