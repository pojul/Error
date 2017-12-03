using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarType3 : PojulObject {

	private float height = 60.0f;

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

	private float aimSpeed = 10.0f;
	private float aimMaxVer = 10.0f;

	private bool isMoving = false;
	private float maxMoveSpeed = GameInit.mach * 0.4f;

	//test
	private Transform target;
	private float fireInterval = 5.0f;
	private float lastFileTime = 0.0f;

	public GameObject navCube;
	public UnityEngine.AI.NavMeshAgent nav;

	private RadiusArea mPatrolArea;

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

		//healthTranssform = transform.FindChild ("health");

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
			transform.position = new Vector3 (0,
				25, 
				-10000);
			/*transform.position = new Vector3 (-40000,
				25, 
				-51000);*/
		}

		RectTransform mRectTransform = sliderHealth.GetComponent<RectTransform> ();
		mRectTransform.sizeDelta = new Vector2 (Screen.width / 18, Screen.width / 60);
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
		InvokeRepeating ("interceptInvade", 5.0f, 5.0f);
		//startNav (new Vector3 (3340, 60, -34925));
		//startNav (new Vector3 (0, 60, 0));
	}

	// Update is called once per frame
	void Update () {
		sliderHealth.transform.rotation = Quaternion.Euler(mainTransform_lod0.rotation.eulerAngles.x , 
			Camera.main.transform.rotation.eulerAngles.y,
			mainTransform_lod0.rotation.eulerAngles.z);

		//Debug.DrawRay(paoTransform_lod0.position, paoTransform_lod0.forward*12000, Color.white);
		if(!isPanDestoryed){
			aimEnemy (target);
		}
			
		if(isMoving){
			listenerRollAni ();
		}

	}

	void behaviorListener(){
		rayCastEnemy ();

		if(nav != null && behavior == 1  && 
			(nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid || nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial) ){
			startNav(mPatrolArea.getRandomPoint());
		}


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
			if(hit.transform != null && (Time.time - lastFileTime) > fireInterval && hit.transform.root.childCount > 0){
				//Debug.Log (hit.transform.root.GetChild(0).name + "; gqb------>hit:" + hit.transform.root.GetChild(0).tag);
				string[] tags = hit.transform.root.GetChild(0).tag.Split ('_');
				if (tags.Length == 2 && enemyId.Equals(tags [0]) && ("car2".Equals (tags [1]) || "car3".Equals (tags [1])
				   || "car4".Equals (tags [1]) || "car5".Equals (tags [1]) || "car6".Equals (tags [1]))) {
					lastFileTime = Time.time;
					fire ();
				}
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
			(paoTransform_lod0.position + paoTransform_lod0.forward*30), paoTransform_lod0.rotation) as GameObject;
		shell1.tag = "shell1";
		((ShellType1)shell1.GetComponent<ShellType1> ()).shoot(10000, 0);
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
		Destroy(navCube.GetComponent<BoxCollider> ());
		transform.parent = navCube.transform;

		MeshRenderer m = navCube.GetComponent<MeshRenderer>();
		m.enabled = false;
		nav= navCube.GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	public override void isFired(Collision collision, int type){
		if(type ==2){
			sliderHealth.value = sliderHealth.value - 36;
			if(sliderHealth.value <= 0 && !isDestoryed){
				isDestoryed = true;
				isPanDestoryed = true;
				DestoryAll (collision);
				return;
			}
			if(collision.gameObject.name.Equals("pan") && !isPanDestoryed && !isDestoryed){
				isPanDestoryed = true;
				destoryPan (collision);
			}
		}else if(type ==3){
			isDestoryed = true;
			isPanDestoryed = true;
			DestoryAll (collision);
			return;
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
			lunzis [i] = lunzi_lod0;
			if(!lunzi_lod0.gameObject.GetComponent<MeshCollider>()){
				lunzi_lod0.gameObject.AddComponent<MeshCollider> ();
				lunzi_lod0.gameObject.GetComponent<MeshCollider> ().convex = true;
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
		Invoke ("destoryAll", 30);
		if(nav != null){
			nav.enabled = false;	
		}
		stop ();
		Transform aim = transform.FindChild ("aim");
		if(aim != null){
			Destroy (aim.gameObject);
		}

		if(GameInit.currentInstance.ContainsKey((string)tag)){
			GameInit.currentInstance[tag] = (int)GameInit.currentInstance[tag] - 1;
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
		if (!paoTransform_lod0.gameObject.GetComponent<MeshCollider> ()) {
			paoTransform_lod0.gameObject.AddComponent<MeshCollider> ();
			paoTransform_lod0.gameObject.GetComponent<MeshCollider> ().convex = true;
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
		for(int i=0;i<= 7;i++){
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
						target = tempTransform.FindChild("aim");
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
