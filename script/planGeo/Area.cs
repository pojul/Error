using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area{

	private Vector3 top;
	private float width;
	private float height;

	private float limitBottom;
	private float limitRight;

	public void setArea(Vector3 top, float width, float height){
		this.top = top;
		this.width = width;
		this.height = height;
		this.limitBottom = (this.top.z - height);
		this.limitRight = (this.top.x - width);
	}

	public bool isInsideArea(Vector3 point){
		if((point.z <= top.z) && (point.z >= limitBottom) && (point.x <= top.x) && (point.x >= limitRight)){
			return true;
		}
		return false;
	}

	public Vector3 getRandomPoint(){
		int mRandomZ = Random.Range ((int)limitBottom, (int)top.z);
		int mRandomX = Random.Range ((int)limitRight, (int)top.x);
		Vector3 point = new Vector3 ((float)mRandomX, 0, (float)mRandomZ);
		return point;
	}
}
