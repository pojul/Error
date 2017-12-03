using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class A10aPlan : PojulObject {

	public static float planeHeight = 60f;
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

	private float height = 126.2f;

	private AudioSource mAudioSource;

	private Transform transform_lod0;
	private Transform transform_lod1;
	private Transform transform_lod2;

	private Transform aimTransform;

	private float maxMoveSpeed = GameInit.mach;
	private float moveSpeed = 0.0f;
	private float rolateSpeed = 30f;
	private GameObject navCube;
	private UnityEngine.AI.NavMeshAgent nav;

	private RadiusArea mPatrolArea;

	private Transform target;

	private int playerType = 1; //0: player; 1:  prefab

	private float minHeight = 1000;
	private float maxHeight = 12000;
	private float piRate = 2* Mathf.PI/360;

	//private List<Transform> missles = new List<Transform>();
	//private List<Transform> misslesPos = new List<Transform>();
	private List<Transform> missilePoses = new List<Transform>();
	private Dictionary<Transform, Transform> missiles = new Dictionary<Transform, Transform> ();
	private int maxMountMissle = 2;
	private int currentMountMissle = 0;

	//血量条
	public Slider sliderHealth;
	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (transform.position.x, height, transform.position.z);

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



		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];

		int areaId = -1;
		if ("0".Equals (playerId)) {
			mPatrolArea = new RadiusArea (Random.Range(1,4));
			myCenter = new Vector3 (0,0,-60000);
			enemyCenter = new Vector3 (0,0,60000);
			enemyId = "1";
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.251f, 0.647f, 0.78f);
		} else if ("1".Equals (playerId)) {
			mPatrolArea = new RadiusArea (Random.Range(5,8));
			myCenter = new Vector3 (0,0,60000);
			enemyCenter = new Vector3 (0,0,-60000);
			enemyId = "0";
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.698f, 0.255f, 0.157f);
		}
		mPatrolArea.minRange = 25000;
		RectTransform mRectTransform = sliderHealth.GetComponent<RectTransform> ();
		mRectTransform.sizeDelta = new Vector2 (Screen.width / 18, Screen.width / 60);
		sliderHealth.value = sliderHealth.maxValue;

		createNavCube ();

		if(mAudioSource != null && !mAudioSource.isPlaying){
			mAudioSource.Play ();
		}

		if(playerType == 1){
			startNav(mPatrolArea.getRandomPoint());
			mAudioSource.volume =0.4f;
		}else if(playerType == 0){
			mAudioSource.volume = 0.015f;
		}

		InvokeRepeating("behaviorListener", 0.5f, 0.5f);
		InvokeRepeating ("findInvade", 2.0f, 2.0f);
		InvokeRepeating ("interceptInvade", 5.0f, 5.0f);

		addMissile ();

	}

	void addMissile(){
		for(int i = 0; i< missilePoses.Count; i++){
			GameObject prefab = (GameObject)Instantiate(Resources.Load((string)GameInit.modelpaths["missile3"]), 
				missilePoses[i].position, Quaternion.Euler(0, 0, 0)) as GameObject;
			missiles [missilePoses[i]] = prefab.transform;
			prefab.transform.parent = transform;
			currentMountMissle = currentMountMissle + 1;
		}
	}

	void Update () {
		if(playerType == 1){
			airMoveAuto ();
		}else if(playerType == 0){
			//Debug.Log ("gqb------>volume: " + mAudioSource.volume);
		}

		sliderHealth.transform.rotation = Quaternion.Euler(transform_lod0.rotation.eulerAngles.x , 
			Camera.main.transform.rotation.eulerAngles.y,
			transform_lod0.rotation.eulerAngles.z);
	}

	void airMoveAuto(){
		float dRolX = transform.rotation.eulerAngles.x;

		if(transform.position.y > 12000){
			height = 12000;
		}else if(transform.position.y < 1000){
			height = 1000;
		}

		//if(target == null){
			if( (myCenter - transform.position).magnitude > 9000 && (Mathf.Abs((transform.position.y - height)) > 200) ){
				if(nav.hasPath && !nav.pathPending){
					//Debug.Log ("gqb------>x: " + nav.pathEndPosition.x +"; y: " + nav.pathEndPosition.y + "; z: " + nav.pathEndPosition.z);
					dRolX = Quaternion.Slerp(transform.rotation, 
						Quaternion.LookRotation(new Vector3(nav.pathEndPosition.x,height,nav.pathEndPosition.z) - transform.position), 5 * Time.deltaTime)
						.eulerAngles.x;
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

	public void setPlayType(int playerType){
		this.playerType = playerType;
		if(playerType == 0){
			Camera.main.transform.position = new Vector3((transform.position.x + cameraX), 
				(transform.position.y + cameraY), 
				(transform.position.z+ cameraZ));
			Camera.main.transform.parent = transform;
			if(nav != null){
				nav.enabled = false;
			}
			if(mAudioSource != null){
				mAudioSource.volume = 0.15f;
			}
		}else if(playerType == 1){
			
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
		if(playerType == 0){
			//isTargetInFire ();
			return;
		}

		isTargetInFire ();

		if(nav != null && behavior == 1  && 
			(nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid || nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial) ){
			startNav(mPatrolArea.getRandomPoint());
		}

		if(nav != null && behavior == 1 && !nav.hasPath && !nav.pathPending && mPatrolArea != null && target == null){
			startNav(mPatrolArea.getRandomPoint());
		}else if(behavior == 3 && nav.destination != enemyCenter){
		}
	}

	void isTargetInFire(){
		if(target == null){
			return;
		}
		PojulObject mPojulObject = target.parent.gameObject.GetComponent<PojulObject> ();
		if(mPojulObject == null || mPojulObject.isMissileAimed){
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

		if(dx < 30 && dy < 45 && distance < 20000){
			fireMissile ();
		}
		Debug.Log ("gqb------>d angle x: " + Mathf.Min(dx1, dx2) + "; d angle y: " + Mathf.Min(dy1, dy2) + "; distance: " + distance);
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
				mMissileType3.fireInit ((playerId + "_missile3" ), target, startSpeed);
				missiles [missilePoses [i]] = null;
				currentMountMissle = currentMountMissle  - 1;
				PojulObject mPojulObject = target.parent.gameObject.GetComponent<PojulObject> ();
				if(mPojulObject != null){
					mPojulObject.isMissileAimed = true;
				}
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
		nav.acceleration = moveSpeed * 0.8f;
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
		nav.acceleration = moveSpeed * 0.8f;
		nav.autoRepath = true;
		nav.angularSpeed = rolateSpeed;
	}

	public void createNavCube(){
		navCube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		navCube.transform.position = new Vector3(transform.position.x, planeHeight, transform.position.z);
		navCube.transform.localScale = new Vector3 (100, 100, 100);
		navCube.AddComponent<UnityEngine.AI.NavMeshAgent>();
		Destroy(navCube.GetComponent<BoxCollider> ());
		//transform.parent = navCube.transform;

		MeshRenderer m = navCube.GetComponent<MeshRenderer>();
		m.enabled = false;
		nav= navCube.GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	void findInvade(){
		if(playerType ==0){
			//return;
		}
		Collider[] colliders = Physics.OverlapSphere (transform.position, 25000);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders [i].transform.root.childCount <= 0) {
				continue;
			}
			Transform tempTransform = colliders [i].transform.root.GetChild (0);
			string tag = tempTransform.tag;
			string[] strs = tempTransform.tag.Split ('_');
			if (strs.Length == 2) {
				string tempPlayerId = strs [0];
				string tempType = strs [1];
				if (enemyId.Equals (tempPlayerId)) {
					if (target == null && ("car2".Equals (tempType) || "car3".Equals (tempType)
					   || "a10".Equals (tempType) || "car5".Equals (tempType))) {
						target = tempTransform.FindChild ("aim");
					}
					Util.AddNearEnemys (tempTransform, playerId);
				}
			}
		}
		if (target == null) {
			findDangeroust ();
		}
	}

	void findDangeroust(){
		if(playerType ==0){
			return;
		}
		Dictionary<int, List<Transform>> nearEnemys = null;
		if("0".Equals(playerId)){
			nearEnemys = GameInit.nearEnemys_0;
		}else if("1".Equals(playerId)){
			nearEnemys = GameInit.nearEnemys_1;
		}
		if(nearEnemys.ContainsKey(mPatrolArea.areaId) && nearEnemys [mPatrolArea.areaId].Count > 0){
			List<Transform> tempTransforms = nearEnemys [mPatrolArea.areaId];
			if (tempTransforms [0] == null) {
				tempTransforms.Remove (tempTransforms [0]);
				return;
			}
			Transform dangeroustEnemy = tempTransforms [0];
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
			}
			if(dangeroustEnemy != null && dangeroustEnemy.FindChild("aim") != null){
				target = dangeroustEnemy.FindChild("aim");
			}
		}
	}


	void interceptInvade(){
		if(playerType ==0){
			return;
		}

		if(behavior == 1 && target != null){
			startNav (new Vector3(target.position.x, planeHeight, target.position.z), maxMoveSpeed*0.8f);
			height = target.position.y;
		}
	}


}
