using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarType4 : PojulObject {

	private AudioSource mAudioSource;

	private Transform transform_lod0;
	private Transform transform_lod1;
	private Transform transform_lod2;

	private Transform[] lunzis = new Transform[6];
	private Transform fireTransform;

	private Transform mainTransform_lod0;
	private Transform mainTransform_lod1;
	private Transform mainTransform_lod2;

	private Animator mAnimator_lod0;
	private Animator mAnimator_lod1;
	private Animator mAnimator_lod2;

	private Transform panTransform_lod0;
	private Transform panTransform_lod1;
	private Transform panTransform_lod2;

	private Renderer mRenderer_lod0_pan;
	private Renderer mRenderer_lod1_pan;
	private Renderer mRenderer_lod2_pan;

	private Transform paoTransform_lod0;
	private Transform paoTransform_lod1;
	private Transform paoTransform_lod2;

	private float height = 61.0f;
	private float aimSpeed = 1.0f;
	private float aimMaxVer = 10.0f;
	private float maxMoveSpeed = GameInit.mach * 0.4f;

	private bool isMoving = false;

	//test
	private Transform target;
	private float fireInterval = 5.0f;
	private float lastFileTime = 0.0f;

	public GameObject navCube;
	public UnityEngine.AI.NavMeshAgent nav;

	private RadiusArea mPatrolArea;

	//血量条
	public Slider sliderHealth;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (transform.position.x, height, transform.position.z);

		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		transform_lod0 = transform.Find ("car_type4_lod0");
		transform_lod1 = transform.Find ("car_type4_lod1");
		transform_lod2 = transform.Find ("car_type4_lod2");

		fireTransform = transform.Find ("car_type4_lod0").Find ("pan").Find("pao").Find("fire");
		mainTransform_lod0 = transform_lod0.Find("main");
		mainTransform_lod1 = transform_lod1.Find("main");
		mainTransform_lod2 = transform_lod2.Find("main");

		mAnimator_lod0 = (Animator)transform_lod0.GetComponent<Animator> ();
		mAnimator_lod1 = (Animator)transform_lod1.GetComponent<Animator> ();
		mAnimator_lod2 = (Animator)transform_lod2.GetComponent<Animator> ();

		panTransform_lod0 = transform_lod0.Find("pan");
		panTransform_lod1 = transform_lod1.Find("pan");
		panTransform_lod2 = transform_lod2.Find("pan");

		mRenderer_lod0_pan = panTransform_lod0.GetComponent<Renderer>();
		mRenderer_lod1_pan = panTransform_lod1.GetComponent<Renderer>();
		mRenderer_lod2_pan = panTransform_lod2.GetComponent<Renderer>();

		paoTransform_lod0 = panTransform_lod0.Find ("pao");
		paoTransform_lod1 = panTransform_lod1.Find ("pao");
		paoTransform_lod2 = panTransform_lod2.Find ("pao");

		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];

		if("0".Equals(playerId)){
			mPatrolArea = new RadiusArea (Random.Range(1,4));
			myCenter = new Vector3 (0,0,-60000);
			enemyCenter = new Vector3 (0,0,60000);
			enemyId = "1";
			transform.position = new Vector3 (transform.position.x, height, transform.position.z);
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.251f, 0.647f, 0.78f);
		}else if("1".Equals(playerId)){
			mPatrolArea = new RadiusArea (Random.Range(5,8));
			myCenter = new Vector3 (0,0,60000);
			enemyCenter = new Vector3 (0,0,-60000);
			enemyId = "0";
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.698f, 0.255f, 0.157f);
			transform.position = new Vector3 ((GameInit.gameObjectInstance[0].transform.position.x + 5000),
				GameInit.gameObjectInstance[0].transform.position.y, 
				(GameInit.gameObjectInstance[0].transform.position.z + 5000));
		}

		RectTransform mRectTransform = sliderHealth.GetComponent<RectTransform> ();
		mRectTransform.sizeDelta = new Vector2 (Screen.width / 18, Screen.width / 60);
		sliderHealth.value = sliderHealth.maxValue;

		//test
		GameObject targetObj = null;//GameObject.FindGameObjectWithTag("player");
		if(targetObj != null){
			target = targetObj.transform.Find ("aim");
		}
		run ();

		createNavCube ();

		InvokeRepeating("behaviorListener", 0.5f, 0.5f);
		InvokeRepeating ("findInvade", 2.0f, 2.0f);
		InvokeRepeating ("interceptInvade", 5.0f, 5.0f);

	}
	
	// Update is called once per frame
	void Update () {
		sliderHealth.transform.rotation = Quaternion.Euler(mainTransform_lod0.rotation.eulerAngles.x , 
			Camera.main.transform.rotation.eulerAngles.y,
			mainTransform_lod0.rotation.eulerAngles.z);

		if(!isPanDestoryed){
			aimEnemy (target);
		}

		if(isMoving){
			listenerRollAni ();
		}

	}


	void behaviorListener(){
		rayCastEnemy ();

		if(nav != null && behavior == 1 && !nav.hasPath && !nav.pathPending && mPatrolArea != null && target == null){
			startNav(mPatrolArea.getRandomPoint());
		}else if(behavior == 3 && nav.destination != enemyCenter){
			startNav(enemyCenter, 270);
		}
	}

	void rayCastEnemy(){
		if(target == null || isPanDestoryed || isDestoryed){
			return;
		}
		RaycastHit hit;
		if(paoTransform_lod0 != null && Physics.Raycast (paoTransform_lod0.position, paoTransform_lod0.forward, out hit, 12000.0f)){
			//Debug.Log (paoTransform_lod0.forward + "gqb------>hit:" + hit.transform.root.name);
			if(hit.transform != null && (Time.time - lastFileTime) > fireInterval){
				lastFileTime = Time.time;
				fire ();
			}
		}
	}

	void fire(){
		shootShell ();
		GameObject fire = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/tankFire"), 
			fireTransform.position, fireTransform.rotation) as GameObject;
		fire.tag = "tankFire";
	}

	void shootShell(){
		GameObject shell1 = (GameObject)Instantiate(Resources.Load("Prefabs/arms/shell_type1"), 
			(paoTransform_lod0.position + paoTransform_lod0.forward*10), paoTransform_lod0.rotation) as GameObject;
		shell1.tag = "shell1";
		((ShellType1)shell1.GetComponent<ShellType1> ()).shoot(10000, 0, 90);
	}

	void aimEnemy(Transform enemyTransform){
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

		if (mRenderer_lod0_pan.isVisible && !mAnimator_lod0.GetBool ("roll")) {
			mAnimator_lod0.SetBool ("roll", true);
			mAnimator_lod1.SetBool ("roll", false);
		} else if (mRenderer_lod1_pan.isVisible && !mAnimator_lod1.GetBool ("roll")) {
			mAnimator_lod1.SetBool ("roll", true);
			mAnimator_lod0.SetBool ("roll", false);
		} else if(!mRenderer_lod0_pan.isVisible && !mRenderer_lod1_pan.isVisible){
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
		nav.destination = navPoint;//target.transform.position;
		float patrol = Random.Range(maxMoveSpeed*0.5f, maxMoveSpeed);
		nav.speed = patrol;
		nav.acceleration = patrol * 2f;
		nav.autoRepath = true;
		//nav.baseOffset = 50;
		nav.angularSpeed = 100;
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
		navCube.transform.position = transform.position;
		navCube.transform.localScale = new Vector3 (100, 100, 100);
		navCube.AddComponent<UnityEngine.AI.NavMeshAgent>();
		Destroy (navCube.GetComponent<BoxCollider> ());
		transform.parent = navCube.transform;

		MeshRenderer m = navCube.GetComponent<MeshRenderer>();
		m.enabled = false;
		nav= navCube.GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	public override void isFired(RaycastHit hit, Collision collision, int type){
		if(type ==2){
			sliderHealth.value = sliderHealth.value - 36;
			if(sliderHealth.value <= 0 && !isDestoryed){
				isDestoryed = true;
				DestoryAll (collision);
				return;
			}
			if(collision.gameObject.name.Equals("pan") && !isPanDestoryed && !isDestoryed){
				isPanDestoryed = true;
				destoryPan (collision);
			}
		}
	}

	void DestoryAll(Collision collision){
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
			if(!paoTransform_lod0.gameObject.GetComponent<BoxCollider> ()){
				paoTransform_lod0.gameObject.AddComponent<BoxCollider> ();
				paoTransform_lod0.gameObject.AddComponent<Rigidbody> ();
				paoTransform_lod0.gameObject.GetComponent<Rigidbody> ().mass = 0.9f;
			}
		}

		mainTransform_lod0.parent = null;
		mainTransform_lod1.parent = mainTransform_lod0;
		mainTransform_lod2.parent = mainTransform_lod0;
		if(!mainTransform_lod0.gameObject.GetComponent<Rigidbody>()){
			mainTransform_lod0.gameObject.AddComponent<Rigidbody> ();
			mainTransform_lod0.gameObject.GetComponent<Rigidbody> ().mass = 1.3f;
		}

		for(int i =0;i <= 5; i++){
			Transform lunzi_lod0 = transform_lod0.Find (("lunzi" + (i + 1).ToString()));
			Transform lunzi_lod1 = transform_lod1.Find (("lunzi" + (i + 1).ToString()));
			Transform lunzi_lod2 = transform_lod2.Find (("lunzi" + (i + 1).ToString()));
			lunzi_lod0.parent = null;
			lunzi_lod1.parent = lunzi_lod0;
			lunzi_lod2.parent = lunzi_lod0;
			lunzis [i] = lunzi_lod0;
			if(!lunzi_lod0.gameObject.GetComponent<BoxCollider>()){
				lunzi_lod0.gameObject.AddComponent<BoxCollider> ();
				lunzi_lod0.gameObject.AddComponent<Rigidbody> ();
				lunzi_lod0.gameObject.GetComponent<Rigidbody> ().mass = 0.8f;
			}
		}
		Vector3 explosionPos = new Vector3 (collision.contacts[0].point.x, 
			(collision.contacts[0].point.y - 200), 
			collision.contacts[0].point.z);
		Collider[] colliders = Physics.OverlapSphere(explosionPos, 200.0f);
		foreach (Collider hit in colliders){
			if (hit.GetComponent<Rigidbody>()){ 
				hit.GetComponent<Rigidbody>().AddExplosionForce(30000.0f, explosionPos, 200.0f);
			}  
		}
		Invoke ("destoryAll", 60);
		if(nav != null){
			nav.enabled = false;	
		}
		Transform aim = transform.Find ("aim");
		stop ();
		if(aim != null){
			Destroy (aim.gameObject);
		}

	}

	void destoryPan(Collision collision){
		panTransform_lod0.parent = null;
		panTransform_lod1.parent = panTransform_lod0;
		panTransform_lod2.parent = panTransform_lod0;
		if (!panTransform_lod0.gameObject.GetComponent<Rigidbody> ()) {
			panTransform_lod0.gameObject.AddComponent<Rigidbody> ();
		}
		paoTransform_lod0.parent = null;
		paoTransform_lod1.parent = paoTransform_lod0;
		paoTransform_lod2.parent = paoTransform_lod0;
		if (!paoTransform_lod0.gameObject.GetComponent<BoxCollider> ()) {
			paoTransform_lod0.gameObject.AddComponent<BoxCollider> ();
			paoTransform_lod0.gameObject.AddComponent<Rigidbody> ();
		}

		Vector3 explosionPos = new Vector3 (collision.contacts[0].point.x, 
			(collision.contacts[0].point.y - 10), 
			collision.contacts[0].point.z);
		Collider[] colliders = Physics.OverlapSphere(explosionPos, 50.0f);
		foreach (Collider hit in colliders){
			if (hit.GetComponent<Rigidbody>()){ 
				hit.GetComponent<Rigidbody>().AddExplosionForce(6000.0f, explosionPos, 50.0f);
			}  
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

	void destoryAll(){
		if(panTransform_lod0 != null){
			Destroy (panTransform_lod0.gameObject);
		}
		if(paoTransform_lod0 != null){
			Destroy (paoTransform_lod0.gameObject);
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

	void findInvade(){
		Collider[] colliders = Physics.OverlapSphere (transform.position, 12000);
		for(int i =0; i< colliders.Length; i++ ){
			if(colliders[i].transform.root.childCount <= 0){
				continue;
			}
			Transform tempTransform = colliders[i].transform.root.GetChild (0);
			string tag = tempTransform.tag;
			string[] strs = tempTransform.tag.Split ('_');
			if(strs.Length == 2){
				string tempPlayerId = strs[0];
				string tempType = strs[1];
				if (enemyId.Equals(tempPlayerId)) {
					//Debug.Log ("gqb------>findInvade: " + tag);
					if( target == null && ("car2".Equals(tempType) || "car3".Equals(tempType)
						|| "car4".Equals(tempType) || "car5".Equals(tempType) || "car6".Equals(tempType)) ){
						target = tempTransform.Find("aim");
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

		Dictionary<int, Dictionary<Transform, float>> nearEnemys = null;
		if("0".Equals(playerId)){
			nearEnemys = GameInit.nearEnemys_0;
		}else if("1".Equals(playerId)){
			nearEnemys = GameInit.nearEnemys_1;
		}
		if(nearEnemys.ContainsKey(mPatrolArea.areaId) && nearEnemys [mPatrolArea.areaId].Count > 0){
			Dictionary<Transform, float> tempTransforms = nearEnemys [mPatrolArea.areaId];//----
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
			if(dangeroustEnemy != null && dangeroustEnemy.Find("aim") != null){
				target = dangeroustEnemy.Find("aim");
			}
		}
	}

	void interceptInvade(){
		if(behavior == 1 && target != null){
			float distance = (transform.position - target.position).magnitude;
			float speed = 280;
			if (distance > 5000) {
				speed = distance / 5000 * 40 + 280;
			}
			if(distance < 10000){
				float toCenterDis = (target.position - myCenter).magnitude;
				if ( toCenterDis > 20000 && toCenterDis < 45000) {
					startNav (new Vector3((target.position.x + Random.Range(-3000,3000)),
						height, 
						(target.position.z + Random.Range(-3000,3000))), speed);
				} else{
					startNav (new Vector3(target.position.x, height, target.position.z), speed);
				}

			}
		}
	}

}
