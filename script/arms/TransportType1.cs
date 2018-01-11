using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportType1 : PojulObject {

	private AudioSource mAudioSource;

	private Transform transform_lod0;
	private Transform transform_lod1;
	private Transform transform_lod2;

	//private Animator mAnimator_lod0;
	//private Animator mAnimator_lod1;
	//private Animator mAnimator_lod2;

	private Transform doorTransform_lod0;
	private Transform doorTransform_lod1;
	private Transform doorTransform_lod2;

	private Transform propeller1Transform_lod0;
	private Transform propeller2Transform_lod0;
	private Transform propeller1Transform_lod1;
	private Transform propeller2Transform_lod1;
	private Transform propeller1Transform_lod2;
	private Transform propeller2Transform_lod2;

	/*private Renderer mRenderer_lod0_propeller1;
	/*private Renderer mRenderer_lod0_propeller2;
	private Renderer mRenderer_lod1_propeller1;
	private Renderer mRenderer_lod1_propeller2;
	private Renderer mRenderer_lod0_propeller1;
	private Renderer mRenderer_lod0_propeller2;*/
	//private Renderer mRenderer_lod2_door;

	private bool isMoving = false;
	private float openDoorSpeed = 0.4f;
	private int initHeight = 2500;
	private float height = 88;//104;//84; //190;
	private float maxMoveSpeed = GameInit.mach * 0.8f;
	private float aniSpeed = 0.0f;
	private float navPathDistance = -1f;
	private float maxFlyHeight = 10000.0f;
	private float minFlyHeight = 9000.0f;
	//private float flyHeight = 190.0f;

	private Vector3 park;
	public GameObject navCube;
	public UnityEngine.AI.NavMeshAgent nav;
	private Vector3 transportTo;

	public List<Vector3> attackMasses;
	public List<Transform> attackTransports;
	public List<Vector3> attackAreas;
	public float transportOutTime = 0;


	void Start () {
		transform.name = "Transport1";
		transform.position = new Vector3 (transform.position.x, initHeight, transform.position.z);

		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		transform_lod0 = transform.FindChild ("transport_type_lod0");
		transform_lod1 = transform.FindChild ("transport_type_lod1");
		transform_lod2 = transform.FindChild ("transport_type_lod2");

		//mAnimator_lod0 = (Animator)transform_lod0.GetComponent<Animator> ();
		//mAnimator_lod1 = (Animator)transform_lod1.GetComponent<Animator> ();
		//mAnimator_lod2 = (Animator)transform_lod2.GetComponent<Animator> ();

		doorTransform_lod0 = transform_lod0.FindChild ("door");
		doorTransform_lod1 = transform_lod1.FindChild ("door");
		doorTransform_lod2 = transform_lod2.FindChild ("door");


		propeller1Transform_lod0 = transform_lod0.FindChild ("propeller1");
		propeller2Transform_lod0 = transform_lod0.FindChild ("propeller2");
		propeller1Transform_lod1 = transform_lod1.FindChild ("propeller1");
		propeller2Transform_lod1 = transform_lod1.FindChild ("propeller2");
		propeller1Transform_lod2 = transform_lod2.FindChild ("propeller1");
		propeller2Transform_lod2 = transform_lod2.FindChild ("propeller2");

		/*mRenderer_lod0_propeller1 = propeller1Transform_lod0.GetComponent<Renderer>();
		mRenderer_lod0_propeller2 = propeller2Transform_lod0.GetComponent<Renderer>();
		mRenderer_lod1_propeller1 = propeller1Transform_lod1.GetComponent<Renderer>();
		mRenderer_lod1_propeller2 = propeller2Transform_lod1.GetComponent<Renderer>();*/
		//mRenderer_lod2_door = doorTransform_lod2.GetComponent<Renderer>();
		createNavCube();

		run ();

		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];

		if ("0".Equals (playerId)) {
			park = Util.getIdlePart ("0");
			GameInit.park0 [park] = 1;
			enemyId = "1";
			myCenter = new Vector3 (0,0,-60000);
			enemyCenter = new Vector3 (0,0,60000);
			attackMasses = GameInit.attackMasses_0;
			attackAreas = GameInit.attackAreas_0;
		}else if ("1".Equals (playerId)){
			park = Util.getIdlePart ("1");
			GameInit.park1 [park] = 1;
			enemyId = "0";
			myCenter = new Vector3 (0,0,60000);
			enemyCenter = new Vector3 (0,0,-60000);
			attackMasses = GameInit.attackMasses_1;
			attackAreas = GameInit.attackAreas_1;
		}
		transportTo = park;
		//startNav (park);

		InvokeRepeating("behaviorListener", 0.5f, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		if(isMoving){
			listenerRollAni ();
		}
	}

	void behaviorListener(){
		if(isDestoryed){
			return;
		}
		if (behavior == 2) {
			ToMass ();
		} else if (behavior == 1) {
			transportTo = park;
			if ((nav.destination - park).magnitude > 500) {
				navPathDistance = -1;
				run ();
			}
		} else if (behavior == 3) {
			attackListener ();
		}
	}

	void ToMass(){
		int massId;
		if ("0".Equals (playerId)) {
			massId = UImanager.massId_0;
		} else {
			massId = UImanager.massId_1;
		}
		transportTo = attackMasses [massId];
		if ((nav.destination - attackMasses [massId]).magnitude > 500) {
			navPathDistance = -1;
			run ();
		}	
	}

	void ToAttack(){
		int attackId;
		if ("0".Equals (playerId)) {
			attackId = UImanager.attackAreaId_0;
		} else {
			attackId = UImanager.attackAreaId_1 -1;
		}
		transportTo = attackAreas [attackId];
		if ((nav.destination - attackAreas[attackId]).magnitude > 500) {
			//Debug.Log("gqb------>ToAttack: " + (nav.destination - attackAreas[attackId]).magnitude);
			navPathDistance = -1;
			run ();
		}	
	}

	void attackListener(){
		int attackId;
		if ("0".Equals (playerId)) {
			attackId = UImanager.attackAreaId_0;
		} else {
			attackId = UImanager.attackAreaId_1 -1;
		}
		if(attackTransports.Count > 0 && (new Vector3(transform.position.x, 0, transform.position.z) - attackAreas[attackId]).magnitude > 500){
			
			bool isHasNearTrans = hasNearTrans ();
			//Debug.Log("gqb------>hasNearTrans: " + isHasNearTrans);
			if(!isHasNearTrans || attackTransports.Count >= 3){
				ToAttack ();
				return;
			}
		}
		//Debug.Log("gqb------>dis: " + (transform.position - new Vector3(attackAreas[attackId].x, height, attackAreas[attackId].z)).magnitude);
		if((transform.position - new Vector3(attackAreas[attackId].x, height, attackAreas[attackId].z)).magnitude < 15 && attackTransports.Count > 0){
			if((Time.time - transportOutTime) > 8.2){
				if(attackTransports [0] ==null){
					attackTransports.RemoveAt (0);
					return;
				}
				PojulObject mPojulObject = attackTransports [0].GetComponent<PojulObject> ();
				if(mPojulObject == null || mPojulObject.isDestoryed){
					attackTransports.RemoveAt (0);
					return;
				}
				mPojulObject.setTransport (transform, false);
				transportOutTime = Time.time;
				attackTransports.RemoveAt (0);
				return;
			}
		}

		int massId;
		if ("0".Equals (playerId)) {
			massId = UImanager.massId_0;
		} else {
			massId = UImanager.massId_1;
		}
		if(attackTransports.Count <= 0 && (new Vector3(transform.position.x, 0, transform.position.z) - attackMasses[massId]).magnitude > 500){
			ToMass ();
			return;
		}

		if((new Vector3(transform.position.x, 0, transform.position.z) - attackMasses[massId]).magnitude < 10){
			RaycastHit hit;
			Vector3 firstMass = new Vector3 ((attackMasses [massId].x - 600), 
				0, 
				attackMasses [massId].z
			);
			if(Physics.Raycast (new Vector3(firstMass.x, 300, firstMass.z), Vector3.down, out hit, 500.0f)){
				if(hit.transform.root.childCount > 0 && !hit.transform.root.GetChild(0).tag.Equals("Untagged")){
					PojulObject mPojulObject = hit.transform.root.GetChild (0).GetComponent<PojulObject> ();
					if(mPojulObject == null){
						return;
					}
					if(mPojulObject.playerType != 0 && !mPojulObject.isDestoryed && attackTransports.Count < 3){
						mPojulObject.setTransport (transform, true);
						attackTransports.Add (hit.transform.root.GetChild (0));
						return;
					}
				}
			}

		}

	}

	bool hasNearTrans(){
		Collider[] colliders = Physics.OverlapSphere (transform.position, 6000);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders [i].transform.root.childCount > 0 && !colliders [i].transform.root.GetChild (0).tag.Equals ("Untagged")){
				Transform temp = colliders [i].transform.root.GetChild (0);
				if(!temp.GetComponent<PojulObject>()){
					continue;
				}
				PojulObject mPojulObject = temp.GetComponent<PojulObject> ();
				if(mPojulObject.isDestoryed || mPojulObject.behavior ==5){
					continue;
				}
				return true;
			}
		}
		return false;
	}

	void openDoor(){
		if(doorTransform_lod0.localEulerAngles.x > 301 || doorTransform_lod0.localEulerAngles.x < 1){
			doorTransform_lod0.rotation = Quaternion.Slerp(doorTransform_lod0.rotation, 
				Quaternion.Euler(new Vector3(300, doorTransform_lod0.localEulerAngles.y, doorTransform_lod0.localEulerAngles.z)), openDoorSpeed * Time.deltaTime);
			doorTransform_lod1.localEulerAngles = new Vector3(doorTransform_lod0.localEulerAngles.x, doorTransform_lod0.localEulerAngles.y, doorTransform_lod0.localEulerAngles.z);
			doorTransform_lod2.localEulerAngles = new Vector3(doorTransform_lod0.localEulerAngles.x, doorTransform_lod0.localEulerAngles.y, doorTransform_lod0.localEulerAngles.z);
		}
	}

	void closeDoor(){
		if(doorTransform_lod0.localEulerAngles.x < 359 && doorTransform_lod0.localEulerAngles.x > 1){
			doorTransform_lod0.rotation = Quaternion.Slerp(doorTransform_lod0.rotation, 
				Quaternion.Euler(new Vector3(0, doorTransform_lod0.localEulerAngles.y, doorTransform_lod0.localEulerAngles.z)), openDoorSpeed * Time.deltaTime);
			doorTransform_lod1.localEulerAngles = new Vector3(doorTransform_lod0.localEulerAngles.x, doorTransform_lod0.localEulerAngles.y, doorTransform_lod0.localEulerAngles.z);
			doorTransform_lod2.localEulerAngles = new Vector3(doorTransform_lod0.localEulerAngles.x, doorTransform_lod0.localEulerAngles.y, doorTransform_lod0.localEulerAngles.z);
		}
	}

	void listenerRollAni(){
		if(isDestoryed){
			return;
		}
		/*float posY = transform.position.y;
		if (Mathf.Abs (flyHeight - transform.position.y +initHeight) > 10) {
			posY = posY + flyHeightSpeed * Time.deltaTime;
		}*/
		//transform.position = new Vector3 (navCube.transform.position.x, 
			//posY,
			//navCube.transform.position.z);//
		transform.rotation = navCube.transform.rotation;

		propeller1Transform_lod0.localEulerAngles = new Vector3(propeller1Transform_lod0.localEulerAngles.x, 
			(propeller1Transform_lod0.localEulerAngles.y + aniSpeed * Time.deltaTime), propeller1Transform_lod0.localEulerAngles.z);
		
		propeller2Transform_lod0.localEulerAngles = new Vector3(propeller1Transform_lod0.localEulerAngles.x, 
			propeller1Transform_lod0.localEulerAngles.y, propeller1Transform_lod0.localEulerAngles.z);
		
		propeller1Transform_lod1.localEulerAngles = new Vector3(propeller1Transform_lod0.localEulerAngles.x, 
			propeller1Transform_lod0.localEulerAngles.y, propeller1Transform_lod0.localEulerAngles.z);
		
		propeller2Transform_lod1.localEulerAngles = new Vector3(propeller1Transform_lod0.localEulerAngles.x, 
			propeller1Transform_lod0.localEulerAngles.y, propeller1Transform_lod0.localEulerAngles.z);

		propeller1Transform_lod2.localEulerAngles = new Vector3(propeller1Transform_lod0.localEulerAngles.x, 
			propeller1Transform_lod0.localEulerAngles.y, propeller1Transform_lod0.localEulerAngles.z);

		propeller2Transform_lod2.localEulerAngles = new Vector3(propeller1Transform_lod0.localEulerAngles.x, 
			propeller1Transform_lod0.localEulerAngles.y, propeller1Transform_lod0.localEulerAngles.z);

		transform.rotation = navCube.transform.rotation;
		float tempHeight = transform.position.y;
		float remainDis = (new Vector3(transform.position.x, 0, transform.position.z) - nav.destination).magnitude;
			float heightScal = Mathf.Abs ( remainDis / navPathDistance );
		if(navPathDistance != -1 && heightScal < 1){
			if (heightScal > 0.48f || remainDis > 12500) {
				heightScal = 1;
			}
			float targetHeight = height + heightScal * maxFlyHeight;

			if((transform.position.y - targetHeight) < -8){
				tempHeight = tempHeight + 13;
			}else if((transform.position.y - targetHeight) > 8){
				tempHeight = tempHeight - 13;
			}
			//float tempHeightSpeed = 0.1f * (1.1f - heightScal) * 6;
			//Debug.Log(heightScal + "gqb------>tempHeight: " + tempHeight);
			//tempHeight = Vector3.Slerp(transform.position, 
				//new Vector3(transform.position.x, (height + heightScal*maxFlyHeight), transform.position.z)
				//Time.deltaTime*tempHeightSpeed).y;
		}
		//Debug.Log("gqb------>listenerRollAni: " + nav.pathStatus);
		transform.position = new Vector3 (navCube.transform.position.x, 
			tempHeight,
			navCube.transform.position.z);

		if((transform.position - transportTo).magnitude < (height + 20)) {
			endMove ();
		} else if (navPathDistance < 0) {
			startMove ();
		}

		//if(nav != null && !nav.hasPath && !nav.pathPending){
		//	
		//}
	}

	void startMove(){
		//2000
		//Debug.Log("gqb------>startMove");
		if (aniSpeed < 1000) {
			aniSpeed = aniSpeed + 2f;
			mAudioSource.volume = aniSpeed * 0.0007f;
		} else {
			if(transportTo != null){
				startNav (transportTo);
			}
		}
	}

	void endMove(){
		//Debug.Log("gqb------>endMove");
		if (aniSpeed > 2) {
			aniSpeed = aniSpeed - 2f;
			mAudioSource.volume = aniSpeed * 0.0007f;
		} else {
			navPathDistance = -1;
			isMoving = false;
			aniSpeed = 0.0f;
			if(mAudioSource != null && mAudioSource.isPlaying){
				mAudioSource.volume = aniSpeed;
				mAudioSource.Stop ();
			}
		}
	}

	void run (){
		isMoving = true;
		//aniSpeed = 0.0f;
		//flyHeight = 190;
		//mAnimator_lod0.speed = 4;
		//mAnimator_lod1.speed = 4;
		//mAnimator_lod0.SetBool ("roll", true);
		if(mAudioSource != null && !mAudioSource.isPlaying){
			mAudioSource.volume = aniSpeed;
			mAudioSource.Play ();
		}
	}

	public void startNav(Vector3 navPoint){
		if(navCube == null){
			createNavCube ();
		}
		nav.destination = navPoint;//target.transform.position;
		//float patrol = Random.Range(maxMoveSpeed*0.6f, maxMoveSpeed);
		nav.speed = maxMoveSpeed;
		nav.acceleration = maxMoveSpeed ;
		nav.autoRepath = true;
		//nav.baseOffset = 50;
		nav.angularSpeed = 30;
		navPathDistance = Mathf.Abs((new Vector3(transform.position.x, 0, transform.position.z) - navPoint).magnitude);
		//flyHeight = Random.Range (minFlyHeight, maxFlyHeight);
		//flyHeightSpeed = Mathf.Abs (flyHeightSpeed);
		//initHeight = 0;
	}

	public void createNavCube(){
		navCube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		navCube.transform.position = new Vector3(transform.position.x, 5, transform.position.z);
		navCube.transform.localScale = new Vector3 (500, 500, 500);
		navCube.AddComponent<UnityEngine.AI.NavMeshAgent>();
		//transform.parent = navCube.transform;

		MeshRenderer m = navCube.GetComponent<MeshRenderer>();
		m.enabled = false;
		nav= navCube.GetComponent<UnityEngine.AI.NavMeshAgent> ();
		//nav.baseOffset
	}

}
