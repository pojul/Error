using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeAngle : MonoBehaviour {

	public GameObject targetObj;

	private Transform target;

	private Quaternion angle;
	private Quaternion angle1;

	private GameObject tempObj;
	//private Quaternion angle2;

	// Use this for initialization
	void Start () {
		tempObj = GameObject.CreatePrimitive (PrimitiveType.Cube);
		Destroy(tempObj.GetComponent<BoxCollider> ());
		tempObj.GetComponent<Renderer> ().enabled = false;

		target = targetObj.transform;

		InvokeRepeating ("relativeAngle", 1,1);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void relativeAngle(){
		if(target != null){
			angle = Quaternion.LookRotation (target.position - transform.position);
			angle1 = Quaternion.LookRotation (transform.forward);
			//angle2 = Quaternion.LookRotation (transform.forward);

			float dy1 = Mathf.Abs (angle1.eulerAngles.y - angle.eulerAngles.y);
			float dy2 = 360 - dy1;
			//Debug.Log ("gqb------>d angle y: " + Mathf.Min(dy1, dy2));




			float dx1 = Mathf.Abs (angle1.eulerAngles.x - angle.eulerAngles.x);
			float dx2 = 360 - dx1;
			Debug.Log ("gqb------>d angle x: " + Mathf.Min(dx1, dx2) + "; d angle y: " + Mathf.Min(dy1, dy2));

			//Debug.Log ("gqb------>angle1 x: " + angle1.eulerAngles.x + "; y: " + angle1.eulerAngles.y);
			//Debug.Log ("gqb------>angle x: " + angle.eulerAngles.x + "; y: " + angle.eulerAngles.y);
			//Debug.Log ("gqb------>dx: " + (angle1.eulerAngles.x - angle.eulerAngles.x) + "; dy: " + (angle1.eulerAngles.y - angle.eulerAngles.y) );
		}
	}
}
