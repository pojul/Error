using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destory : PojulObject {

	float destoryTime = 0.0f;

	// Use this for initialization
	void Start () {

		if (CompareTag ("tankFire")) {
			destoryTime = 2;
		} else if (CompareTag ("bomb1")) {
			destoryTime = 22;
		} else if (CompareTag ("bomb2")) {
			destoryTime = 20;
		} else if (CompareTag ("bulletFire")) {
			destoryTime = 1;
		}else if (CompareTag ("bulletBomb1")) {
			destoryTime = 1.2f;
		}else if (CompareTag ("bulletBomb2")) {
			destoryTime = 1.0f;
		}

		Invoke ("destory", destoryTime);
	}

	void destory(){
		Destroy (this.gameObject);
	}
}
