using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirage5 : MonoBehaviour {

    public static float planeHeight = 29.1f;
	public static float cameraX = 0.0f;
	public static float cameraY = -5.1f;
	public static float cameraZ = 34f;

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
