using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInit : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//GetComponent<Terrain> ().terrainData.size = new Vector3(1 , GetComponent<Terrain> ().terrainData.size.y, 1);
		transform.localScale = new Vector3(2500, 1, 2500);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
