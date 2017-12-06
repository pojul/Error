using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planMove : MonoBehaviour {
	
	public static float speed = 900f;
	private float piRate = 2* Mathf.PI/360;
	
	void Start(){
		//transform.position = new Vector3 (8400, 20, 86000);
	}
	
	void Update(){
		
		if(PlanControls.newPoint1Rolation != -1 || PlanControls.newPoint3Rolation != -1){
			updateHori();
			Quaternion targetRotation = Quaternion.Euler(-(PlanControls.newPoint3Rolation + 360) , 
											  PlanControls.newPoint1Rolation,
											  -(PlanControls.newPoint2Rolation + 360));
			/*Quaternion targetRotation = Quaternion.Euler(PlanControls.newPoint3Rolation, 
											  PlanControls.newPoint1Rolation,
											  -(PlanControls.newPoint2Rolation + 360));*/
											  
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8);
			/*//float dRolz = (PlanControls.newPoint2Rolation - transform.eulerAngles.z) * Time.deltaTime * 10;
			//Debug.Log("gqb------>dRolz: " + dRolz + " ;" + (PlanControls.newPoint2Rolation - transform.eulerAngles.z) + " ;z: " + 
						//transform.eulerAngles.z + " ;newPoint2Rolation: " + PlanControls.newPoint2Rolation);
				//+ Input.acceleration.y + " ;z: " + Input.acceleration.z + " ;newPoint2Rolation: " + newPoint2Rolation + " ;dRol: " + dRol);
			//if(transform.rotation.y != PlanControls.newPoint1Rolation){
				//targetRotation = 
				transform.rotation=Quaternion.Euler(new Vector3(-(PlanControls.newPoint3Rolation + 360),
						PlanControls.newPoint1Rolation,
						-(PlanControls.newPoint2Rolation + 360)));
			//}*/
		}
		 
	}
	
	void updateHori(){
		float dy = speed * Mathf.Sin(piRate*PlanControls.newPoint3Rolation);
		float dxz = speed * Mathf.Cos(piRate*PlanControls.newPoint3Rolation);
		float dx = dxz * Mathf.Sin(piRate*PlanControls.newPoint1Rolation);
		float dz = dxz * Mathf.Cos(piRate*PlanControls.newPoint1Rolation);
		
		transform.position = new Vector3(transform.position.x + dx*Time.deltaTime, 
						transform.position.y + dy*Time.deltaTime,
						transform.position.z + dz*Time.deltaTime);
	}
	
}