using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarType5 : PojulObject {

	private AudioSource mAudioSource;

	private Transform transform_lod0;
	private Transform transform_lod1;
	private Transform transform_lod2;

	private Transform[] lunzis = new Transform[6];
	private Transform aimTransform;

	private Transform mainTransform_lod0;
	private Transform mainTransform_lod1;
	private Transform mainTransform_lod2;

	private Animator mAnimator_lod0;
	private Animator mAnimator_lod1;

	private Renderer mRenderer_lod0_lunzi1;
	private Renderer mRenderer_lod1_lunzi1;

	private Transform paoTransform_lod0;
	private Transform paoTransform_lod1;
	private Transform paoTransform_lod2;

	private bool isMoving = false;
	public bool readyLaunch = false;

	private float aimSpeed = 4.8f;
	private float height = 38f; //69.0f;
	private float maxMoveSpeed = GameInit.mach * 0.36f;

	public GameObject navCube;
	public UnityEngine.AI.NavMeshAgent nav;

	public RadiusArea mPatrolArea;

	private Transform target;

	private List<Transform> missilePoses = new List<Transform>();
	private Dictionary<Transform, Transform> missiles = new Dictionary<Transform, Transform> ();
	public int maxMountMissle = 3;
	public int currentMountMissle = 0;
	public int priority = 1;
	public bool isSupplied = false;

	//血量条
	public Slider sliderHealth;
	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (transform.position.x, height, transform.position.z);

		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		transform_lod0 = transform.Find ("car_type5_lod0");
		transform_lod1 = transform.Find ("car_type5_lod1");
		transform_lod2 = transform.Find ("car_type5_lod2");


		aimTransform = transform.Find ("aim");

		mainTransform_lod0 = transform_lod0.Find("main");
		mainTransform_lod1 = transform_lod1.Find("main");
		mainTransform_lod2 = transform_lod2.Find("main");

		//Debug.Log (mainTransform_lod0 + "gqb------>" + mainTransform_lod1);

		mAnimator_lod0 = (Animator)transform_lod0.GetComponent<Animator> ();
		mAnimator_lod1 = (Animator)transform_lod1.GetComponent<Animator> ();

		mRenderer_lod0_lunzi1 = transform_lod0.Find("lunzi1").GetComponent<Renderer>();
		mRenderer_lod1_lunzi1 = transform_lod1.Find("lunzi1").GetComponent<Renderer>();

		paoTransform_lod0 = transform_lod0.Find ("pao");
		paoTransform_lod1 = transform_lod1.Find ("pao");
		paoTransform_lod2 = transform_lod2.Find ("pao");

		missilePoses.Add (paoTransform_lod0.Find ("missilePos1"));
		missilePoses.Add (paoTransform_lod0.Find ("missilePos2"));
		missilePoses.Add (paoTransform_lod0.Find ("missilePos3"));
		missilePoses.Add (paoTransform_lod0.Find ("missilePos4"));

		missiles.Add (missilePoses[0], null);
		missiles.Add (missilePoses[1], null);
		missiles.Add (missilePoses[2], null);
		missiles.Add (missilePoses[3], null);

		run ();

		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];

		int areaId = -1;
		if ("0".Equals (playerId)) {
			areaId = Util.getCar5AreaId ("0");
			if(!GameInit.Car5Area0.ContainsKey (areaId)){
				GameInit.Car5Area0.Add (areaId, this.gameObject);
			}
			myCenter = new Vector3 (0,0,-60000);
			enemyCenter = new Vector3 (0,0,60000);
			enemyId = "1";
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.251f, 0.647f, 0.78f);
		} else if ("1".Equals (playerId)) {
			areaId = Util.getCar5AreaId ("1");
			if(!GameInit.Car5Area1.ContainsKey (areaId)){
				GameInit.Car5Area1.Add (areaId, this.gameObject);
			}
			myCenter = new Vector3 (0,0,60000);
			enemyCenter = new Vector3 (0,0,-60000);
			enemyId = "0";
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.698f, 0.255f, 0.157f);
		}

		RectTransform mRectTransform = sliderHealth.GetComponent<RectTransform> ();
		mRectTransform.sizeDelta = new Vector2 (Screen.width / 18, Screen.width / 60);
		sliderHealth.value = sliderHealth.maxValue;

		mPatrolArea = new RadiusArea (areaId);
		mPatrolArea.maxRange = 40000;
		mPatrolArea.minRange = 20000;
		startNav(mPatrolArea.getRandomPoint());

		//addMissile ();
		currentMountMissle = 0;
		InvokeRepeating("behaviorListener", 0.5f, 0.5f);
		InvokeRepeating ("findInvade", 1.5f, 1.5f);

	}

	public void addMissile(int num){
		int addnum = 0;
		for(int i = 0; i< missilePoses.Count; i++){
			if(addnum >= num){
				return;
			}
			if(missiles [missilePoses[i]] == null){
				GameObject prefab = (GameObject)Instantiate(Resources.Load((string)GameInit.modelpaths["missile1"]), 
					missilePoses[i].position, missilePoses[i].rotation) as GameObject;
				missiles [missilePoses[i]] = prefab.transform;
				prefab.transform.parent = paoTransform_lod0;
				prefab.GetComponent<MeshRenderer> ().enabled = false;
				currentMountMissle = currentMountMissle + 1;
				addnum = addnum + 1;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		health = sliderHealth.value;
		if(isDestoryed){
			return;
		}
		sliderHealth.transform.rotation = Quaternion.Euler(mainTransform_lod0.rotation.eulerAngles.x , 
			Camera.main.transform.rotation.eulerAngles.y,
			mainTransform_lod0.rotation.eulerAngles.z);

		if (readyLaunch) {
			radyLaunch ();
		} else {
			unRadyLaunch ();
		}

		if(isMoving){
			listenerRollAni ();
		}
	}

	void behaviorListener(){
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

		if(nav != null && behavior == 1  && 
			(nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid || nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial) ){
			startNav(mPatrolArea.getRandomPoint());
			readyLaunch = true;
		}

		if(nav != null && nav.hasPath){
			readyLaunch = false;
		}

		if(nav != null && !nav.hasPath){
			readyLaunch = true;
		}

		isTargetInFire ();
	}

	void isTargetInFire(){
		if(target == null || isDestoryed){
			return;
		}
		PojulObject mPojulObject = target.parent.gameObject.GetComponent<PojulObject> ();
		if(mPojulObject == null || mPojulObject.isMissileAimed || mPojulObject.isDestoryed){
			return;
		}

		if (paoTransform_lod0.localEulerAngles.x > 275 || paoTransform_lod0.localEulerAngles.x < 265) {
			return;
		}
		float distance = (transform.position - target.position).magnitude;

		//Debug.DrawRay(transform.position, (target.position - transform.position).normalized *16000, Color.white);
		RaycastHit hit;
		if((target.position.y - transform.position.y) > 0 && distance < 32000
			&& Physics.Raycast (transform.position, (target.position - transform.position).normalized, out hit, 35000.0f)){
			if(!hit.transform.root.tag.Equals("Untagged") || (hit.transform.root.childCount > 0 && !hit.transform.root.GetChild(0).tag.Equals("Untagged"))){
				fireMissile ();
			}
		}
		//Debug.Log ("gqb------>distance: " + distance);
	}

	void fireMissile(){
		if(isDestoryed){
			return;
		}
		if(currentMountMissle <= 0){
			return;
		}
		for (int i = 0; i < missilePoses.Count; i++) {
			if(missiles [missilePoses[i]] != null){
				missiles [missilePoses [i]].gameObject.GetComponent<MeshRenderer> ().enabled = true;
				MissileType1 mMissileType1 = missiles [missilePoses [i]].gameObject.GetComponent<MissileType1> ();
				float startSpeed = 0;
				PojulObject mPojulObject = target.parent.gameObject.GetComponent<PojulObject> ();
				mMissileType1.fireInit ((playerId + "_missile1" ), target, startSpeed);
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

	void radyLaunch(){
		if(isDestoryed){
			return;
		}
		if(paoTransform_lod0.localEulerAngles.x > 271 || paoTransform_lod0.localEulerAngles.x < 269){
			paoTransform_lod0.localEulerAngles = new Vector3((paoTransform_lod0.localEulerAngles.x - aimSpeed * Time.deltaTime), 
				paoTransform_lod0.localEulerAngles.y, paoTransform_lod0.localEulerAngles.z);
			paoTransform_lod1.localEulerAngles = new Vector3(paoTransform_lod0.localEulerAngles.x, paoTransform_lod0.localEulerAngles.y, paoTransform_lod0.localEulerAngles.z);
			paoTransform_lod2.localEulerAngles = new Vector3(paoTransform_lod0.localEulerAngles.x, paoTransform_lod0.localEulerAngles.y, paoTransform_lod0.localEulerAngles.z);
		}

		//if(paoTransform_lod0.localEulerAngles.x <= 271 ){
			//readyLaunch = false;
		//}

	}

	void unRadyLaunch(){
		if(isDestoryed){
			return;
		}
		if(paoTransform_lod0.localEulerAngles.x > 1 && paoTransform_lod0.localEulerAngles.x < 359){
			paoTransform_lod0.localEulerAngles = new Vector3((paoTransform_lod0.localEulerAngles.x + aimSpeed * Time.deltaTime), 
				paoTransform_lod0.localEulerAngles.y, paoTransform_lod0.localEulerAngles.z);
			paoTransform_lod1.localEulerAngles = new Vector3(paoTransform_lod0.localEulerAngles.x, paoTransform_lod0.localEulerAngles.y, paoTransform_lod0.localEulerAngles.z);
			paoTransform_lod2.localEulerAngles = new Vector3(paoTransform_lod0.localEulerAngles.x, paoTransform_lod0.localEulerAngles.y, paoTransform_lod0.localEulerAngles.z);
		}
	}

	void listenerRollAni(){
		if(isDestoryed){
			return;
		}
		if (mRenderer_lod0_lunzi1.isVisible && !mAnimator_lod0.GetBool ("roll")) {
			mAnimator_lod0.SetBool ("roll", true);
			mAnimator_lod1.SetBool ("roll", false);
		} else if (mRenderer_lod1_lunzi1.isVisible && !mAnimator_lod1.GetBool ("roll")) {
			mAnimator_lod1.SetBool ("roll", true);
			mAnimator_lod0.SetBool ("roll", false);
		} else if(!mRenderer_lod0_lunzi1.isVisible && !mRenderer_lod1_lunzi1.isVisible){
			mAnimator_lod0.SetBool ("roll", false);
			mAnimator_lod1.SetBool ("roll", false);
		}
		if(nav != null && !nav.hasPath && !nav.pathPending){
			stop ();
			readyLaunch = true;
		}
	}

	void run (){
		isMoving = true;
		if(mAudioSource != null && !mAudioSource.isPlaying){
			mAudioSource.Play ();
		}
	}

	void stop(){
		isMoving = false;
		mAnimator_lod0.SetBool ("roll", false);
		mAnimator_lod1.SetBool ("roll", false);
		if(mAudioSource != null && mAudioSource.isPlaying){
			mAudioSource.Stop ();
		}
	}

	public void startNav(Vector3 navPoint){
		if(navCube == null){
			createNavCube ();
		}
		if(!nav.enabled){
			return;
		}
		nav.destination = new Vector3(navPoint.x, 5, navPoint.z);//target.transform.position;
		float patrol = Random.Range(maxMoveSpeed*0.5f, maxMoveSpeed);
		nav.speed = patrol;
		nav.acceleration = patrol * 2f;
		nav.autoRepath = true;
		//nav.baseOffset = 50;
		nav.angularSpeed = 100;
		readyLaunch = false;
		run ();
	}

	public void startNav(Vector3 navPoint, float speed){
		if(navCube == null){
			createNavCube ();
		}
		if(!nav.enabled){
			return;
		}
		nav.destination = new Vector3(navPoint.x, 5, navPoint.z);//target.transform.position;
		nav.speed = speed;
		nav.acceleration = speed * 2;
		nav.autoRepath = true;
		nav.angularSpeed = 100;
		readyLaunch = false;
		run ();
	}

	public void createNavCube(){
		navCube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		navCube.transform.position = new Vector3(transform.position.x, 5, transform.position.z);
		navCube.transform.localScale = new Vector3 (10, 10, 10);
		navCube.AddComponent<UnityEngine.AI.NavMeshAgent>();
		Destroy(navCube.GetComponent<BoxCollider> ());
		transform.parent = navCube.transform;

		MeshRenderer m = navCube.GetComponent<MeshRenderer>();
		m.enabled = false;
		nav= navCube.GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	public override void isFired(RaycastHit hit, Collision collision, int type){
		if(isDestoryed){
			return;
		}
		if(type == 2){
			Vector3 hitPoint;
			if (collision != null) {
				hitPoint = collision.contacts[0].point;
			} else {
				hitPoint = hit.point;
			}
			sliderHealth.value = sliderHealth.value - 58;
			if(sliderHealth.value <= 0){
				isDestoryed = true;
				DestoryAll (hitPoint, 120000.0f);
				destoryData ();
				return;
			}
		}else if(type == 3){
			isDestoryed = true;
			isPanDestoryed = true;
			DestoryAll (collision.contacts[0].point, 120000.0f);
			destoryData ();
			return;
		}else if (type == 4) {
			sliderHealth.value = sliderHealth.value - 2.6f;
			if(sliderHealth.value <= 0){
				isDestoryed = true;
				isPanDestoryed = true;

				GameObject bomb2 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bomb2"), 
					transform.position, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
				bomb2.tag = "bomb2";
				bomb2.transform.parent = transform;

				DestoryAll (transform.position, 10000.0f);
				destoryData ();
				return;
			}
		}
	}

	void DestoryAll(Vector3 point, float power){
		mainTransform_lod0.parent = null;
		mainTransform_lod1.parent = mainTransform_lod0;
		mainTransform_lod2.parent = mainTransform_lod0;
		if(!mainTransform_lod0.gameObject.GetComponent<Rigidbody>()){
			mainTransform_lod0.gameObject.AddComponent<Rigidbody> ();
			mainTransform_lod0.gameObject.GetComponent<Rigidbody> ().mass = 1.0f;
		}

		paoTransform_lod0.parent = null;
		paoTransform_lod1.parent = paoTransform_lod0;
		paoTransform_lod2.parent = paoTransform_lod0;
		if(!paoTransform_lod0.gameObject.GetComponent<Rigidbody>()){
			paoTransform_lod0.gameObject.AddComponent<Rigidbody> ();
			paoTransform_lod0.gameObject.GetComponent<Rigidbody> ().mass = 0.9f;
		}
		for(int i =0;i <= 5; i++){
			Transform lunzi_lod0 = transform_lod0.Find (("lunzi" + (i + 1).ToString()));
			Transform lunzi_lod1 = transform_lod1.Find (("lunzi" + (i + 1).ToString()));
			Transform lunzi_lod2 = transform_lod2.Find (("lunzi" + (i + 1).ToString()));
			lunzi_lod0.parent = null;
			lunzi_lod1.parent = lunzi_lod0;
			lunzi_lod2.parent = lunzi_lod0;
			lunzis [i] = lunzi_lod0;
			if(!lunzi_lod0.gameObject.GetComponent<MeshCollider>()){
				lunzi_lod0.gameObject.AddComponent<MeshCollider> ();
				lunzi_lod0.gameObject.GetComponent<MeshCollider> ().convex = true;
				lunzi_lod0.gameObject.AddComponent<Rigidbody> ();
				lunzi_lod0.gameObject.GetComponent<Rigidbody> ().mass = 0.8f;
			}
		}

		Vector3 explosionPos = new Vector3 (point.x, 
			(point.y - 150), 
			point.z);
		Collider[] colliders = Physics.OverlapSphere(explosionPos, 200.0f);
		foreach (Collider hit in colliders){
			if (hit.GetComponent<Rigidbody>()){
				hit.GetComponent<Rigidbody>().AddExplosionForce(power, explosionPos, 200.0f);
			}  
		}
		Invoke ("destoryAll", 60);
		if(nav != null){
			nav.enabled = false;	
		}
		stop ();
		Transform aim = transform.Find ("aim");
		if(aim != null){
			Destroy (aim.gameObject);
		}
	}

	public void destoryData(){
		GameInit.currentInstance [(playerId + "_missile1")] = GameInit.currentInstance [(playerId + "_missile1")] - currentMountMissle;
		currentMountMissle = 0;
		if("0".Equals(playerId)){
			if (GameInit.Car5Area0.ContainsValue(this.gameObject)) {
				GameInit.Car5Area0.Remove (mPatrolArea.areaId);
			}
		}else if("1".Equals(playerId)){
			if (GameInit.Car5Area1.ContainsValue(this.gameObject)) {
				GameInit.Car5Area1.Remove (mPatrolArea.areaId);
			}
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
	}

	public void destoryAll(){

		if(mainTransform_lod0 != null){
			Destroy (mainTransform_lod0.gameObject);
		}
		if(paoTransform_lod0 != null){
			Destroy (paoTransform_lod0.gameObject);
		}
		for(int i=0;i<= 5;i++){
			if(lunzis[i] != null){
				Destroy(lunzis[i].gameObject);
			}
		}

		Destroy(transform.root.gameObject);
	}

	void findInvade(){
		updateCurrentMountMissle ();

		Collider[] colliders = Physics.OverlapSphere (transform.position, 33000);
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
			//Debug.Log ("gqb------>findInvade1 tag: " + tag);
			string[] strs = tempTransform.tag.Split ('_');
			if (strs.Length == 2) {
				string tempPlayerId = strs [0];
				string tempType = strs [1];
				if (enemyId.Equals (tempPlayerId)) {
					if (target == null && ("a10".Equals (tempType))) {
						target = tempTransform.Find ("aim");
					}
					Util.AddNearEnemys (tempTransform, playerId);
				}
			}
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

	public override int getSellGold(){
		if(isDestoryed){
			return 0;
		}
		float gold1 = (GameInit.prices ["car5"] * 0.5f *sliderHealth.value / sliderHealth.maxValue);
		float gold2 = 0.5f * currentMountMissle * GameInit.prices ["missile1"] ;
		int gold = (int)(gold1 + gold2);
		return gold;
	}

}
