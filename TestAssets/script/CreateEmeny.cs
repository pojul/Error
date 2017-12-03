using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEmeny : MonoBehaviour {

	private GameObject prefab;
	//private GameObject nearObj;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("createEnemy", 120, 50);
		//Invoke ("createEnemy", 120);
		//Invoke ("createEnemy", 160);
		//Invoke ("createEnemy", 170);
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

		//Invoke ("changePosition", 2);

		//gameObjectInstance.Add (prefab);
	}

}
