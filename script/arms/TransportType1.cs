using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	private Transform mainTransform_lod0;
	private Transform mainTransform_lod1;
	private Transform mainTransform_lod2;

	private Transform force1_lod0;
	private Transform force2_lod0;

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
	private float height = 70;//104;//84; //190;
	private float maxMoveSpeed = GameInit.mach * 1.1f;
	private float aniSpeed = 0.0f;
	private float navPathDistance = -1f;
	private float maxFlyHeight = 10000.0f;
	//private float flyHeight = 190.0f;

	private Vector3 park;
	public GameObject navCube;
	public UnityEngine.AI.NavMeshAgent nav;
	private Vector3 transportTo;

	public List<Vector3> attackMasses;
	public List<Transform> attackTransports;
	public List<Vector3> attackAreas;
	public float transportOutTime = 0;

	private bool ispropeller1Destory = false;
	private bool ispropeller2Destory = false;

	//血量条
	public Slider sliderHealth;

	void Start () {
		transform.name = "Transport1";
		transform.position = new Vector3 (transform.position.x, initHeight, transform.position.z);

		transform_lod0 = transform.FindChild ("transport_type_lod0");
		transform_lod1 = transform.FindChild ("transport_type_lod1");
		transform_lod2 = transform.FindChild ("transport_type_lod2");

		//mAnimator_lod0 = (Animator)transform_lod0.GetComponent<Animator> ();
		//mAnimator_lod1 = (Animator)transform_lod1.GetComponent<Animator> ();
		//mAnimator_lod2 = (Animator)transform_lod2.GetComponent<Animator> ();

		mainTransform_lod0 = transform_lod0.FindChild ("main");
		force1_lod0 = mainTransform_lod0.FindChild ("force1");
		force2_lod0 = mainTransform_lod0.FindChild ("force2");
		mAudioSource = (AudioSource)mainTransform_lod0.GetComponent<AudioSource> ();

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

		RectTransform mRectTransform = sliderHealth.GetComponent<RectTransform> ();
		if ("0".Equals (playerId)) {
			park = Util.getIdlePart ("0");
			GameInit.park0 [park] = 1;
			enemyId = "1";
			myCenter = new Vector3 (0,0,-60000);
			enemyCenter = new Vector3 (0,0,60000);
			attackMasses = GameInit.attackMasses_0;
			attackAreas = GameInit.attackAreas_0;
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.251f, 0.647f, 0.78f);
		}else if ("1".Equals (playerId)){
			park = Util.getIdlePart ("1");
			GameInit.park1 [park] = 1;
			enemyId = "0";
			myCenter = new Vector3 (0,0,60000);
			enemyCenter = new Vector3 (0,0,-60000);
			attackMasses = GameInit.attackMasses_1;
			attackAreas = GameInit.attackAreas_1;
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.698f, 0.255f, 0.157f);
		}
		if(playerType == 1){
			mRectTransform.sizeDelta = new Vector2 (Screen.width / 18, Screen.width / 60);
		}
		sliderHealth.value = sliderHealth.maxValue;

		transportTo = park;
		//startNav (park);

		InvokeRepeating("behaviorListener", 0.5f, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		if(isMoving){
			listenerRollAni ();
		}

		sliderHealth.transform.rotation = Quaternion.Euler(transform_lod0.rotation.eulerAngles.x , 
			Camera.main.transform.rotation.eulerAngles.y,
			transform_lod0.rotation.eulerAngles.z);

		if(isDestoryed){
			onDestoried ();
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
				if(mPojulObject.isDestoryed || mPojulObject.behavior == 5){
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
			mAudioSource.volume = 1;
			mAudioSource.pitch = aniSpeed * 0.001f;
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
			mAudioSource.volume = 1;
			mAudioSource.pitch = aniSpeed * 0.001f;
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
		nav.angularSpeed = 60;
		navPathDistance = Mathf.Abs((new Vector3(transform.position.x, 0, transform.position.z) - navPoint).magnitude);
		//flyHeight = Random.Range (minFlyHeight, maxFlyHeight);
		//flyHeightSpeed = Mathf.Abs (flyHeightSpeed);
		//initHeight = 0;
	}

	public void createNavCube(){
		navCube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		navCube.transform.position = new Vector3(transform.position.x, 5, transform.position.z);
		navCube.transform.localScale = new Vector3 (10, 10, 10);
		navCube.AddComponent<UnityEngine.AI.NavMeshAgent>();
		Destroy(navCube.GetComponent<BoxCollider> ());
		//transform.parent = navCube.transform;

		MeshRenderer m = navCube.GetComponent<MeshRenderer>();
		m.enabled = false;
		nav= navCube.GetComponent<UnityEngine.AI.NavMeshAgent> ();
		//nav.baseOffset
	}

	public override void isFired(RaycastHit hit, Collision collision, int type){
		if (isDestoryed) {
			return;
		}

		if (type == 2) {
			Vector3 hitPoint;
			if (collision != null) {
				hitPoint = collision.contacts[0].point;
			} else {
				hitPoint = hit.point;
			}
			sliderHealth.value = sliderHealth.value - 42;
			if(sliderHealth.value <= 0){
				isDestoryed = true;
				isPanDestoryed = true;
				destoryData ();
				inflame(hitPoint, 122000);
				return;
			}
		} else if (type == 3) {
			sliderHealth.value = sliderHealth.value - 107;
			if (sliderHealth.value <= 0) {
				isDestoryed = true;
				isPanDestoryed = true;

				float ds1 = (collision.contacts[0].point - force1_lod0.position).magnitude;
				float ds2 = (collision.contacts[0].point - force2_lod0.position).magnitude;
				if (ds1 > ds2) {
					ispropeller1Destory = true;
				} else {
					ispropeller2Destory = true;
				}
				destoryData ();
				inflame (collision.contacts[0].point, 122000.0f);
				return;
			}
		}


	}

	public void destoryData(){
		for(int i =0; i < attackTransports.Count; i++){
			if(attackTransports[i] != null && attackTransports[i].GetComponent<CarType3>()){
				attackTransports [i].GetComponent<CarType3> ().destoryData ();
				attackTransports [i].GetComponent<CarType3> ().destoryAll ();
			}
		}

		if(GameInit.currentInstance.ContainsKey((string)tag)){
			GameInit.currentInstance[tag] = (int)GameInit.currentInstance[tag] - 1;
		}
		if(GameInit.gameObjectInstance.Contains(transform.gameObject)){
			GameInit.gameObjectInstance.Remove (transform.gameObject);
		}
	}

	void inflame(Vector3 point, float force){

		Destroy (transform_lod1.gameObject);
		Destroy (transform_lod2.gameObject);

		GameObject inflameObj = (GameObject)Instantiate(Resources.Load((string)GameInit.modelpaths["inflame"]), 
			point, Quaternion.Euler(0, 0, 0)) as GameObject;
		inflameObj.transform.parent = mainTransform_lod0;

		//test
		transform.position = new Vector3 (transform.position.x, 3000, transform.position.z);
		aniSpeed = 1000;
		ispropeller1Destory = true;

		mainTransform_lod0.parent = null;
		propeller1Transform_lod0.parent = mainTransform_lod0;
		propeller2Transform_lod0.parent = mainTransform_lod0;
		doorTransform_lod0.parent = mainTransform_lod0;
		transform.GetComponent<LODGroup>().enabled = false;

		mainTransform_lod0.gameObject.AddComponent<Rigidbody> ();
		//mainTransform_lod0.gameObject.GetComponent<Rigidbody> ().drag = 0.1f;
		//mainTransform_lod0.gameObject.GetComponent<Rigidbody> ().angularDrag = 0.1f;
		propeller1Transform_lod0.gameObject.GetComponent<MeshCollider> ().enabled = true;
		propeller2Transform_lod0.gameObject.GetComponent<MeshCollider> ().enabled = true;
		//mainTransform_lod0.gameObject.GetComponent<MeshRenderer> ().enabled = true;
		//propeller1Transform_lod0.gameObject.GetComponent<MeshRenderer> ().enabled = true;
		//propeller2Transform_lod0.gameObject.GetComponent<MeshRenderer> ().enabled = true;

		Vector3 explosionPos = new Vector3 (point.x, 
			(point.y - 20), 
			point.z);
		if(ispropeller1Destory){
			propeller1Transform_lod0.parent = null;
			propeller1Transform_lod0.gameObject.AddComponent<Rigidbody> ();
			//propeller1Transform_lod0.gameObject.GetComponent<Rigidbody> ().drag = 0.1f;
			//propeller1Transform_lod0.gameObject.GetComponent<Rigidbody> ().angularDrag = 0.1f;
			propeller1Transform_lod0.GetComponent<Rigidbody>().AddExplosionForce(force, explosionPos, 300.0f);
		}

		if(ispropeller2Destory){
			propeller2Transform_lod0.parent = null;
			propeller2Transform_lod0.gameObject.AddComponent<Rigidbody> ();
			//propeller2Transform_lod0.gameObject.GetComponent<Rigidbody> ().drag = 0.1f;
			//propeller2Transform_lod0.gameObject.GetComponent<Rigidbody> ().angularDrag = 0.1f;
			propeller2Transform_lod0.GetComponent<Rigidbody>().AddExplosionForce(force, explosionPos, 300.0f);
		}

		Invoke ("destoryAll", 80);
	}

	public void destoryAll(){
		if(propeller1Transform_lod0 != null){
			Destroy (propeller1Transform_lod0.gameObject);
		}
		if(propeller2Transform_lod0 != null){
			Destroy (propeller2Transform_lod0.gameObject);
		}
		if(mainTransform_lod0 != null){
			Destroy (mainTransform_lod0.gameObject);
		}
		if(transform_lod0 != null){
			Destroy (transform_lod0.gameObject);
		}
		Destroy(transform.root.gameObject);
	}

	void onDestoried(){
		propeller1Transform_lod0.localEulerAngles = new Vector3(propeller1Transform_lod0.localEulerAngles.x, 
			(propeller1Transform_lod0.localEulerAngles.y + aniSpeed * Time.deltaTime), propeller1Transform_lod0.localEulerAngles.z);
		propeller2Transform_lod0.localEulerAngles = new Vector3(propeller2Transform_lod0.localEulerAngles.x, 
			(propeller2Transform_lod0.localEulerAngles.y + aniSpeed * Time.deltaTime), propeller2Transform_lod0.localEulerAngles.z);

		if (ispropeller1Destory) {
			mainTransform_lod0.GetComponent<Rigidbody> ().AddForceAtPosition (force1_lod0.right * 0.05f * aniSpeed, 
				force1_lod0.position);
		}else{
			mainTransform_lod0.GetComponent<Rigidbody> ().AddForceAtPosition (force1_lod0.up * 0.1f * aniSpeed, 
				force1_lod0.position);
		}

		if(ispropeller2Destory){
			mainTransform_lod0.GetComponent<Rigidbody> ().AddForceAtPosition (-force2_lod0.right * 0.05f * aniSpeed, 
				force2_lod0.position);
		}else{
			mainTransform_lod0.GetComponent<Rigidbody> ().AddForceAtPosition (force2_lod0.up * 0.1f * aniSpeed, 
				force2_lod0.position);
		}

		if(aniSpeed >= 1000){
			mainTransform_lod0.GetComponent<Rigidbody> ().AddForce (mainTransform_lod0.forward * 0.07f * aniSpeed);
			mainTransform_lod0.GetComponent<Rigidbody> ().AddForce (new Vector3 (0, 1, 0) * 0.08f * aniSpeed);
		}
	}

	public void setBombParent(GameObject bmob){
		if(mainTransform_lod0 != null){
			bmob.transform.parent = mainTransform_lod0;
		}
	}

}
