using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEmeny : MonoBehaviour {

	private GameObject prefab;
	private GameObject nearObj;

	// Use this for initialization
	void Start () {
		Invoke ("createEnemy", 150);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void createEnemy(){
		nearObj = GameInit.gameObjectInstance [0];
		prefab = (GameObject)Instantiate(Resources.Load((string)GameInit.modelpaths["car3"]), 
			new Vector3(0, 200, 60000), Quaternion.Euler(0, 0, 0)) as GameObject;
		prefab.tag = "1_car3";
		PojulObject p = prefab.GetComponent<PojulObject> ();
		p.behavior = 3;

		//Debug.Log ("GQB------>createObj");

		//Invoke ("changePosition", 2);

		//gameObjectInstance.Add (prefab);
	}

	void changePosition(){
		/*if(prefab != null && nearObj != null){
			
		}*/
		//prefab.transform.position = new Vector3 ( (nearObj.transform.position.x + 100), 
			//nearObj.transform.position.y,
			//(nearObj.transform.position.z + 100) );
		//Debug.Log ("GQB------>createObj");
	}

}
