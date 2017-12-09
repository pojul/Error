using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEmeny : MonoBehaviour {

	private GameObject prefab;
	//private GameObject nearObj;

	// Use this for initialization
	void Start () {
		//InvokeRepeating ("createEnemy", 80, 50);
		//Invoke ("createEnemy", 120);
		//Invoke ("createEnemy", 160);
		//Invoke ("createEnemy", 170);
		InvokeRepeating ("createEnemy1", 150, 50);
		//Invoke ("createEnemy1", 155);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void createEnemy(){
		//nearObj = GameInit.gameObjectInstance [0];
		prefab = (GameObject)Instantiate(Resources.Load((string)GameInit.modelpaths["car3"]), 
			new Vector3(0, 200, 60000), Quaternion.Euler(0, 0, 0)) as GameObject;
		prefab.tag = "1_car3";
		PojulObject p = prefab.GetComponent<PojulObject> ();
		p.behavior = 3;

		Debug.Log ("GQB------>createObj");
	}

	void createEnemy1(){
		prefab = (GameObject)Instantiate(Resources.Load((string)GameInit.modelpaths["a10"]), 
			new Vector3(0, 200, 60000), Quaternion.Euler(0, 0, 0)) as GameObject;
		prefab.tag = "1_a10";
		A10aPlan p = prefab.GetComponent<A10aPlan> ();
		p.setAttackPatrolArea (3);
		p.behavior = 3;

		Debug.Log ("GQB------>createObj1");
	}



}
