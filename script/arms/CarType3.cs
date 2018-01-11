using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarType3 : PojulObject {

	private float height = 37.5f; //60.0f;

	private AudioSource mAudioSource;

	private Transform transform_lod0;
	private Transform transform_lod1;
	private Transform transform_lod2;

	private Transform[] lunzis = new Transform[8];

	private Transform fireTransform;
	private Transform aimTransform;

	private Animator mAnimator_lod0;
	private Animator mAnimator_lod1;
	private Animator mAnimator_lod2;

	private Transform mainTransform_lod0;
	private Transform mainTransform_lod1;
	private Transform mainTransform_lod2;

	private Transform panTransform_lod0;
	private Transform panTransform_lod1;
	private Transform panTransform_lod2;

	private Renderer mRenderer_lod0_main;
	private Renderer mRenderer_lod1_main;
	private Renderer mRenderer_lod2_main;

	private Transform paoTransform_lod0;
	private Transform paoTransform_lod1;
	private Transform paoTransform_lod2;

	private float aimSpeed = 5.0f;
	private float aimMaxVer = 12.0f;

	private bool isMoving = false;
	private float maxMoveSpeed = GameInit.mach * 0.4f;
	private float speed =  0.0f;

	//test
	private Transform target;
	private float fireInterval = 6.0f;
	private float lastFileTime = 0.0f;

	private Rigidbody mPlayerRigidbody;
	public GameObject navCube;
	public UnityEngine.AI.NavMeshAgent nav;

	public RadiusArea mPatrolArea;
	public List<Vector3> attackMasses;
	public Vector3 massPark = new Vector3(0,0,0);
	int massIndex = -1;
	public float massSpeed;
	private Transform transporter;
	//private float transportRawHeight = 0;

	//血量条
	public Slider sliderHealth;
	/*private float rawWidth = Screen.width/10;
	private float rawHeight = Screen.width/100;
	private float RawHeightSpace = Screen.height / 40;
	private int currentHealthy = 10;
	private int maxtHealthy = 100;
	private Transform healthTranssform;*/


	// Use this for initialization
	void Start () {

		//transform.position = new Vector3 (transform.position.x, height, transform.position.z);

		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		transform_lod0 = transform.FindChild ("car_type3_lod0");
		transform_lod1 = transform.FindChild ("car_type3_lod1");
		transform_lod2 = transform.FindChild ("car_type3_lod2");
		fireTransform = transform.FindChild ("car_type3_lod0").FindChild ("pan").FindChild("pao").FindChild("fire");
		aimTransform = transform.FindChild ("aim");

		mAnimator_lod0 = (Animator)transform_lod0.GetComponent<Animator> ();
		mAnimator_lod1 = (Animator)transform_lod1.GetComponent<Animator> ();
		mAnimator_lod2 = (Animator)transform_lod2.GetComponent<Animator> ();
		mAnimator_lod0.SetBool ("roll", false);
		mAnimator_lod1.SetBool ("roll", false);
		mAnimator_lod2.SetBool ("roll", false);

		panTransform_lod0 = transform_lod0.FindChild("pan");
		panTransform_lod1 = transform_lod1.FindChild("pan");
		panTransform_lod2 = transform_lod2.FindChild("pan");

		mainTransform_lod0 = transform_lod0.FindChild("main");
		mainTransform_lod1 = transform_lod1.FindChild("main");
		mainTransform_lod2 = transform_lod2.FindChild("main");

		mRenderer_lod0_main = mainTransform_lod0.GetComponent<Renderer>();
		mRenderer_lod1_main = mainTransform_lod1.GetComponent<Renderer>();
		mRenderer_lod2_main = mainTransform_lod2.GetComponent<Renderer>();

		paoTransform_lod0 = panTransform_lod0.FindChild ("pao");
		paoTransform_lod1 = panTransform_lod1.FindChild ("pao");
		paoTransform_lod2 = panTransform_lod2.FindChild ("pao");
		if (lunzis [0] == null) {
			getLunzis ();
		}
		//healthTranssform = transform.FindChild ("health");

		massSpeed = Random.Range(GameInit.mach * 0.39f, GameInit.mach * 0.48f);

		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];
		RectTransform mRectTransform = sliderHealth.GetComponent<RectTransform> ();
		if("0".Equals(playerId)){
			mPatrolArea = new RadiusArea (Random.Range(1,4));
			myCenter = new Vector3 (0,0,-60000);
			enemyCenter = new Vector3 (0,0,60000);
			enemyId = "1";
			transform.position = new Vector3 (transform.position.x, height, transform.position.z);
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.251f, 0.647f, 0.78f);
			attackMasses = GameInit.attackMasses_0;
		}else if("1".Equals(playerId)){
			mPatrolArea = new RadiusArea (Random.Range(5,8));
			myCenter = new Vector3 (0,0,60000);
			enemyCenter = new Vector3 (0,0,-60000);
			enemyId = "0";
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.698f, 0.255f, 0.157f);
			transform.position = new Vector3 (transform.position.x, height, transform.position.z);
			/*transform.position = new Vector3 (-40000,
				25, 
				-51000);*/
			attackMasses = GameInit.attackMasses_1;
		}

		if(playerType == 1){
			mRectTransform.sizeDelta = new Vector2 (Screen.width / 18, Screen.width / 60);
		}
		sliderHealth.value = sliderHealth.maxValue;

		//test
		GameObject targetObj = null;//GameObject.FindGameObjectWithTag("player");
		if(targetObj != null){
			target = targetObj.transform.FindChild ("aim");
		}
		run ();

		createNavCube ();
		InvokeRepeating("behaviorListener", 0.5f, 0.5f);
		InvokeRepeating ("findInvade", 2.0f, 2.0f);
		InvokeRepeating ("interceptInvade", 6.0f, 6.0f);
		//startNav (new Vector3 (3340, 60, -34925));
		//startNav (new Vector3 (0, 60, 0));
	}

	// Update is called once per frame
	void Update () {
		if(isDestoryed){
			return;
		}

		if(isMoving){
			listenerRollAni ();
		}

		if (playerType == 0) {
			navCube.transform.position = new Vector3 (transform.position.x, navCube.transform.position.y, transform.position.z);
			navCube.transform.rotation = transform.rotation;
			return;
		}

		sliderHealth.transform.rotation = Quaternion.Euler(mainTransform_lod0.rotation.eulerAngles.x , 
			Camera.main.transform.rotation.eulerAngles.y,
			mainTransform_lod0.rotation.eulerAngles.z);

		//Debug.DrawRay(paoTransform_lod0.position, paoTransform_lod0.forward*12000, Color.white);
		if(!isPanDestoryed){
			aimEnemy (target);
		}
	}

	void getLunzis(){
		if(transform_lod0 == null){
			transform_lod0 = transform.FindChild ("car_type3_lod0");
		}
		for (int i = 0; i <= 7; i++) {
			lunzis[i] = transform_lod0.FindChild (("lunzi00" + (i + 1).ToString ()));
		}
	}

	public override void setPlayType(int playerType){
		this.playerType = playerType;
		if (playerType == 0) {
			//mCanvas.enabled = false;
			sliderHealth.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, 0);

			if (nav != null) {
				nav.enabled = false;
				transform.parent = null;
				transform.rotation = navCube.transform.rotation;
				PlanControls.newPoint1Rolation = transform.rotation.eulerAngles.y;
			}
			if (mAudioSource != null) {
				mAudioSource.volume = 0.15f;
			}
			if(!transform.gameObject.GetComponent<planMove> ()){
				transform.gameObject.AddComponent<planMove> ();
			}
			if (mPlayerRigidbody == null) {
				transform.gameObject.AddComponent<Rigidbody> ();
				mPlayerRigidbody = transform.gameObject.GetComponent<Rigidbody> ();
				mPlayerRigidbody.mass = 1;
				mPlayerRigidbody.useGravity = true;
				mPlayerRigidbody.drag = 1.5f;
				mPlayerRigidbody.angularDrag = 1.5f;
				planMove.mRigidbody = mPlayerRigidbody;
			}
			planMove.player = transform;
			planMove.speed = speed;
			planMove.maxSpeed = 600;
			planMove.maxAccelerate = 1.2f;
			PlanControls.rorateSpeed = 35f;
			planMove.rolSpeed = 7.0f;
			if(fireTransform == null){
				fireTransform = transform.FindChild ("car_type3_lod0").FindChild ("pan").FindChild("pao").FindChild("fire");
			}
			WorldUIManager.fireAimTra = fireTransform;
			WorldUIManager.fireAimDistance = 12000.0f;
			GameObject.FindGameObjectWithTag ("mainUI").GetComponent<UImanager> ().setPlayerUI ("car3");
			if(lunzis[0] == null){
				getLunzis ();
			}
			for (int i = 0; i <= 7; i++) {
				lunzis [i].GetComponent<CapsuleCollider> ().enabled = true;
			}

		} else if (playerType == 1) {
			/*
			*need judge is in nav area
			*/
			sliderHealth.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width / 18, Screen.width / 60);
			if (nav != null) {
				nav.enabled = true;
				transform.parent = navCube.transform;
			}
			if (mAudioSource != null) {
				mAudioSource.volume = 0.8f;
			}
			if (mPlayerRigidbody != null) {
				Destroy (transform.gameObject.GetComponent<Rigidbody> ());
				planMove.mRigidbody = null;
			}
			if(transform.gameObject.GetComponent<planMove> ()){
				Destroy (transform.gameObject.GetComponent<planMove> ());
			}
			mAnimator_lod0.speed = 5f;
			mAnimator_lod1.speed = 5f;
			planMove.player = null;
			WorldUIManager.fireAimTra = null;
			WorldUIManager.fireAimDistance = 0.0f;
			if(lunzis[0] == null){
				getLunzis ();
			}
			for (int i = 0; i <= 7; i++) {
				lunzis [i].GetComponent<CapsuleCollider> ().enabled = false;
			}
			//GameObject.FindGameObjectWithTag ("mainUI").GetComponent<UImanager> ().setPlayerUI ("car3");
		}
	}

	void behaviorListener(){

		if(isDestoryed){
			return;
		}

		if (playerType == 0) {
			float aniSpeed =  planMove.speed * 1.0f / maxMoveSpeed;
			if(Mathf.Abs(aniSpeed - mAnimator_lod0.speed) > 0.1){
				mAnimator_lod0.speed = aniSpeed;
				mAnimator_lod1.speed = aniSpeed;
			}
			return;
		}

		if(transporter != null){
			behavior = 5;
			return;
		}

		if(behavior == 5){
			return;
		}

		if (Util.isOnEnemyNavArea1 (transform.position, playerId)) {
			behavior = 3;
		} else if (Util.isOnMyNavArea1 (transform.position, playerId)) {
			if (behavior == 3) {
				behavior = 2;
			} else if (behavior == 4) {
				behavior = 1;
			}
		}

		if (MissileAimedTra == null) {
			isMissileAimed = false;
		} else {
			isMissileAimed = true;
		}

		rayCastEnemy ();

		if(nav != null  && 
			(nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid || nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial) ){
			navCube.transform.position = new Vector3 ((navCube.transform.position.x + 20), navCube.transform.position.y, (navCube.transform.position.z + 20));
			//startNav(mPatrolArea.getRandomPoint());
		}

		if(target!= null && nav != null && nav.enabled && behavior == 1){
			if(!Util.isOnMyNavArea1(target.position, playerId)){
				target = null;
			}	
		}

		if(nav != null && nav.enabled && behavior == 1 && !nav.hasPath && !nav.pathPending && mPatrolArea != null){
			startNav(mPatrolArea.getRandomPoint());
			massPark = new Vector3 (0,0,0);
		}else if(behavior == 3 && nav.destination != enemyCenter){
			startNav(enemyCenter, 270);
		}

		if(behavior == 2){
			findMassPark ();
		}

	}

	void findMassPark(){
		if ((new Vector3(transform.position.x, 0, transform.position.z) - massPark).magnitude < 3) {
			if(nav.remainingDistance < 3){
				stop ();
			}
			int massId;
			if ("0".Equals (playerId)) {
				massId = UImanager.massId_0;
			} else {
				massId = UImanager.massId_1;
			}
			 
			if (massIndex <= 0 && (new Vector3(transform.position.x, 0, transform.position.z) - new Vector3 ((attackMasses [massId].x -600), 
				0, attackMasses [massId].z)).magnitude < 3) {
				return;
			}
			Vector3 nextMassPark = new Vector3 ((attackMasses [massId].x -600), 
				0, 
				(attackMasses [massId].z - (massIndex -1) * 600));
			if(isMassParkIdle(nextMassPark)){
				massPark = nextMassPark;
				startNav (nextMassPark, massSpeed);
				massIndex = massIndex - 1;
			}
		} else {
			int massId;
			if ("0".Equals (playerId)) {
				massId = UImanager.massId_0;
			} else {
				massId = UImanager.massId_1;
			}
			for(int i = 0; i < 9; i++){
				Vector3 tempMassPark = new Vector3 ((attackMasses [massId].x -600), 
					0, 
					(attackMasses [massId].z - i * 600)
				);
				bool isIdle = isMassParkIdle (tempMassPark);
				if(isIdle){
					massPark = tempMassPark;
					startNav (tempMassPark, massSpeed);
					massIndex = i;
					break;
				}
			}
		}
	}

	bool isMassParkIdle(Vector3 massPark){
		RaycastHit hit;
		if(Physics.Raycast (new Vector3(massPark.x, 300, massPark.z), Vector3.down, out hit, 500.0f)){
			if(hit.transform.root.childCount > 0 && !hit.transform.root.GetChild(0).tag.Equals("Untagged") && hit.transform.root.GetChild(0) != transform){
				return false;
			}
		}
		return true;
	}

	public override void setTransport(Transform transporter, bool isTransport){
		if (isTransport) {
			this.transporter = transporter;
			behavior = 5;
			nav.enabled = false;
			transform.root.localScale = new Vector3 (0, 0, 0);
			transform.root.position = new Vector3 (-200000, 0, 0);
			//transform.root.parent = transporter;
			stop ();
		} else {
			this.transporter = null;
			int attackBehavorId;
			if ("0".Equals (playerId)) {
				attackBehavorId = UImanager.attackBehavorId_0;
			} else {
				attackBehavorId = UImanager.attackBehavorId_1;
			}
			behavior = attackBehavorId;
			transform.root.position = new Vector3 ((transporter.position.x - 600), 0, (transporter.position.z - Random.Range(0, 800)));
			transform.root.localScale = new Vector3 (10, 10, 10);
			nav.enabled = true;
		}

	}

	void rayCastEnemy(){
		if (playerType == 0) {
			return;
		}
		if(target == null || isPanDestoryed || isDestoryed){
			return;
		}
		RaycastHit hit;
		if(paoTransform_lod0 != null && Physics.Raycast (paoTransform_lod0.position, paoTransform_lod0.forward, out hit, 12000.0f)){
			if(hit.transform != null && (Time.time - lastFileTime) > fireInterval && hit.transform.root.childCount > 0){
				//Debug.Log (hit.transform.root.GetChild(0).name + "; gqb------>hit:" + hit.transform.root.GetChild(0).tag);
				if (hit.transform.root.childCount <= 0) {
					return;
				}
				string tag = hit.transform.root.GetChild (0).tag;
				if(tag.Equals("Untagged")){
					tag = hit.transform.root.tag;
				}
				string[] tags = tag.Split ('_');
				if (tags.Length == 2 && enemyId.Equals(tags [0]) && ("car2".Equals (tags [1]) || "car3".Equals (tags [1])
				   || "car4".Equals (tags [1]) || "car5".Equals (tags [1]) || "car6".Equals (tags [1]))) {
					lastFileTime = Time.time;
					fire ();
				}
			}
		}
	}

	void fire(){
		GameObject fire = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/tankFire"), 
			fireTransform.position, fireTransform.rotation) as GameObject;
		fire.tag = "tankFire";
		fire.transform.parent = transform;
		shootShell ();
	}

	void shootShell(){
		GameObject shell1 = (GameObject)Instantiate(Resources.Load("Prefabs/arms/shell_type1"), 
			(paoTransform_lod0.position + paoTransform_lod0.forward*50), paoTransform_lod0.rotation) as GameObject;
		shell1.tag = "shell1";
		((ShellType1)shell1.GetComponent<ShellType1> ()).shoot(7000, 0);
	}

    void aimEnemy(Transform enemyTransform){
		if (playerType == 0) {
			return;
		}
		if(target == null || isPanDestoryed || isDestoryed){
			return;
		}
		panTransform_lod0.rotation = Quaternion.Slerp(panTransform_lod0.rotation, 
			Quaternion.LookRotation(enemyTransform.position - panTransform_lod0.position), aimSpeed * Time.deltaTime);

		paoTransform_lod0.rotation = Quaternion.Slerp(paoTransform_lod0.rotation, 
			Quaternion.LookRotation(enemyTransform.position - paoTransform_lod0.position), aimSpeed * Time.deltaTime);
		
		float paoAnglesX_lod0 = paoTransform_lod0.localEulerAngles.x;

		if(paoAnglesX_lod0 > aimMaxVer && paoAnglesX_lod0 <= 180){
			paoAnglesX_lod0 = aimMaxVer;
		}else if(paoAnglesX_lod0 > 180 && paoAnglesX_lod0 < (360 - aimMaxVer)){
			paoAnglesX_lod0 = 360 - aimMaxVer;
		}
		panTransform_lod0.localEulerAngles = new Vector3(0, panTransform_lod0.localEulerAngles.y, 0);
		panTransform_lod1.localEulerAngles = new Vector3(0, panTransform_lod0.localEulerAngles.y, 0);
		panTransform_lod2.localEulerAngles = new Vector3(0, panTransform_lod0.localEulerAngles.y, 0);

		paoTransform_lod0.localEulerAngles = new Vector3(paoAnglesX_lod0, 0, 0);
		paoTransform_lod1.localEulerAngles = new Vector3(paoAnglesX_lod0, 0, 0);
		paoTransform_lod2.localEulerAngles = new Vector3(paoAnglesX_lod0, 0, 0);
	}

	void listenerRollAni(){
		if(isPanDestoryed || isDestoryed){
			return;
		}
		if (mRenderer_lod0_main.isVisible && !mAnimator_lod0.GetBool ("roll")) {
			mAnimator_lod0.SetBool ("roll", true);
			mAnimator_lod1.SetBool ("roll", false);
		} else if (mRenderer_lod1_main.isVisible && !mAnimator_lod1.GetBool ("roll")) {
			mAnimator_lod1.SetBool ("roll", true);
			mAnimator_lod0.SetBool ("roll", false);
		} else if(!mRenderer_lod0_main.isVisible && !mRenderer_lod1_main.isVisible){
			mAnimator_lod0.SetBool ("roll", false);
			mAnimator_lod1.SetBool ("roll", false);
		}
	}

	void run(){
		isMoving = true;
		if(mAudioSource != null && !mAudioSource.isPlaying){
			mAudioSource.Play ();
		}
	}

	void stop(){
		isMoving = false;
		mAnimator_lod0.SetBool ("roll", false);
		mAnimator_lod1.SetBool ("roll", false);
		if (mAudioSource != null && mAudioSource.isPlaying) {
			mAudioSource.Stop();
		}
	}

	public void startNav(Vector3 navPoint){
		if(navCube == null){
			createNavCube ();
		}
		if(!nav.enabled){
			return;
		}
		nav.destination = new Vector3(navPoint.x, 5, navPoint.z);
		float patrol = Random.Range(maxMoveSpeed*0.6f, maxMoveSpeed);
		nav.speed = patrol;
		speed = nav.speed;
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
		nav.destination = new Vector3(navPoint.x, 5, navPoint.z);//target.transform.position;
		nav.speed = speed;
		this.speed = nav.speed;
		nav.acceleration = speed * 2;
		nav.autoRepath = true;
		nav.angularSpeed = 100;
		run ();
	}

	public void createNavCube(){
		navCube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		navCube.transform.position = new Vector3(transform.position.x, 5f, transform.position.z);
		navCube.transform.localScale = new Vector3 (10, 10, 10);
		navCube.AddComponent<UnityEngine.AI.NavMeshAgent>();
		Destroy(navCube.GetComponent<BoxCollider> ());
		transform.parent = navCube.transform;

		MeshRenderer m = navCube.GetComponent<MeshRenderer>();
		m.enabled = false;
		nav= navCube.GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	public override void fireOfPlayer(){
		if(isDestoryed){
			return;	
		}
		if(mPlayerRigidbody != null){
			//mPlayerRigidbody.AddForce (-panTransform_lod0.forward  * 6000);
		}
		fire ();
	}

	public override void isFired(RaycastHit hit, Collision collision, int type){
		if(isDestoryed){
			return;
		}
		if(type ==2){
			Vector3 hitPoint;
			string hitName = "";
			if (collision != null) {
				hitPoint = collision.contacts[0].point;
				hitName = collision.gameObject.name;
			} else {
				hitPoint = hit.point;
				hitName = hit.transform.name;
			}
			sliderHealth.value = sliderHealth.value - 36;
			if(sliderHealth.value <= 0){
				isDestoryed = true;
				isPanDestoryed = true;
				DestoryAll (hitPoint, 30000.0f);
				destoryData ();
				return;
			}
			if(hitName.Equals("pan") && !isPanDestoryed){
				isPanDestoryed = true;
				destoryPan (hitPoint);
			}
		}else if(type ==3){
			isDestoryed = true;
			isPanDestoryed = true;
			DestoryAll (collision.contacts[0].point, 30000.0f);
			destoryData ();
			return;
		}else if (type == 4) {
			sliderHealth.value = sliderHealth.value - 0.8f;
			if(sliderHealth.value <= 0){
				isDestoryed = true;
				isPanDestoryed = true;
				GameObject bomb2 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bomb2"), 
					transform.position, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
				bomb2.tag = "bomb2";
				bomb2.transform.parent = transform;
				DestoryAll (transform.position, 1000.0f);
				destoryData ();
			}
		}
	}

	void DestoryAll(Vector3 point, float force){

		if(playerType == 0){
			UImanager.isOnLeave = true;
		}

		if(panTransform_lod0 != null){
			panTransform_lod0.parent = null;
			panTransform_lod1.parent = panTransform_lod0;
			panTransform_lod2.parent = panTransform_lod0;
			if(!panTransform_lod0.gameObject.GetComponent<Rigidbody> ()){
				panTransform_lod0.gameObject.AddComponent<Rigidbody> ();
			}
			paoTransform_lod0.parent = null;
			paoTransform_lod1.parent = paoTransform_lod0;
			paoTransform_lod2.parent = paoTransform_lod0;
			if(!paoTransform_lod0.gameObject.GetComponent<MeshCollider> ()){
				paoTransform_lod0.gameObject.AddComponent<MeshCollider> ();
				paoTransform_lod0.gameObject.GetComponent<MeshCollider> ().convex = true;
				paoTransform_lod0.gameObject.AddComponent<Rigidbody> ();
				paoTransform_lod0.gameObject.GetComponent<Rigidbody> ().mass = 0.9f;
			}
		}

		mainTransform_lod0.parent = null;
		mainTransform_lod1.parent = mainTransform_lod0;
		mainTransform_lod2.parent = mainTransform_lod0;
		if(!mainTransform_lod0.gameObject.GetComponent<Rigidbody>()){
			mainTransform_lod0.gameObject.AddComponent<Rigidbody> ();
			mainTransform_lod0.gameObject.GetComponent<Rigidbody> ().mass = 1.4f;
		}

		for(int i =0;i <= 7; i++){
			Transform lunzi_lod0 = transform_lod0.FindChild (("lunzi00" + (i + 1).ToString()));
			Transform lunzi_lod1 = transform_lod1.FindChild (("lunzi00" + (i + 1).ToString()));
			Transform lunzi_lod2 = transform_lod2.FindChild (("lunzi00" + (i + 1).ToString()));
			lunzi_lod0.parent = null;
			lunzi_lod1.parent = lunzi_lod0;
			lunzi_lod2.parent = lunzi_lod0;
			//lunzis [i] = lunzi_lod0;
			lunzis [i].gameObject.GetComponent<CapsuleCollider>().enabled = true;
			if(!lunzis [i].gameObject.GetComponent<Rigidbody>()){
				lunzis [i].gameObject.AddComponent<Rigidbody> ();
				lunzis [i].gameObject.GetComponent<Rigidbody> ().mass = 0.8f;
			}
		}
		Vector3 explosionPos = new Vector3 (point.x, 
			(point.y - 200), 
			point.z);
		Collider[] colliders = Physics.OverlapSphere(explosionPos, 200.0f);
		foreach (Collider hit in colliders){
			if (hit.GetComponent<Rigidbody>() && hit.transform != Camera.main.transform){
				hit.GetComponent<Rigidbody>().AddExplosionForce(force, explosionPos, 200.0f);
			}  
		}
		Invoke ("destoryAll", 30);
		if(nav != null){
			nav.enabled = false;	
		}
		stop ();
		Transform aim = transform.FindChild ("aim");
		if(aim != null){
			Destroy (aim.gameObject);
		}
	}

	void destoryPan(Vector3 hitPoint){
		panTransform_lod0.parent = null;
		panTransform_lod1.parent = panTransform_lod0;
		panTransform_lod2.parent = panTransform_lod0;
		if (!panTransform_lod0.gameObject.GetComponent<Rigidbody> ()) {
			panTransform_lod0.gameObject.AddComponent<Rigidbody> ();
		}
		paoTransform_lod0.parent = null;
		paoTransform_lod1.parent = paoTransform_lod0;
		paoTransform_lod2.parent = paoTransform_lod0;
		if (!paoTransform_lod0.gameObject.GetComponent<MeshCollider> ()) {
			paoTransform_lod0.gameObject.AddComponent<MeshCollider> ();
			paoTransform_lod0.gameObject.GetComponent<MeshCollider> ().convex = true;
			paoTransform_lod0.gameObject.AddComponent<Rigidbody> ();
		}

		Vector3 explosionPos = new Vector3 (hitPoint.x, 
			(hitPoint.y - 10), 
			hitPoint.z);
		Collider[] colliders = Physics.OverlapSphere(explosionPos, 50.0f);
		foreach (Collider hit in colliders){
			if (hit.GetComponent<Rigidbody>()){ 
				hit.GetComponent<Rigidbody>().AddExplosionForce(6000.0f, explosionPos, 50.0f);
			}  
		}

		if(aimTransform != null && mainTransform_lod0 != null){
			aimTransform.position = mainTransform_lod0.position;
		}

		Invoke ("destoryPan", 20);
	}

	void destoryPan(){
		if(panTransform_lod0 != null){
			Destroy (panTransform_lod0.gameObject);
		}
		if(paoTransform_lod0 != null){
			Destroy (paoTransform_lod0.gameObject);
		}
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
		}
	}

	public void destoryAll(){
		if(panTransform_lod0 != null){
			Destroy (panTransform_lod0.gameObject);
		}
		if(paoTransform_lod0 != null){
			Destroy (paoTransform_lod0.gameObject);
		}
		if(mainTransform_lod0 != null){
			Destroy (mainTransform_lod0.gameObject);
		}
		for(int i=0;i<= 7;i++){
			if(lunzis[i] != null){
				Destroy(lunzis[i].gameObject);
			}
		}

		Destroy(transform.root.gameObject);
	}

	void findInvade(){
		if(isDestoryed){
			return;
		}

		Collider[] colliders = Physics.OverlapSphere (transform.position, 12000);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders [i].transform.root.childCount <= 0) {
				continue;
			}
			Transform tempTransform = colliders [i].transform.root.GetChild (0);
			string tag = tempTransform.tag;
			if (tag.Equals ("Untagged")) {
				if (colliders [i].transform.root == null) {
					continue;
				}
				if (colliders [i].transform.root.tag.Equals ("Untagged")) {
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
				if (enemyId.Equals (tempPlayerId)) {
					if (target == null && ("car2".Equals (tempType) || "car3".Equals (tempType) || "car5".Equals (tempType))) {
						target = tempTransform.FindChild ("aim");
					}
					Util.AddNearEnemys (tempTransform, playerId);
				}
			}
		}
		if(target == null){
			findDangeroust ();
		}
	}

	void findDangeroust(){
		if(isDestoryed){
			return;
		}

		if(playerType ==0){
			return;
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
			string[] strs = tempDangeroustEnemy.tag.Split ('_');
			if(strs.Length != 2){
				return;
			}
			float tempDis = (myCenter - tempDangeroustEnemy.position).magnitude;
			if(tempDis < dangeroustDis && ("car2".Equals (strs[1]) || "car3".Equals (strs[1]) || "car5".Equals (strs[1]))){
				//Debug.Log (transform.tag +  "; gqb------>findDangeroust: " +  tempDangeroustEnemy.tag );
				dangeroustEnemy = tempDangeroustEnemy;
				dangeroustDis = tempDis;
			}
		}
		if(dangeroustEnemy != null && dangeroustEnemy.FindChild("aim") != null){
			target = dangeroustEnemy.FindChild("aim");
		}
	}

	void interceptInvade(){
		if (playerType == 0) {
			return;
		}
		if(behavior == 1 && target != null){
			float distance = (transform.position - target.position).magnitude;
			if(distance > 50000){
				speed = 660;
			}else if(distance > 30000){
				speed = 600;
			}else if(distance > 20000){
				speed = 530;
			}else if(distance > 9000){
				speed = 480;
			}else {
				Random.Range(maxMoveSpeed*0.5f, maxMoveSpeed);
			}
			startNav (new Vector3(target.position.x, 0, target.position.z), speed);
			//float distance = (transform.position - target.position).magnitude;
			//float speed = 280;
			//if (distance > 5000) {
				//speed = distance * 40/ 5000  + 280;
			//}
			//if(distance < 10000){
			/*float toCenterDis = (target.position - myCenter).magnitude;
			if ( toCenterDis > 20000 && toCenterDis < 45000) {
				startNav (new Vector3((target.position.x + Random.Range(-3000,3000)),
					height, 
					(target.position.z + Random.Range(-3000,3000))), speed);
			} else{
				startNav (new Vector3(target.position.x, 0, target.position.z), speed);
			}*/

			//}
		}
	}

	public void rotatePan(float rotationY){
		if(isDestoryed || isPanDestoryed){
			return;
		}
		float dy = panTransform_lod0.localEulerAngles.y + rotationY *0.3f;
		panTransform_lod0.localEulerAngles = new Vector3(0, dy, 0);
		panTransform_lod1.localEulerAngles = new Vector3(0, dy, 0);
		panTransform_lod2.localEulerAngles = new Vector3(0, dy, 0);
	}

	public void rotatePao(float rotationX){
		if(isDestoryed || isPanDestoryed || Mathf.Abs(rotationX) > 360){
			return;
		}
		//Debug.Log ("gqb------>rotationX0: " + rotationX);
		rotationX = -rotationX * aimMaxVer * 0.01f;
		//Debug.Log ("gqb------>rotationX1: " + rotationX);
		if(rotationX < 0){
			rotationX = 360 + rotationX;
		}

		if(rotationX <1 || rotationX > 359){
			rotationX = 0;
		}

		paoTransform_lod0.localEulerAngles = new Vector3(rotationX, 0, 0);
		paoTransform_lod1.localEulerAngles = new Vector3(rotationX, 0, 0);
		paoTransform_lod2.localEulerAngles = new Vector3(rotationX, 0, 0);

	}

	public void rotateCamera(){
		if(isDestoryed){
			return;
		}
		//Camera.main.transform.position = new Vector3(transform.position.x, (transform.position.y + 200), transform.position.z);
		//Camera.main.transform.rotation = Quaternion.LookRotation (transform.position - Camera.main.transform.position);
		if(isPanDestoryed){
			Camera.main.transform.position = transform.position +  (-transform.forward * planMove.dzMainCamera + transform.up * planMove.dyMainCamera);
			Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, 
				Quaternion.LookRotation(transform.position - Camera.main.transform.position), 3.5f * Time.deltaTime);
			return;
		}
		Camera.main.transform.position = paoTransform_lod0.position +  (-paoTransform_lod0.forward * planMove.dzMainCamera + paoTransform_lod0.up * planMove.dyMainCamera);
		Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, 
			Quaternion.LookRotation(paoTransform_lod0.position - Camera.main.transform.position), 3.5f * Time.deltaTime);
		
	}

}
