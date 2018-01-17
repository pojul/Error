using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planMove : MonoBehaviour {
	
	public static float speed;
	public static Transform player;
	public static int currentMountMissle;

	public static List<Transform> nearEnemy = new List<Transform>();

	//public float velocity = 0.0f;
	public static float maxAccelerate;
	public static float accelerate;
	public static float maxSpeed;
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
	public float minSpeed = 0;
	
	void Start(){
		player = transform;
		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];
		if("car3".Equals(type)){
			minSpeed = 0;
			dzMainCamera = 110;
			dyMainCamera = 18;
		}else if("a10".Equals(type)){
			minSpeed = 888;
			dzMainCamera = 220;
			dyMainCamera = 50;
		}else if("littlecannon1".Equals(type)){
			minSpeed = 0;
			dzMainCamera = 200;
			dyMainCamera = 150;
		}else if("homepao".Equals(type)){
			minSpeed = 0;
			dzMainCamera = 220;
			dyMainCamera = 150;
		}

		mPojulObject = transform.gameObject.GetComponent<PojulObject> ();

	}

	void Update(){

		if(mPojulObject == null || mPojulObject.isDestoryed){
			return;
		}

		if(PlanControls.newPoint1Rolation != -1 || PlanControls.newPoint3Rolation != -1){
			if(speed >= minSpeed && speed <= maxSpeed){
				speed = speed + accelerate;
			}
			if (speed < minSpeed) {
				speed = minSpeed;
			}
			if (speed > maxSpeed) {
				speed = maxSpeed;
			}

			if ("a10".Equals (type)) {
				Quaternion targetRotation = Quaternion.Euler (-(PlanControls.newPoint3Rolation + 360), 
					                            PlanControls.newPoint1Rolation,
					                            -(PlanControls.newPoint2Rolation + 360));
				transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * rolSpeed);

				if (!UImanager.isCamreaMoveTo) {
					Camera.main.transform.position = transform.position + (-transform.forward * dzMainCamera + transform.up * dyMainCamera);
					Camera.main.transform.rotation = Quaternion.Slerp (Camera.main.transform.rotation, 
						Quaternion.LookRotation (transform.position - Camera.main.transform.position), 3.5f * Time.deltaTime);
				}
			} else if ("car3".Equals (type)) {
				float rolX = transform.rotation.eulerAngles.x;
				float rolY = transform.rotation.eulerAngles.y;
				if (transform.position.y >= 0) {
					rolX = 0;
					rolY = PlanControls.newPoint1Rolation;
				} else if (transform.position.y < 0) {
					rolX = 90;
					rolSpeed = 0.022f;
				}

				Quaternion targetRotation = Quaternion.Euler (rolX, 
					                            rolY,
					                            0);
				transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * rolSpeed);

				((CarType3)mPojulObject).rotatePan (dRotate3);
				dRotate3 = 0;
				//((CarType3)mPojulObject).rotatePao (rotate2);

				if (!UImanager.isCamreaMoveTo) {
					((CarType3)mPojulObject).rotateCamera ();
				}

				if (transform.position.y < -1800) {
					//Destroy (this.gameObject);
					mPojulObject.isDestoryed = true;

					((CarType3)mPojulObject).destoryData ();
					((CarType3)mPojulObject).destoryAll ();
				}

			} else if ("littlecannon1".Equals (type)) {
				((LittleCannon1)mPojulObject).rotatePan (PlanControls.newPoint1Rolation ,dRotate3);
				dRotate3 = 0;
				if (!UImanager.isCamreaMoveTo) {
					((LittleCannon1)mPojulObject).rotateCamera ();
				}
			} else if ("homepao".Equals (type)) {
				((HomePao)mPojulObject).rotatePan (PlanControls.newPoint1Rolation ,dRotate3);
				dRotate3 = 0;
				if (!UImanager.isCamreaMoveTo) {
					((HomePao)mPojulObject).rotateCamera ();
				}
			}

			if(mRigidbody != null && "car3".Equals(type) && transform.position.y >= 0){
				mRigidbody.AddForce (transform.forward * speed * 2f, ForceMode.Force);
				//Debug.Log ("gqb------>" + mRigidbody.velocity.magnitude);
			}else if(mRigidbody != null){
				mRigidbody.AddForce (transform.forward * speed * 2f, ForceMode.Force);
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