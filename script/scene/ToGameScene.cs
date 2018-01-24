using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToGameScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke ("toGame", 5);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void toGame (){
		SceneManager.LoadScene ("T4");
	}
}
