using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusArea{

	public int areaId;

	public float maxRange = 50000f;
	public float minRange = 15000f;

	public RadiusArea(int areaId){
		this.areaId = areaId;
	}

	public Vector3 getRandomPoint(){
		float mRandomAngle = Random.Range (0.0f, Mathf.PI/2);
		float mRandomRadius = Random.Range (minRange, maxRange);
		float x = 0.0f;
		float z = 0.0f;
		if(areaId == 1){
			x = -mRandomRadius * Mathf.Cos (mRandomAngle);
			z = -60000 + mRandomRadius * Mathf.Sin (mRandomAngle);
		}else if(areaId == 2){
			x = mRandomRadius * Mathf.Cos (mRandomAngle);
			z = -60000 + mRandomRadius * Mathf.Sin (mRandomAngle);
		}else if(areaId == 3){
			x = mRandomRadius * Mathf.Cos (mRandomAngle);
			z = -60000 - mRandomRadius * Mathf.Sin (mRandomAngle);
		}else if(areaId == 4){
			x = -mRandomRadius * Mathf.Cos (mRandomAngle);
			z = -60000 - mRandomRadius * Mathf.Sin (mRandomAngle);
		}else if(areaId == 5){
			x = mRandomRadius * Mathf.Cos (mRandomAngle);
			z = 60000 - mRandomRadius * Mathf.Sin (mRandomAngle);
		}else if(areaId == 6){
			x = -mRandomRadius * Mathf.Cos (mRandomAngle);
			z = 60000 - mRandomRadius * Mathf.Sin (mRandomAngle);
		}else if(areaId == 7){
			x = mRandomRadius * Mathf.Cos (mRandomAngle);
			z = 60000 + mRandomRadius * Mathf.Sin (mRandomAngle);
		}else if(areaId == 8){
			x = -mRandomRadius * Mathf.Cos (mRandomAngle);
			z = 60000 + mRandomRadius * Mathf.Sin (mRandomAngle);
		}
		return new Vector3 (x,100,z);
	}

}
