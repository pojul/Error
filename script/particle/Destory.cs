using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destory : PojulObject {

	float destoryTime = 0.0f;

	// Use this for initialization
	void Start () {

		if(CompareTag("tankFire")){
			destoryTime = 2;
		}else if(CompareTag("bomb1")){
			destoryTime = 22;
		}else if(CompareTag("bomb2")){
			destoryTime = 20;
		}

		Invoke ("destory", destoryTime);
	}

	void destory(){
		Destroy (this.gameObject);
	}
}
