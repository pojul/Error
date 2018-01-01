using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarType2 : PojulObject {

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
	private Animator mAnimator_lod2;

	private Renderer mRenderer_lod0_lunzi1;
	private Renderer mRenderer_lod1_lunzi1;

	private bool isMoving = false;

	private float height = 46; //68.0f;
	private float maxMoveSpeed = GameInit.mach * 0.6f;
	private Vector3 park;
	public GameObject navCube;
	public UnityEngine.AI.NavMeshAgent nav;

	public int[] maxMissiles = {4, 3, 5};

	public int[] currentMissiles = {0, 0, 0};

	private GameObject supplyCar5;

	public bool isGetMissileRoad = false;

	//血量条
	public Slider sliderHealth;
	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (transform.position.x, height, transform.position.z);

		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		transform_lod0 = transform.FindChild ("car_type2_lod0");
		transform_lod1 = transform.FindChild ("car_type2_lod1");
		transform_lod2 = transform.FindChild ("car_type2_lod2");

		aimTransform = transform.FindChild ("aim");

		mainTransform_lod0 = transform_lod0.FindChild("main");
		mainTransform_lod1 = transform_lod1.FindChild("main");
		mainTransform_lod2 = transform_lod2.FindChild("main");

		mAnimator_lod0 = (Animator)transform_lod0.GetComponent<Animator> ();
		mAnimator_lod1 = (Animator)transform_lod1.GetComponent<Animator> ();
		mAnimator_lod2 = (Animator)transform_lod2.GetComponent<Animator> ();

		mRenderer_lod0_lunzi1 = transform_lod0.FindChild("lunzi1").GetComponent<Renderer>();
		mRenderer_lod1_lunzi1 = transform_lod1.FindChild("lunzi1").GetComponent<Renderer>();

		run ();

		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];

		if ("0".Equals (playerId)) {
			GameInit.MyCar2.Add (transform);
			park = Util.getIdlePart ("0");
			GameInit.park0 [park] = 1;
			myCenter = new Vector3 (0,0,-60000);
			enemyCenter = new Vector3 (0,0,60000);
			enemyId = "1";
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.251f, 0.647f, 0.78f);
		}else if ("1".Equals (playerId)){
			GameInit.EnemyCar2.Add (transform);
			park = Util.getIdlePart ("1");
			GameInit.park1 [park] = 1;
			myCenter = new Vector3 (0,0,60000);
			enemyCenter = new Vector3 (0,0,-60000);
			enemyId = "0";
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.698f, 0.255f, 0.157f);
		}

		RectTransform mRectTransform = sliderHealth.GetComponent<RectTransform> ();
		mRectTransform.sizeDelta = new Vector2 (Screen.width / 18, Screen.width / 60);
		sliderHealth.value = sliderHealth.maxValue;


		startNav (park);
	
		//InvokeRepeating("behaviorListener", 1f, 1f);
		InvokeRepeating("missileTransport", 2f, 2f);

	}
		
	// Update is called once per frame
	void Update () {
		sliderHealth.transform.rotation = Quaternion.Euler(mainTransform_lod0.rotation.eulerAngles.x , 
			Camera.main.transform.rotation.eulerAngles.y,
			mainTransform_lod0.rotation.eulerAngles.z);

		if(isMoving){
			listenerRollAni ();
		}
	}

	void behaviorListener(){

	}

	void missileTransport(){
		if(supplyCar5 != null){
			startNav (new Vector3(supplyCar5.transform.position.x, 0 ,supplyCar5.transform.position.z));
			isGetMissileRoad = false;
			CarType5 mCarType5 = supplyCar5.GetComponent<CarType5> ();
			if((supplyCar5.transform.position - transform.position).magnitude < 800){
				exchange (mCarType5);
			}
			return;
		}
		if(currentMissiles[0] > 0){
			findNeedMissile1 ();
		}

		getMissile ();
	}

	void getMissile(){

		if ((GameInit.remainMissile [(playerId + "_missile1")] > 0 && (currentMissiles[0] < maxMissiles[0]))
			|| (GameInit.remainMissile [(playerId + "_missile2")] > 0 && (currentMissiles[1] < maxMissiles[1]))
			|| (GameInit.remainMissile [(playerId + "_missile3")] > 0 && (currentMissiles[2] < maxMissiles[2]))) {

			if((transform.position - myCenter).magnitude < 800 ){
				exchangeRemainMissile ((playerId + "_missile1"), 0);
				exchangeRemainMissile ((playerId + "_missile2"), 1);
				exchangeRemainMissile ((playerId + "_missile3"), 2);
			}
			if(isGetMissileRoad){
				return;
			}
			Vector3 centerPoint = new Vector3 ((myCenter.x + Random.Range (200, 300)), 0, 
				                      (myCenter.z + Random.Range (200, 300)));
			startNav (centerPoint);
			isGetMissileRoad = true;
		} else {
			if(nav.destination != park){
				isGetMissileRoad = false;
				startNav (park);
			}
		}
	}

	void exchangeRemainMissile(string type, int which){
		if(GameInit.remainMissile[type] <= 0){
			return;
		}
		int needNum = maxMissiles[which] - currentMissiles[which];
		int canSupplyNum = GameInit.remainMissile[type];
		//Debug.Log ("gqb------>needNum: " + needNum + "; canSupplyNum: " + canSupplyNum);
		if (canSupplyNum <= needNum) {
			currentMissiles [which] = currentMissiles [which] + canSupplyNum;
			GameInit.remainMissile [type] = 0;
		} else {
			currentMissiles [which] = maxMissiles [which];
			GameInit.remainMissile [type] = GameInit.remainMissile [type] - needNum;
		}
	}

	void exchange(CarType5 mCarType5){
		if(mCarType5.isDestoryed){
			return;
		}

		int needNum = mCarType5.maxMountMissle - mCarType5.currentMountMissle;
		int canSupplyNum = currentMissiles[0];

		if(canSupplyNum <= needNum){
			mCarType5.addMissile (canSupplyNum);
			//mCarType5.currentMountMissle = mCarType5.currentMountMissle + canSupplyNum;
			currentMissiles[0] = 0;
		}else{
			//mCarType5.currentMountMissle = mCarType5.maxMountMissle;
			mCarType5.addMissile (needNum);
			currentMissiles[0] = currentMissiles[0] - needNum;
		}
		mCarType5.isSupplied = false;
		supplyCar5 = null;
	}

	void findNeedMissile1(){
		if(supplyCar5 != null){
			return;
		}
		Dictionary<int, GameObject> Car5s = new Dictionary<int, GameObject>();
		if("0".Equals (playerId)) {
			Car5s = GameInit.Car5Area0;
		}else if("1".Equals (playerId)){
			Car5s = GameInit.Car5Area1;
		}
		int currentPriority = 4;
		GameObject tempSupplyCar5 = null;
		foreach(int key in Car5s.Keys){
			GameObject car5 = Car5s[key];
			if(car5 == null){
				continue;
			}
			CarType5 mCarType5= car5.GetComponent<CarType5> ();
			if(mCarType5 != null && !mCarType5.isSupplied && (mCarType5.currentMountMissle < mCarType5.maxMountMissle) 
				&& (mCarType5.priority < currentPriority)){
				currentPriority = mCarType5.priority;
				tempSupplyCar5 = car5;
			}
		}
		if(tempSupplyCar5 != null){
			CarType5 mCarType5= tempSupplyCar5.GetComponent<CarType5> ();
			mCarType5.isSupplied = true;
			supplyCar5 = tempSupplyCar5;
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
		nav.destination = navPoint;//target.transform.position;
		float patrol = Random.Range(maxMoveSpeed*0.9f, maxMoveSpeed);
		nav.speed = patrol;
		nav.acceleration = patrol * 2f;
		nav.autoRepath = true;
		//nav.baseOffset = 50;
		nav.angularSpeed = 100;
		run ();
	}

	public void startNav(Vector3 navPoint, float speed){
		if(navCube == null){
			createNavCube ();
		}
		if(!nav.enabled){
			return;
		}
		nav.destination = navPoint;//target.transform.position;
		nav.speed = speed;
		nav.acceleration = speed * 2;
		nav.autoRepath = true;
		nav.angularSpeed = 100;
	}

	public void createNavCube(){
		navCube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		navCube.transform.position = new Vector3(transform.position.x, 6f, transform.position.z);
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
		if(type ==2){
			sliderHealth.value = sliderHealth.value - 68;
			if(sliderHealth.value <= 0){
				isDestoryed = true;
				DestoryAll (collision.contacts[0].point, 120000.0f);
				destoryData ();
				return;
			}
		}else if(type ==4){
			sliderHealth.value = sliderHealth.value - 2.6f;
			if(sliderHealth.value <= 0){
				isDestoryed = true;

				GameObject bomb2 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bomb2"), 
					transform.position, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
				bomb2.tag = "bomb2";
				bomb2.transform.parent = transform;

				DestoryAll (transform.position, 10000.0f);
				destoryData ();
			}
		}
	}

	public void destoryData(){
		GameInit.currentInstance [(playerId + "_missile1")] = GameInit.currentInstance [(playerId + "_missile1")] - currentMissiles [0];
		currentMissiles [0] = 0;

		GameInit.currentInstance [(playerId + "_missile2")] = GameInit.currentInstance [(playerId + "_missile2")] - currentMissiles [1];
		currentMissiles [1] = 0;

		GameInit.currentInstance [(playerId + "_missile3")] = GameInit.currentInstance [(playerId + "_missile3")] - currentMissiles [2];
		currentMissiles [2] = 0;

		if(GameInit.currentInstance.ContainsKey((string)tag)){
			GameInit.currentInstance[tag] = (int)GameInit.currentInstance[tag] - 1;
		}
		if(GameInit.gameObjectInstance.Contains(transform.gameObject)){
			GameInit.gameObjectInstance.Remove (transform.gameObject);
		}
		if("0".Equals(playerId) && GameInit.myThumbnailObjs.Contains(transform.gameObject)){
			GameInit.myThumbnailObjs.Remove (transform.gameObject);
		}

		if ("0".Equals (playerId)) {
			GameInit.park0 [park] = 0;
		}else{
			GameInit.park1 [park] = 0;
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
		for(int i =0;i <= 5; i++){
			Transform lunzi_lod0 = transform_lod0.FindChild (("lunzi" + (i + 1).ToString()));
			Transform lunzi_lod1 = transform_lod1.FindChild (("lunzi" + (i + 1).ToString()));
			Transform lunzi_lod2 = transform_lod2.FindChild (("lunzi" + (i + 1).ToString()));
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
			(point.y - 300), 
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
		Transform aim = transform.FindChild ("aim");
		if(aim != null){
			Destroy (aim.gameObject);
		}
	}

	public void destoryAll(){
		if ("0".Equals (playerId)) {
			GameInit.MyCar2.Remove (transform);
		} else if("1".Equals (playerId)){
			GameInit.EnemyCar2.Remove (transform);
		}
		if(mainTransform_lod0 != null){
			Destroy (mainTransform_lod0.gameObject);
		}
		for(int i=0;i<= 5;i++){
			if(lunzis[i] != null){
				Destroy(lunzis[i].gameObject);
			}
		}

		Destroy(transform.root.gameObject);
	}


}
