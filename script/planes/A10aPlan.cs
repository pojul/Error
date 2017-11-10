using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A10aPlan : MonoBehaviour {

	public static float planeHeight = 26.2f;
	public static float cameraX = 0.33f;
	public static float cameraY = 3.07f;
	public static float cameraZ = 44.08f;
	
	public static float air_ray1X = 13.8f;
	public static float air_ray1Y = -1.06348f;
    public static float air_ray1Z = -44.96f;
	
	public static float air_ray2X = -13.33f;
	public static float air_ray2Y = -1.06348f;
    public static float air_ray2Z = -44.96f;
	
	public GameObject mainCamera;
	public GameObject airRay1;
	public GameObject airRay2;
	public GameObject target;
	
	public int playerType = -1; //0: player; 1:  prefab

	// Use this for initialization
	void Start () {
		addAirRay ();

		//for test optimition
		//setPlayType (0);
	}
	
	public void setPlayType(int playerType){
		this.playerType = playerType;
		if(playerType == 0){
			mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
			//transform.position = new Vector3 (0, 50, 0);-10161.35
			//transform.position = new Vector3 (0, 50, -10161.35f);
			//transform.position = new Vector3 (373, 50, 9870);
			mainCamera.transform.position = new Vector3((transform.position.x + cameraX), 
				(transform.position.y + cameraY), 
				(transform.position.z+ cameraZ));
			mainCamera.transform.parent = transform;
		}
	}
	
	void OnBecameVisible(){
		//show
	}
	
	void OnBecameInvisible(){
		
	}

	void addAirRay (){
		//airRay1 = (GameObject)Instantiate(Resources.Load((string)GameInit.modelpaths["air_ray_A10"]), 
		airRay1 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/air_ray_A101"), 
		
			new Vector3(transform.position.x + air_ray1X, 
						transform.position.y + air_ray1Y, 
						transform.position.z+ air_ray1Z), 
					Quaternion.Euler(0, 180, 0)) as GameObject;
		airRay1.transform.parent = transform;

		//airRay2 = (GameObject)Instantiate(Resources.Load((string)GameInit.modelpaths["air_ray_A10"]), 
		airRay2 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/air_ray_A101"), 
			new Vector3(transform.position.x + air_ray2X, 
				transform.position.y + air_ray2Y, 
				transform.position.z+ air_ray2Z), 
			Quaternion.Euler(0, 180, 0)) as GameObject;
		airRay2.transform.parent = transform;
	}

}
