using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planMove : MonoBehaviour {
	
	public static float speed = 1000;
	public static Transform player;
	public static int currentMountMissle = 0;

	public static List<Transform> nearEnemy = new List<Transform>();

	//public float velocity = 0.0f;
	public static float maxAccelerate = 1.5f;
	public static float accelerate = 0.0f;
	public static float maxSpeed = 1100;
	private float piRate = 2* Mathf.PI/360;

	public static float dzMainCamera = 220;
	public static float dyMainCamera = 50;

	public static Rigidbody mRigidbody;
	public bool isCollisioned = false;

	public string playerId = "";
	public string type = "";

	public static float rolSpeed = 7.0f;
	private PojulObject mPojulObject;

	public static float dRotate3 = 0.0f;
	public static float rotate2 = 0.0f;
	
	void Start(){
		player = transform;
		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];

		mPojulObject = transform.gameObject.GetComponent<PojulObject> ();

	}

	void Update(){

		if(mPojulObject == null || mPojulObject.isDestoryed){
			return;
		}

		if(PlanControls.newPoint1Rolation != -1 || PlanControls.newPoint3Rolation != -1){
			if(speed >= 0 && speed <= maxSpeed){
				speed = speed + accelerate;
			}
			if (speed < 0) {
				speed = 0;
			}
			if (speed > maxSpeed) {
				speed = maxSpeed;
			}

			if("a10".Equals(type)){
				Quaternion targetRotation = Quaternion.Euler(-(PlanControls.newPoint3Rolation + 360) , 
					PlanControls.newPoint1Rolation,
					-(PlanControls.newPoint2Rolation + 360));
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rolSpeed);

				Camera.main.transform.position = transform.position +  (-transform.forward * dzMainCamera + transform.up * dyMainCamera);
				Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, 
					Quaternion.LookRotation(transform.position - Camera.main.transform.position), 3.5f * Time.deltaTime);
			}else if("car3".Equals(type)){
				float rolX = transform.rotation.eulerAngles.x;
				if(transform.position.y >= 0){
					rolX = 0;
				}else if(transform.position.y < 0){
					rolX = 90;
					rolSpeed = 0.012f;
				}

				Quaternion targetRotation = Quaternion.Euler(rolX, 
					PlanControls.newPoint1Rolation,
					0);
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rolSpeed);

				((CarType3)mPojulObject).rotatePan (dRotate3);
				dRotate3 = 0;
				//((CarType3)mPojulObject).rotatePao (rotate2);

				((CarType3)mPojulObject).rotateCamera ();

				if(transform.position.y < -1800){
					mPojulObject.isDestoryed = true;
					Destroy (this.gameObject);
				}

			}

			if(mRigidbody != null && transform.position.y >= 0){
				mRigidbody.AddForce (transform.forward * speed * 4);
				//Debug.Log ("gqb------>" + mRigidbody.velocity.magnitude);
			}
				

		}
		 
	}

	void updateHori(){
		/*float dy = speed * Mathf.Sin(piRate*PlanControls.newPoint3Rolation);
		float dxz = speed * Mathf.Cos(piRate*PlanControls.newPoint3Rolation);
		float dx = dxz * Mathf.Sin(piRate*PlanControls.newPoint1Rolation);
		float dz = dxz * Mathf.Cos(piRate*PlanControls.newPoint1Rolation);
		
		transform.position = new Vector3(transform.position.x + dx*Time.deltaTime, 
						transform.position.y + dy*Time.deltaTime,
						transform.position.z + dz*Time.deltaTime);*/
	}

	void OnCollisionEnter(Collision collision){
		if (mRigidbody != null) {
			//mRigidbody.drag = 1;
			//mRigidbody.angularDrag = 1;
		}
	}

	void OnCollisionExit(Collision collision){
		if(mRigidbody != null){
			//mRigidbody.drag = 10;
			//mRigidbody.angularDrag = 10;
			//Debug.Log ("gqb------>" + mRigidbody.velocity.magnitude);
			//mRigidbody.velocity = mRigidbody.velocity;
		}
	}

}