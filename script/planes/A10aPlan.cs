using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class A10aPlan : PojulObject {

	public static float planeHeight = 6f;//60f;
	public static float cameraX = 0.33f;
	public static float cameraY = 3.07f;
	public static float cameraZ = 44.08f;
	
	public static float air_ray1X = 13.8f;
	public static float air_ray1Y = -1.06348f;
    public static float air_ray1Z = -44.96f;
	
	public static float air_ray2X = -13.33f;
	public static float air_ray2Y = -1.06348f;
    public static float air_ray2Z = -44.96f;

	public GameObject airRay1;
	public GameObject airRay2;

	private AudioSource mAudioSource;

	private Transform transform_lod0;
	private Transform transform_lod1;
	private Transform transform_lod2;

	private Transform[] fires = new Transform[8];

	private Transform aimTransform;

	private float maxMoveSpeed = GameInit.mach;
	private float moveSpeed = 0.0f;
	private float rolateSpeed = 30f;
	private GameObject navCube;
	private UnityEngine.AI.NavMeshAgent nav;

	private RadiusArea mPatrolArea;
	private RadiusArea mAttackPatrolArea;

	private Transform target;

	private float minHeight = 1000;
	private float maxHeight = 12000;
	private float piRate = 2* Mathf.PI/360;
	private float initHeight = 126.2f;
	private float height = 7000;
	private float avoidAngle = 0;

	private List<Transform> missilePoses = new List<Transform>();
	private Dictionary<Transform, Transform> missiles = new Dictionary<Transform, Transform> ();
	public int maxMountMissle = 3;
	public int currentMountMissle = 0;
	private GameObject inflameObj;
	private Transform missileCar;

	private Rigidbody mPlayerRigidbody;

	//血量条
	public Slider sliderHealth;
	public Canvas mCanvas;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (transform.position.x, initHeight, transform.position.z);
		maxMountMissle = 3;
		currentMountMissle = 0;

		addAirRay ();

		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		transform_lod0 = transform.FindChild ("A10a_lod0");
		transform_lod1 = transform.FindChild ("A10a_lod1");
		transform_lod2 = transform.FindChild ("A10a_lod2");

		missilePoses.Add (transform.FindChild ("missilePos1"));
		missilePoses.Add (transform.FindChild ("missilePos2"));
		missilePoses.Add (transform.FindChild ("missilePos3"));
		missilePoses.Add (transform.FindChild ("missilePos4"));

		missiles.Add (missilePoses[0], null);
		missiles.Add (missilePoses[1], null);
		missiles.Add (missilePoses[2], null);
		missiles.Add (missilePoses[3], null);

		fires[0] = transform.FindChild ("fire1");
		fires[1] = transform.FindChild ("fire2");
		fires[2] = transform.FindChild ("fire3");
		fires[3] = transform.FindChild ("fire4");
		fires[4] = transform.FindChild ("fire5");
		fires[5] = transform.FindChild ("fire6");
		fires[6] = transform.FindChild ("fire7");
		fires[7] = transform.FindChild ("fire8");


		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];

		int areaId = -1;
		RectTransform mRectTransform = sliderHealth.GetComponent<RectTransform> ();
		if ("0".Equals (playerId)) {
			mPatrolArea = new RadiusArea (Random.Range(1,4));
			myCenter = new Vector3 (0,0,-60000);
			enemyCenter = new Vector3 (0,0,60000);
			enemyId = "1";
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.251f, 0.647f, 0.78f);
			mRectTransform.sizeDelta = new Vector2 (0, 0);
		} else if ("1".Equals (playerId)) {
			mPatrolArea = new RadiusArea (Random.Range(5,8));
			myCenter = new Vector3 (0,0,60000);
			enemyCenter = new Vector3 (0,0,-60000);
			enemyId = "0";
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.698f, 0.255f, 0.157f);
			mRectTransform.sizeDelta = new Vector2 (Screen.width / 18, Screen.width / 60);
			//for test
			//transform.position = new Vector3 (0, initHeight, -10000);

		}
		mPatrolArea.minRange = 25000;
		sliderHealth.value = sliderHealth.maxValue;

		createNavCube ();

		if(mAudioSource != null && !mAudioSource.isPlaying){
			mAudioSource.Play ();
		}

		if(playerType == 1){
			//startNav(mPatrolArea.getRandomPoint());
			mAudioSource.volume =0.4f;
		}else if(playerType == 0){
			mAudioSource.volume = 0.015f;
		}

		//mRigidbody = transform.gameObject.GetComponent<Rigidbody> ();


		InvokeRepeating("behaviorListener", 0.5f, 0.5f);
		InvokeRepeating ("findInvade", 2.0f, 2.0f);
		InvokeRepeating ("interceptInvade", 5.0f, 5.0f);
		InvokeRepeating("avoidMissile", 0.9f, 0.9f);


		//transform.gameObject.
		//addMissile ();

		//initMisslie ();

	}
		
	void Update () {
		if(isDestoryed){
			return;
		}

		if(playerType == 1){
			airMoveAuto ();
		}else if(playerType == 0){
			planMove.currentMountMissle = currentMountMissle;
			//Debug.Log ("gqb------>fireAim: " + UImanager.fireAim);
		}

		sliderHealth.transform.rotation = Quaternion.Euler(transform_lod0.rotation.eulerAngles.x , 
			Camera.main.transform.rotation.eulerAngles.y,
			transform_lod0.rotation.eulerAngles.z);
	}

	void airMoveAuto(){
		if(playerType == 0){
			return;
		}
		float dRolX = transform.rotation.eulerAngles.x;

		//if(target == null){
			if( (myCenter - transform.position).magnitude > 9000 && (Mathf.Abs((transform.position.y - height)) > 200) ){
				if(nav.hasPath && !nav.pathPending){
					//Debug.Log ("gqb------>x: " + nav.pathEndPosition.x +"; y: " + nav.pathEndPosition.y + "; z: " + nav.pathEndPosition.z);
					if(MissileAimedTra != null && isAvoidMissile){
						dRolX = 12 * avoidAngle * Time.deltaTime;
					}else{
						dRolX = Quaternion.Slerp(transform.rotation, 
							Quaternion.LookRotation(new Vector3(nav.pathEndPosition.x,height,nav.pathEndPosition.z) - transform.position), 5 * Time.deltaTime)
							.eulerAngles.x;
					}
				}
			}
		//}

		if(dRolX > 60 && dRolX < 180){
			dRolX = 60;
		}else if(dRolX >= 180 && dRolX < 300){
			dRolX = 300;
		}
		float dy = -moveSpeed * Mathf.Sin (piRate * dRolX) * Time.deltaTime;
		nav.speed = moveSpeed * Mathf.Cos (piRate * dRolX);

		transform.position = new Vector3 (navCube.transform.position.x, 
			transform.position.y + dy,
			navCube.transform.position.z);
		transform.rotation = Quaternion.Euler( dRolX, 
			navCube.transform.rotation.eulerAngles.y,
			navCube.transform.rotation.eulerAngles.z);
			//navCube.transform.rotation;
	}

	public override void setPlayType(int playerType){
		this.playerType = playerType;
		if(playerType == 0){
			sliderHealth.GetComponent<RectTransform> ().sizeDelta = new Vector2(0,0);
			if(nav != null){
				nav.enabled = false;
			}
			if(mAudioSource != null){
				mAudioSource.volume = 0.1f;
			}
			if(!transform.gameObject.GetComponent<planMove> ()){
				transform.gameObject.AddComponent<planMove> ();
			}
			if(mPlayerRigidbody == null){
				transform.gameObject.AddComponent<Rigidbody> ();
				mPlayerRigidbody = transform.gameObject.GetComponent<Rigidbody> ();
				mPlayerRigidbody.useGravity = false;
				mPlayerRigidbody.drag = 2f;
				mPlayerRigidbody.angularDrag = 2f;
				planMove.mRigidbody = mPlayerRigidbody;
			}
			planMove.player = transform;
			planMove.speed = moveSpeed;
			planMove.maxSpeed = 1000;
			planMove.maxAccelerate = 1.5f;
			PlanControls.rorateSpeed = 65f;
			if(fires [0] == null){
				fires [0] = transform.FindChild ("fire1");
			}
			WorldUIManager.fireAimTra = fires [0];
			WorldUIManager.fireAimDistance = 21000.0f;
			GameObject.FindGameObjectWithTag ("mainUI").GetComponent<UImanager> ().setPlayerUI("a10");
		}else if(playerType == 1){
			/*
			*need judge is in nav area
			*/
			sliderHealth.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width / 18, Screen.width / 60);
			if (nav != null) {
				nav.enabled = true;
			}
			if (mAudioSource != null) {
				mAudioSource.volume = 1.0f;
			}
			if (mPlayerRigidbody != null) {
				Destroy (transform.gameObject.GetComponent<Rigidbody> ());
				planMove.mRigidbody = null;
			}
			if(transform.gameObject.GetComponent<planMove> ()){
				Destroy (transform.gameObject.GetComponent<planMove> ());
			}
			planMove.player = null;
			WorldUIManager.fireAimTra = null;
			WorldUIManager.fireAimDistance = 0.0f;
		}
	}

	void addAirRay (){
		airRay1 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/air_ray_A101"), 
		
			new Vector3(transform.position.x + air_ray1X, 
						transform.position.y + air_ray1Y, 
						transform.position.z+ air_ray1Z), 
					Quaternion.Euler(0, 180, 0)) as GameObject;
		airRay1.transform.parent = transform;

		//airRay2 = (GameObject)Instantiate(Resources.Load((string)GameInit.modelpaths["air_ray_A10"]), 
		airRay2 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/air_ray_A101"), 
			new Vector3(transform.position.x + air_ray2X, 
				transform.position.y + air_ray2Y, 
				transform.position.z+ air_ray2Z), 
			Quaternion.Euler(0, 180, 0)) as GameObject;
		airRay2.transform.parent = transform;
	}

	void behaviorListener(){
		//rayCastEnemy ();
		if(isDestoryed){
			return;
		}

		if (MissileAimedTra == null) {
			isMissileAimed = false;
		} else {
			PojulObject p = MissileAimedTra.gameObject.GetComponent<PojulObject> ();
			if (p.missTarget) {
				isMissileAimed = false;
			} else {
				isMissileAimed = true;
			}
		}

		if(playerType == 0){
			//isTargetInFire ();
			addMissileOfPlayer();
			return;
		}

		if(MissileAimedTra == null){
			isTargetInFire ();
		}

		if(nav != null && behavior == 1  && 
			(nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid || nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial) ){
			startNav(mPatrolArea.getRandomPoint());
		}

		if(nav != null && behavior == 3  &&  mAttackPatrolArea != null &&
			(nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid || nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial) ){
			startNav(mAttackPatrolArea.getRandomPoint());
		}

		if(MissileAimedTra != null && isAvoidMissile){
			return;
		}

		if(missileCar != null){
			if( (missileCar.position - transform.position).magnitude < 3000){
				addMissile ();
			}
			return;
		}

		if(nav != null && behavior == 1 && !nav.hasPath && !nav.pathPending && mPatrolArea != null){
			startNav(mPatrolArea.getRandomPoint());
		}else if(nav != null && behavior == 3 && !nav.hasPath && !nav.pathPending && mAttackPatrolArea != null){
			//while is attacking but no missiles need back to home ,not in eneny area
			startNav(mAttackPatrolArea.getRandomPoint());
		}
	}

	public void setAttackPatrolArea(int areaId){
		mAttackPatrolArea = new RadiusArea (areaId);
	}

	void isTargetInFire(){
		if(target == null){
			return;
		}
		PojulObject mPojulObject = target.parent.gameObject.GetComponent<PojulObject> ();
		if(mPojulObject == null || mPojulObject.isMissileAimed || mPojulObject.isDestoryed){
			return;
		}
		Quaternion angle = Quaternion.LookRotation (target.position - transform.position);
		Quaternion angle1 = Quaternion.LookRotation (transform.forward);
		float dy1 = Mathf.Abs (angle1.eulerAngles.y - angle.eulerAngles.y);
		float dy2 = 360 - dy1;
		float dx1 = Mathf.Abs (angle1.eulerAngles.x - angle.eulerAngles.x);
		float dx2 = 360 - dx1;

		float dx = Mathf.Min (dx1, dx2);
		float dy = Mathf.Min(dy1, dy2);

		float distance = (transform.position - target.position).magnitude;

		//Debug.DrawRay(transform.position, (target.position - transform.position).normalized *16000, Color.white);
		RaycastHit hit;
		if(dx < 30 && dy < 45 && distance < 20000 
			&& Physics.Raycast (transform.position, (target.position - transform.position).normalized, out hit, 21000.0f)){
			if(!hit.transform.root.tag.Equals("Untagged") || (hit.transform.root.childCount > 0 && !hit.transform.root.GetChild(0).tag.Equals("Untagged"))){
				fireMissile ();
			}
		}
		//Debug.Log ("gqb------>d angle x: " + Mathf.Min(dx1, dx2) + "; d angle y: " + Mathf.Min(dy1, dy2) + "; distance: " + distance);
	}

	void fireMissile(){
		if(currentMountMissle <= 0){
			return;
		}
		for (int i = 0; i < missilePoses.Count; i++) {
			if(missiles [missilePoses[i]] != null){
				MissileType3 mMissileType3 = missiles [missilePoses [i]].gameObject.GetComponent<MissileType3> ();
				float startSpeed = moveSpeed;
				if(playerType == 0){
					startSpeed = planMove.speed;
				}
				PojulObject mPojulObject = target.parent.gameObject.GetComponent<PojulObject> ();
				mMissileType3.fireInit ((playerId + "_missile3" ), target, startSpeed);
				if(mPojulObject != null){
					mPojulObject.isMissileAimed = true;
					mPojulObject.MissileAimedTra = missiles [missilePoses [i]];
				}
				missiles [missilePoses [i]] = null;
				currentMountMissle = currentMountMissle  - 1;
				break;
			}
		}
	}

	public override void fireMissileOfPlayer(Transform target){
		if(currentMountMissle <= 0){
			return;
		}
		for (int i = 0; i < missilePoses.Count; i++) {
			if(missiles [missilePoses[i]] != null){
				MissileType3 mMissileType3 = missiles [missilePoses [i]].gameObject.GetComponent<MissileType3> ();
				float startSpeed = planMove.speed;
				PojulObject mPojulObject = target.gameObject.GetComponent<PojulObject> ();
				mMissileType3.fireInit ((playerId + "_missile3" ), target, startSpeed);
				if(mPojulObject != null){
					mPojulObject.isMissileAimed = true;
					mPojulObject.MissileAimedTra = missiles [missilePoses [i]];
				}
				missiles [missilePoses [i]] = null;
				currentMountMissle = currentMountMissle  - 1;
				break;
			}
		}
	}

	public void startNav(Vector3 navPoint){
		if(navCube == null){
			createNavCube ();
		}
		if(!nav.enabled){
			return;
		}
		nav.destination = navPoint;//target.transform.position;
		float patrol = Random.Range(maxMoveSpeed*0.7f, maxMoveSpeed);
		height = Random.Range(minHeight, maxHeight);
		//Debug.Log ("gqb------>startNav height: " + height);
		moveSpeed = patrol;
		nav.speed = moveSpeed;
		nav.acceleration = moveSpeed * 0.6f;
		nav.autoRepath = true;
		//nav.baseOffset = 50;
		nav.angularSpeed = rolateSpeed;
	}

	public void startNav(Vector3 navPoint, float speed){
		
		if(navCube == null){
			createNavCube ();
		}
		if(!nav.enabled){
			return;
		}
		nav.destination = navPoint;//target.transform.position;
		height = Random.Range(minHeight, maxHeight);
		moveSpeed = speed;
		nav.speed = moveSpeed;
		nav.acceleration = moveSpeed * 0.6f;
		nav.autoRepath = true;
		nav.angularSpeed = rolateSpeed;
	}

	public void createNavCube(){
		navCube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		navCube.transform.position = new Vector3(transform.position.x, planeHeight, transform.position.z);
		navCube.transform.localScale = new Vector3 (10, 10, 10);
		navCube.AddComponent<UnityEngine.AI.NavMeshAgent>();
		Destroy(navCube.GetComponent<BoxCollider> ());
		//transform.parent = navCube.transform;

		MeshRenderer m = navCube.GetComponent<MeshRenderer>();
		m.enabled = false;
		nav= navCube.GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	void findInvade(){
		if(isDestoryed){
			return;
		}
		if(currentMountMissle < maxMountMissle){
			findMissileCar ();
		}
		//if (MissileAimedTra != null) {
			//return;
		//}

		List<Transform> tempNearEnemy = new List<Transform> ();

		Collider[] colliders = Physics.OverlapSphere (transform.position, 25000);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders [i].transform.root.childCount <= 0) {
				continue;
			}
			Transform tempTransform = colliders [i].transform.root.GetChild (0);
			string tag = tempTransform.tag;
			if(tag.Equals("Untagged")){
				if (colliders [i].transform == null) {
					continue;
				}
				if(colliders [i].transform.tag.Equals("Untagged")){
					continue;
				}
				tempTransform = colliders [i].transform;
			}
			tag = tempTransform.tag;
			//Debug.Log ("gqb------>findInvade tag: " + tag);
			string[] strs = tempTransform.tag.Split ('_');
			if (strs.Length == 2) {
				string tempPlayerId = strs [0];
				string tempType = strs [1];
				if (enemyId.Equals (tempPlayerId)) {
					if (target == null && ("a10".Equals (tempType) || "car2".Equals (tempType) || "car3".Equals (tempType)
					   || "a10".Equals (tempType) || "car5".Equals (tempType))) {
						target = tempTransform.FindChild ("aim");
					}
					Util.AddNearEnemys (tempTransform, playerId);
					if(playerType == 0){
						if(tempTransform != null && (transform.position - tempTransform.position).magnitude < 22000 && !tempNearEnemy.Contains(tempTransform)){
							tempNearEnemy.Add (tempTransform);
						}
					}
				}
			}
		}
		if (target == null) {
			findDangeroust ();
		}
		if(playerType == 0){
			lock(GameInit.locker){
				planMove.nearEnemy = tempNearEnemy;
			}
		}
	}


	void avoidMissile(){

		if(isDestoryed){
			return;
		}

		if (MissileAimedTra != null && (transform.position - MissileAimedTra.position).magnitude < 10000) {
			isAvoidMissile = true;
			//Debug.Log (height + "gqb------>avoidMissile");
			float maxAngle = 70;
			if(transform.position.y < 800){
				maxAngle = 0;
			}
			avoidAngle = Random.Range (-70, maxAngle);
			//height = ((int)Random.Range (1, 6)) * 2000f;
		} else {
			isAvoidMissile = false;
		}
	}

	void findDangeroust(){
		if(isDestoryed){
			return;
		}

		if(playerType ==0){
			return;
		}
		Dictionary<int, Dictionary<Transform, float>> nearEnemys = null;//----
		if("0".Equals(playerId)){
			nearEnemys = GameInit.nearEnemys_0;
		}else if("1".Equals(playerId)){
			nearEnemys = GameInit.nearEnemys_1;
		}
		if(nearEnemys.ContainsKey(mPatrolArea.areaId) && nearEnemys [mPatrolArea.areaId].Count > 0){
			Dictionary<Transform, float> tempTransforms = nearEnemys [mPatrolArea.areaId];//----
			/*if (tempTransforms [0] == null) {
				tempTransforms.Remove (tempTransforms [0]);
				return;
			}*/
			float dangeroustDis = 1000000.0f;
			Transform dangeroustEnemy = null;
			foreach(Transform key in tempTransforms.Keys){
				if(key == null){
					continue;
				}
				float tempDis = (myCenter - key.position).magnitude;
				if( tempDis < dangeroustDis && (Time.time - tempTransforms[key]) < 3.2f){
					dangeroustEnemy = key;
					dangeroustDis = tempDis;
				}

			}
			/*Transform dangeroustEnemy = tempTransforms [0];
			float dangeroustDis = (myCenter - dangeroustEnemy.position).magnitude;
			if(tempTransforms.Count > 1){
				for(int i = 1; i < tempTransforms.Count; i++){
					if(tempTransforms[i] = null){
						tempTransforms.Remove (tempTransforms [i]);
						continue;
					}
					if(tempTransforms[i] != null && (myCenter - tempTransforms[i].position).magnitude < dangeroustDis){
						dangeroustEnemy = tempTransforms [i];
						dangeroustDis = (myCenter - tempTransforms [i].position).magnitude;
					}
				}
			}*/
			if(dangeroustEnemy != null && dangeroustEnemy.FindChild("aim") != null){
				target = dangeroustEnemy.FindChild("aim");
			}
		}
	}


	void interceptInvade(){
		if(isDestoryed){
			return;
		}

		if(playerType ==0){
			return;
		}
		if(MissileAimedTra != null && isAvoidMissile){
			if(behavior ==1){
				startNav (mPatrolArea.getRandomPoint(), GameInit.mach*1.2f);
			}else if(behavior ==3){
				startNav (mAttackPatrolArea.getRandomPoint(), GameInit.mach*1.2f);
			}
			return;
		}

		updateCurrentMountMissle ();

		if((behavior == 1 || behavior == 3) && target != null && currentMountMissle > 0){
			startNav (new Vector3(target.position.x, planeHeight, target.position.z), maxMoveSpeed*0.8f);
			height = target.position.y;
		}else if(missileCar != null){
			startNav (new Vector3((missileCar.position.x + Random.Range(100,300)), planeHeight, (missileCar.position.z + Random.Range(100,300))), maxMoveSpeed*0.8f);
			height = missileCar.position.y;
		}
		if (height < 1000){
			height = 1000;
		}else if(height > 12000){
			height = 12000;
		}

	}

	void updateCurrentMountMissle(){
		int temptMountMissle = 0;
		for (int i = 0; i < missilePoses.Count; i++) {
			if(missiles [missilePoses[i]] != null){
				temptMountMissle = temptMountMissle + 1;
			}
		}
		currentMountMissle = temptMountMissle;
	}

	public override void isFired(Collision collision, int type){

		//test
		if(playerType == 0){
			return;
		}

		if(isDestoryed){
			return;
		}
		if(type ==3 || type ==2){
			isDestoryed = true;
			isPanDestoryed = true;
			inflame(collision);
			return;
		}
	}

	void inflame(Collision collision){

		GameInit.currentInstance [(playerId + "_missile3")] = GameInit.currentInstance [(playerId + "_missile3")] - currentMountMissle;
		currentMountMissle = 0;

		if(transform_lod0 == null){
			return;
		}

		//transform_lod0.parent = null;
		//transform_lod1.parent = transform_lod0;
		//transform_lod2.parent = transform_lod0;

		Destroy (airRay1);
		Destroy (airRay2);

		if(!transform.gameObject.GetComponent<Rigidbody>()){
			transform.gameObject.AddComponent<Rigidbody>();
		}
		Rigidbody mRigidbody = transform.gameObject.GetComponent<Rigidbody>();
		if(mRigidbody != null){
			mRigidbody.AddForce (transform.forward * 72000);
		}
		if(mAudioSource != null){
			mAudioSource.Stop ();
		}

		inflameObj = (GameObject)Instantiate(Resources.Load((string)GameInit.modelpaths["inflame"]), 
			transform_lod0.position, Quaternion.Euler(0, 0, 0)) as GameObject;
		inflameObj.transform.parent = transform_lod0;

		Transform aim = transform.FindChild ("aim");
		if(aim != null){
			Destroy (aim.gameObject);
		}

		if(GameInit.currentInstance.ContainsKey((string)tag)){
			GameInit.currentInstance[tag] = (int)GameInit.currentInstance[tag] - 1;
		}
		if(GameInit.gameObjectInstance.Contains(transform.gameObject)){
			GameInit.gameObjectInstance.Remove (transform.gameObject);
		}
		if("0".Equals(playerId) && GameInit.myThumbnailObjs.Contains(transform.gameObject)){
			GameInit.myThumbnailObjs.Remove (transform.gameObject);
		}

		if(playerType == 0){
			planMove.player = null;
		}

		Invoke ("destoryAll", 40);

	}

	void destoryAll(){
		Destroy (navCube);
		Destroy (transform_lod0.gameObject);
		Destroy (this.gameObject);
	}

	void addMissileOfPlayer(){
		for(int i = 0; i < GameInit.MyCar2.Count; i++){
			if(GameInit.MyCar2[i] == null){
				continue;
			}
			if( (GameInit.MyCar2[i].position - transform.position).magnitude < 3000){
				missileCar = GameInit.MyCar2 [i];
				addMissile ();
				missileCar = null;
				break;
			}
		}
	}

	void addMissile(){
		if (missileCar == null) {
			return;
		}
		CarType2 mCarType2 = missileCar.gameObject.GetComponent<CarType2> ();
		if(mCarType2.currentMissiles[2] <= 0){
			missileCar = null;
			return;
		}

		int needNum = maxMountMissle - currentMountMissle;
		int canSupplyNum = mCarType2.currentMissiles[2];
		int exchangeNum = 0;
		if (canSupplyNum <= needNum) {
			exchangeNum = canSupplyNum;
			mCarType2.currentMissiles [2] = 0;
		} else {
			exchangeNum = needNum;
			mCarType2.currentMissiles [2] = mCarType2.currentMissiles [2] - needNum;
		}
		int addednum = 0;
		for(int i = 0; i< missilePoses.Count; i++){
			if(addednum >= exchangeNum){
				missileCar = null;
				return;
			}
			if (missiles [missilePoses [i]] == null) {
				GameObject prefab = (GameObject)Instantiate(Resources.Load((string)GameInit.modelpaths["missile3"]), 
					missilePoses[i].position, missilePoses[i].rotation) as GameObject;
				missiles [missilePoses[i]] = prefab.transform;
				prefab.transform.parent = transform;
				currentMountMissle = currentMountMissle + 1;
				addednum = addednum + 1;
			}
		}
	}

	void findMissileCar(){
		if(playerType == 0){
			return;
		}
		if(MissileAimedTra != null && isAvoidMissile){
			return;
		}
		if(target != null && currentMountMissle > 0){
			return;
		}
		if(behavior ==3 && currentMountMissle > 0){
			return;
		}
		//test
		//if("1".Equals(playerId)){
			//return;
		//}

		List<Transform> Car2s = new List<Transform> ();
		if("0".Equals(playerId)){
			Car2s = GameInit.MyCar2;
		}else if("1".Equals(playerId)){
			Car2s = GameInit.EnemyCar2;
		}
		float distance = 1000000;
		Transform tempTra = null;
		for(int i =0; i< Car2s.Count; i++ ){
			if(Car2s[i] == null){
				continue;
			}
			CarType2 mCarType2 = Car2s[i].gameObject.GetComponent<CarType2> ();
			float tempDistance = (transform.position - Car2s [i].position).magnitude;
			if(mCarType2.currentMissiles[2] > 0 && (Car2s[i].position - myCenter).magnitude > 9000 
				&& tempDistance < distance){
				tempTra = Car2s [i];
				distance = tempDistance;
			}
		}
		if(tempTra != null){
			missileCar = tempTra;
		}
	}

}
