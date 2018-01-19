using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class A10aPlan : PojulObject {

	public static float planeHeight = 6f;//60f;
	public static float cameraX = 0.33f;
	public static float cameraY = 3.07f;
	public static float cameraZ = 44.08f;

    public static float air_ray2Z = -44.96f;

	public GameObject airRay1;
	public GameObject airRay2;

	private AudioSource mAudioSource;

	private Transform transform_lod0;
	private Transform transform_lod1;
	private Transform transform_lod2;

	private Transform[] fires = new Transform[8];
	private Transform airRay1Pos;
	private Transform airRay2Pos;

	private Transform aimTransform;

	private float maxMoveSpeed = GameInit.mach;
	private float moveSpeed = 0.0f;
	private float rolateSpeed = 75f;
	private Vector3 backEnemyPos = new Vector3(0, 6f, 8818);
	private GameObject navCube;
	private UnityEngine.AI.NavMeshAgent nav;
	private bool isNavPathPartial = false;

	public RadiusArea mPatrolArea;

	private Transform target;

	private float minHeight = 1000;
	private float maxHeight = 12000;
	private float piRate = 2* Mathf.PI/360;
	private float initHeight = 126.2f;
	private float height = 7000;
	private float avoidAngle = 0;

	private List<Transform> missilePoses = new List<Transform>();
	private Dictionary<Transform, Transform> missiles = new Dictionary<Transform, Transform> ();
	public int maxMountMissle = 2;
	public int currentMountMissle = 0;
	private GameObject inflameObj;
	private Transform missileCar;

	private Rigidbody mPlayerRigidbody;
	int fireIndex = 0;
	private float lastFireTime = 0;
	private bool isInFire = false;

	//血量条
	public Slider sliderHealth;
	public Canvas mCanvas;

	public List<Vector3> attackMasses;
	public string airType = "";

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (transform.position.x, initHeight, transform.position.z);

		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		/*transform_lod0 = transform.FindChild ("A10a_lod0");
		transform_lod1 = transform.FindChild ("A10a_lod1");
		transform_lod2 = transform.FindChild ("A10a_lod2");*/

		transform_lod0 = transform.GetChild (0);
		transform_lod1 = transform.GetChild (1);
		transform_lod2 = transform.GetChild (2);
		airType = transform_lod0.name.Split ('_') [0];

		missilePoses.Add (transform.FindChild ("missilePos1"));
		missilePoses.Add (transform.FindChild ("missilePos2"));
		missilePoses.Add (transform.FindChild ("missilePos3"));
		missilePoses.Add (transform.FindChild ("missilePos4"));

		missiles.Add (missilePoses[0], null);
		missiles.Add (missilePoses[1], null);
		missiles.Add (missilePoses[2], null);
		missiles.Add (missilePoses[3], null);

		if("A10a".Equals(airType)){
			fires[0] = transform.FindChild ("fire1");
			fires[1] = transform.FindChild ("fire2");
			fires[2] = transform.FindChild ("fire3");
			fires[3] = transform.FindChild ("fire4");
			fires[4] = transform.FindChild ("fire5");
			fires[5] = transform.FindChild ("fire6");
			fires[6] = transform.FindChild ("fire7");
			fires[7] = transform.FindChild ("fire8");
		}

		airRay1Pos = transform.FindChild ("airRay1Pos");
		airRay2Pos = transform.FindChild ("airRay2Pos");

		addAirRay ();

		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];


		if("A10a".Equals(airType)){
			maxMoveSpeed = 0.5f * 3400;
		}else if("su34".Equals(airType)){
			maxMoveSpeed = 0.7f * 3400;
		}else if("F15E".Equals(airType)){
			maxMoveSpeed = 0.72f * 3400;
		}

		int areaId = -1;
		RectTransform mRectTransform = sliderHealth.GetComponent<RectTransform> ();
		currentMountMissle = 0;
		if ("0".Equals (playerId)) {
			mPatrolArea = new RadiusArea (Random.Range(1,4));
			myCenter = new Vector3 (0,0,-60000);
			enemyCenter = new Vector3 (0,0,60000);
			enemyId = "1";
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.251f, 0.647f, 0.78f);
			mRectTransform.sizeDelta = new Vector2 (0, 0);
			attackMasses = GameInit.attackMasses_0;
			maxMountMissle = 2;
		} else if ("1".Equals (playerId)) {
			mPatrolArea = new RadiusArea (Random.Range(5,8));
			myCenter = new Vector3 (0,0,60000);
			enemyCenter = new Vector3 (0,0,-60000);
			enemyId = "0";
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.698f, 0.255f, 0.157f);
			mRectTransform.sizeDelta = new Vector2 (Screen.width / 18, Screen.width / 60);
			attackMasses = GameInit.attackMasses_1;
			maxMountMissle = 4;
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
		health = sliderHealth.value;
		if(isDestoryed){
			return;
		}

		if(playerType == 1){
			if(isNavPathPartial){
				navCube.transform.position = navCube.transform.position + navCube.transform.forward * 12;
				navCube.transform.rotation = Quaternion.Euler(0, navCube.transform.rotation.eulerAngles.y, 0);
				transform.position = new Vector3 (navCube.transform.position.x, transform.position.y, navCube.transform.position.z);
				transform.rotation = navCube.transform.rotation;
			}
			airMoveAuto ();

			if ((Time.time - lastFireTime) > 0.12f && isInFire) {
				lastFireTime = Time.time;
				fire ();
			}

		}else if(playerType == 0){
			navCube.transform.position = new Vector3 (transform.position.x , planeHeight, transform.position.z);
			navCube.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
			planMove.currentMountMissle = currentMountMissle;
			//Debug.Log ("gqb------>fireAim: " + UImanager.fireAim);
		}

		sliderHealth.transform.rotation = Quaternion.Euler(transform_lod0.rotation.eulerAngles.x , 
			Camera.main.transform.rotation.eulerAngles.y,
			transform_lod0.rotation.eulerAngles.z);
	}

	void airMoveAuto(){
		if(!nav.enabled){
			return;
		}
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
				transform.rotation = navCube.transform.rotation;
				PlanControls.newPoint3Rolation = -(transform.rotation.eulerAngles.x + 360);
				PlanControls.newPoint1Rolation = transform.rotation.eulerAngles.y;
				PlanControls.newPoint2Rolation = -(int)(transform.rotation.eulerAngles.z + 360);
			}
			if(mAudioSource != null){
				mAudioSource.volume = 0.0f;
			}
			if(!transform.gameObject.GetComponent<planMove> ()){
				transform.gameObject.AddComponent<planMove> ();
			}
			if(mPlayerRigidbody == null){
				transform.gameObject.AddComponent<Rigidbody> ();
				mPlayerRigidbody = transform.gameObject.GetComponent<Rigidbody> ();
				mPlayerRigidbody.useGravity = false;
				mPlayerRigidbody.drag = 1.0f;
				mPlayerRigidbody.angularDrag = 1.0f;
				planMove.mRigidbody = mPlayerRigidbody;
			}
			planMove.player = transform;
			planMove.speed = maxMoveSpeed * 0.6f;
			planMove.maxSpeed = maxMoveSpeed;
			planMove.maxAccelerate = 5f;
			PlanControls.rorateSpeed = 92f;
			planMove.rolSpeed = 12.0f;
			if ("A10a".Equals (airType)) {
				if (fires [0] == null) {
					fires [0] = transform.FindChild ("fire1");
				}
				WorldUIManager.fireAimTra = fires [0];
			} else {
				WorldUIManager.fireAimTra = null;
			}

			WorldUIManager.fireAimDistance = 21000.0f;
			GameObject.FindGameObjectWithTag ("mainUI").GetComponent<UImanager> ().setPlayerUI("a10", airType);
			Camera.main.GetComponent<AudioSource> ().enabled = true;
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
			Camera.main.GetComponent<AudioSource> ().enabled = false;
		}
	}

	void addAirRay (){
		airRay1 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/air_ray_A101"), 
			airRay1Pos.position, 
			Quaternion.Euler(0, 180, 0)) as GameObject;
		airRay1.transform.parent = transform;

		//airRay2 = (GameObject)Instantiate(Resources.Load((string)GameInit.modelpaths["air_ray_A10"]), 
		airRay2 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/air_ray_A101"), 
			airRay2Pos.position,
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
		//Debug.Log ("gqb------>pathStatus: " + nav.pathStatus);

		if(nav != null && nav.enabled && 
			(nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial || nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid) && !isNavPathPartial){
			nav.enabled = false;
			isNavPathPartial = true;
			Invoke ("updatePartial", 1.5f);

			//Debug.Log ("gqb------>pathStatus: " + nav.pathStatus);
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
		if(behavior == 4){
			behavior = 1;
		}
		if (target != null && nav != null && nav.enabled && behavior == 1 && Util.isOnEnemyNavArea1(target.position, playerId)) {
			startNav(mPatrolArea.getRandomPoint());
			target = null;
		}

		if(nav != null && behavior == 1 && !nav.hasPath && !nav.pathPending && mPatrolArea != null){
			startNav(mPatrolArea.getRandomPoint());
		}else if(nav != null && behavior == 3 && !nav.hasPath && !nav.pathPending){
			toAttck ();
		}else if(nav != null && behavior == 2 && !nav.hasPath && !nav.pathPending){
			toMass ();
		}
	}

	public void onBehavorChanged(){
		//Debug.Log ("gqb------>onBehavorChanged" + behavior);
		if(behavior == 1){
			startNav(mPatrolArea.getRandomPoint());
		}else if(behavior == 3){
			toAttck();
		}else if(behavior == 2){
			toMass ();
		}
	}

	void toMass(){
		if(target != null){
			return;
		}
		int massId;
		if ("0".Equals (playerId)) {
			massId = UImanager.massId_0;
		} else {
			massId = UImanager.massId_1;
		}
		if ("1".Equals(playerId)) {
			startNav(new Vector3(attackMasses [massId].x + Random.Range(-5800, 5800) ,0, 
				attackMasses [massId].z + Random.Range(6000, 9000)));
		} else {
			startNav(new Vector3(attackMasses [massId].x + Random.Range(-10000, 10000) ,0, 
				attackMasses [massId].z + Random.Range(-10000, 10000)));
		}
	}

	void toAttck(){
		//Debug.Log (playerId + "; gqb------>toAttck1111: " + UImanager.attackAreaId_1);
		if (currentMountMissle <= 0) {
			startNav (mPatrolArea.getRandomPoint ());
		} else {
			int attackId;
			if ("0".Equals (playerId)) {
				attackId = 5 + UImanager.attackAreaId_0;
			} else {
				attackId = UImanager.attackAreaId_1;
			}
			//Debug.Log (playerId + "; gqb------>toAttck2222: " + attackId + ";dpoint: " + nav.destination);
			startNav(new RadiusArea(attackId).getRandomPoint(), maxMoveSpeed * 0.6f);
		}
	}

	void updatePartial(){
		nav.enabled = true;
		isNavPathPartial = false;
	}

	void isTargetInFire(){
		if(target == null){
			isInFire = false;
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
		float fireMissileDis = 20000;
		if("0".Equals(playerId) && behavior == 1){
			//Debug.Log (playerId + "gqb------>fireMissileDis: " + fireMissileDis );
			fireMissileDis = 12000;
		}
		RaycastHit hit;
		if(dx < 30 && dy < 45 && distance < fireMissileDis 
			&& Physics.Raycast (transform.position, (target.position - transform.position).normalized, out hit, 21000.0f)){
			if(!hit.transform.root.tag.Equals("Untagged") || (hit.transform.root.childCount > 0 && !hit.transform.root.GetChild(0).tag.Equals("Untagged"))){
				fireMissile ();
			}
		}

		if(dx < 20 && dy < 25 && distance < 6000){
			isInFire = true;
		}else {
			isInFire = false;
		}

		//Debug.Log ("gqb------>d angle x: " + dx + "; d angle y: " + dy + "; distance: " + distance);
		//
	}

	void fire(){
		if(isDestoryed){
			return;	
		}
		if(!"A10a".Equals (airType)) {
			return;
		}
		if(fireIndex > 7){
			fireIndex = 0;
		}
		if(fires[fireIndex] == null){
			return;
		}

		GameObject fireEffect = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bulletFire"), 
			fires[fireIndex].position , fires[fireIndex].rotation ) as GameObject;
		fireEffect.tag = "bulletFire";
		fireEffect.transform.parent = transform;

		GameObject bullet = (GameObject)Instantiate(Resources.Load("Prefabs/arms/bullet"),
			fires[fireIndex].position  + fires[fireIndex].forward * 0.5f, fires[fireIndex].rotation ) as GameObject;
		bullet.tag = "bullet";
		Bullet mBullet = bullet.GetComponent<Bullet> ();
		mBullet.shoot (planMove.speed);

		fireIndex = fireIndex + 1;
	}

	public override void fireOfPlayer(){
		fire ();
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
					startSpeed = planMove.player.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
				}
				PojulObject mPojulObject = target.parent.gameObject.GetComponent<PojulObject> ();
				mMissileType3.fireInit ((playerId + "_missile3" ), target, startSpeed * 1.1f);
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
				if(transform.gameObject.GetComponent<Rigidbody>()){
					startSpeed = transform.gameObject.GetComponent<Rigidbody> ().velocity.magnitude;
				}
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
		nav.destination = new Vector3(navPoint.x, planeHeight, navPoint.z);//target.transform.position;
		float patrol = Random.Range(maxMoveSpeed*0.6f, maxMoveSpeed*0.8f);
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
		nav.destination = new Vector3(navPoint.x, planeHeight, navPoint.z);//target.transform.position;
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

		Collider[] colliders = Physics.OverlapSphere (transform.position, 22000);

		float dangeroustDis = 100000.0f;
		Transform dangeroustEnemy = null;
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders [i].transform.root.childCount <= 0) {
				continue;
			}
			Transform tempTransform = colliders [i].transform.root.GetChild (0);
			string tag = tempTransform.tag;
			if(tag.Equals("Untagged")){
				if (colliders [i].transform.root == null) {
					continue;
				}
				if(colliders [i].transform.root.tag.Equals("Untagged")){
					continue;
				}
				tempTransform = colliders [i].transform.root;
			}
			tag = tempTransform.tag;
			//Debug.Log ("gqb------>findInvade tag: " + tag);
			string[] strs = tempTransform.tag.Split ('_');
			if (strs.Length == 2) {
				string tempPlayerId = strs [0];
				string tempType = strs [1];
				float tempDis = (transform.position - tempTransform.position).magnitude;
				if (enemyId.Equals (tempPlayerId)) {
					if (tempDis < dangeroustDis && ("littlecannon1".Equals (tempType) || "car2".Equals (tempType) || "car3".Equals (tempType)
						|| "a10".Equals (tempType) || "car5".Equals (tempType)) 
						&& tempTransform.GetComponent<PojulObject>() && !tempTransform.GetComponent<PojulObject>().isDestoryed) {
						dangeroustEnemy = tempTransform;
						dangeroustDis = tempDis;
						//target = tempTransform.FindChild ("aim");

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
		if(dangeroustEnemy != null && dangeroustEnemy.FindChild ("aim") != null){
			target = dangeroustEnemy.FindChild ("aim");
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

		if(playerId.Equals("1") && planMove.player != null){
			PojulObject mPojulObject = planMove.player.GetComponent<PojulObject> ();
			if(mPojulObject != null && !mPojulObject.isDestoryed && mPojulObject.type.Equals("a10") && Util.isOnNavArea2(planMove.player.position)){
				if(planMove.player.FindChild("aim") != null){
					target = planMove.player.FindChild("aim");
					return;
				}
			}
		}

		List<Transform> allNearEnemys = null;
		if("0".Equals(playerId)){
			allNearEnemys = GameInit.attackArms_1;
		}else if("1".Equals(playerId)){
			allNearEnemys = GameInit.attackArms_0;
		}
		float dangeroustDis = 1000000.0f;
		Transform dangeroustEnemy = null;
		for(int i=0; i<allNearEnemys.Count;i++){
			Transform tempDangeroustEnemy = allNearEnemys[i];
			if(tempDangeroustEnemy == null || !Util.isOnMyNavArea1(tempDangeroustEnemy.position, playerId) || 
				!tempDangeroustEnemy.GetComponent<PojulObject>() || tempDangeroustEnemy.GetComponent<PojulObject>().isDestoryed){
				continue;
			}
			//Debug.Log (tempDangeroustEnemy.tag + "; gqb------>isOnEnemyNavArea1: " + Util.isOnEnemyNavArea1(tempDangeroustEnemy.position, playerId));
			float tempDis = (myCenter - tempDangeroustEnemy.position).magnitude;
			if(tempDis < dangeroustDis){
				dangeroustEnemy = tempDangeroustEnemy;
				dangeroustDis = tempDis;
			}
		}
		if(dangeroustEnemy != null && dangeroustEnemy.FindChild("aim") != null){
			target = dangeroustEnemy.FindChild("aim");
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
			if (behavior == 1) {
				startNav (mPatrolArea.getRandomPoint (), maxMoveSpeed * 0.8f);
			} else if (behavior == 3) {
				if(currentMountMissle <= 0){
					startNav (mPatrolArea.getRandomPoint (), maxMoveSpeed * 0.8f);
				}else{
					int attackId;
					if ("0".Equals (playerId)) {
						attackId = 5 + UImanager.attackAreaId_0;
					} else {
						attackId = UImanager.attackAreaId_1;
					}
					startNav (new RadiusArea (attackId).getRandomPoint (), maxMoveSpeed * 0.8f);
				}
			} else if (behavior == 2) {
				int massId;
				if ("0".Equals (playerId)) {
					massId = UImanager.massId_0;
				} else {
					massId = UImanager.massId_1;
				}
				startNav(new Vector3(attackMasses [massId].x + Random.Range(-10000, 10000) ,0, 
					attackMasses [massId].z + Random.Range(-10000, 10000)));
			}
			return;
		}

		//updateCurrentMountMissle ();

		if(target != null && target.GetComponent<PojulObject>() && !target.GetComponent<PojulObject>().isDestoryed && currentMountMissle > 0){
			if (behavior == 1) {
				startNav (new Vector3 (target.position.x, planeHeight, target.position.z), maxMoveSpeed * 0.6f);
				height = target.position.y;
			} else if (behavior == 3) {
				startNav (new Vector3 (target.position.x, planeHeight, target.position.z), maxMoveSpeed * 0.6f);
				//startNav (new RadiusArea(attackId).getRandomPoint(), GameInit.mach*0.8f);
				height = target.position.y;
			} else if (behavior == 2) {
				if (Util.isOnMyNavArea1 (target.position, playerId)) {
					startNav (new Vector3 (target.position.x, planeHeight, target.position.z), maxMoveSpeed * 0.6f);
				} else {
					startNav (mPatrolArea.getRandomPoint (), maxMoveSpeed * 0.6f);
					target = null;
				}

			}
		}else if(missileCar != null){
			startNav (new Vector3((missileCar.position.x + Random.Range(100,300)), planeHeight, (missileCar.position.z + Random.Range(100,300))), maxMoveSpeed*0.6f);
			height = missileCar.position.y;
		}
		if (height < 1000){
			height = 1000;
		}else if(height > 12000){
			height = 12000;
		}

	}

	void updateCurrentMountMissle(){
		if(isDestoryed){
			return;
		}
		int temptMountMissle = 0;
		for (int i = 0; i < missilePoses.Count; i++) {
			if(missiles [missilePoses[i]] != null){
				temptMountMissle = temptMountMissle + 1;
			}
		}
		currentMountMissle = temptMountMissle;
	}

	public override void isFired(RaycastHit hit, Collision collision, int type){
		//return;
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
			inflame(72000);
			return;
		}else if (type == 4) {
			sliderHealth.value = sliderHealth.value - 3.8f;
			if(sliderHealth.value <= 0){
				isDestoryed = true;
				isPanDestoryed = true;
				GameObject bomb2 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bomb2"), 
					transform.position, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
				bomb2.tag = "bomb2";
				bomb2.transform.parent = transform;
				inflame(72000);
			}
		}
	}

	public void inflame(float force){
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
			mRigidbody.AddForce (transform.forward * force);
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
		destoryData ();

		Invoke ("destoryAll", 40);

	}

	public void destoryData(){
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
			UImanager.isOnLeave = true;
			Camera.main.GetComponent<AudioSource> ().enabled = false;
		}
	}

	public void destoryAll(){

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
		lock(GameInit.locker){
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
	}

	void findMissileCar(){
		if(playerType == 0){
			return;
		}
		if(MissileAimedTra != null && isAvoidMissile){
			return;
		}
		if(target != null && currentMountMissle > 0 && behavior != 2){
			return;
		}
		if(behavior == 3 && currentMountMissle > 0){
			return;
		}

		List<Transform> Car2s = new List<Transform> ();
		if("0".Equals(playerId)){
			Car2s = GameInit.MyCar2;
		}else if("1".Equals(playerId)){
			Car2s = GameInit.EnemyCar2;
		}
		//Debug.Log (missileCar + "::" + "; gqb------>findMissileCar Car2sCount22222: " +   Car2s.Count + "currentMountMissle: " + currentMountMissle);
		float distance = 1000000;
		Transform tempTra = null;
		for(int i =0; i< Car2s.Count; i++ ){
			if(Car2s[i] == null){
				continue;
			}
			CarType2 mCarType2 = Car2s[i].gameObject.GetComponent<CarType2> ();
			float tempDistance = (transform.position - Car2s [i].position).magnitude;
			//Debug.Log (missileCar + "::" + (Car2s[i].position - myCenter).magnitude + "; gqb------>findMissileCar Car2sCount22222: " +   Car2s.Count + "currentMountMissle: " + currentMountMissle);
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
