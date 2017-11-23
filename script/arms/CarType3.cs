using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarType3 : MonoBehaviour {

	private float height = 60.0f;

	private AudioSource mAudioSource;

	private Transform transform_lod0;
	private Transform transform_lod1;
	private Transform transform_lod2;

	private Transform fireTransform;

	private Animator mAnimator_lod0;
	private Animator mAnimator_lod1;
	private Animator mAnimator_lod2;

	private Transform panTransform_lod0;
	private Transform panTransform_lod1;
	private Transform panTransform_lod2;

	private Renderer mRenderer_lod0_main;
	private Renderer mRenderer_lod1_main;
	private Renderer mRenderer_lod2_main;

	private Transform paoTransform_lod0;
	private Transform paoTransform_lod1;
	private Transform paoTransform_lod2;

	private float aimSpeed = 1.0f;
	private float aimMaxVer = 10.0f;

	private bool isPanDestoryed = false;
	private bool isMoving = false;
	private float maxMoveSpeed = GameInit.mach * 0.4f;

	//test
	private GameObject target;
	private float fireInterval = 12.0f;
	private float lastFileTime = 0.0f;

	public GameObject navCube;
	public UnityEngine.AI.NavMeshAgent nav;

	public string playerId = "";
	public string type = "";
	public string currentCoordinate = "";

	public int behavior = 1;//1: patrol;
	private RadiusArea mPatrolArea;

	// Use this for initialization
	void Start () {

		transform.position = new Vector3 (transform.position.x, height, transform.position.z);

		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		transform_lod0 = transform.FindChild ("car_type3_lod0");
		transform_lod1 = transform.FindChild ("car_type3_lod1");
		transform_lod2 = transform.FindChild ("car_type3_lod2");

		fireTransform = transform.FindChild ("car_type3_lod0").FindChild ("pan").FindChild("pao").FindChild("fire");

		mAnimator_lod0 = (Animator)transform_lod0.GetComponent<Animator> ();
		mAnimator_lod1 = (Animator)transform_lod1.GetComponent<Animator> ();
		mAnimator_lod2 = (Animator)transform_lod2.GetComponent<Animator> ();
		mAnimator_lod0.SetBool ("roll", false);
		mAnimator_lod1.SetBool ("roll", false);
		mAnimator_lod2.SetBool ("roll", false);

		panTransform_lod0 = transform_lod0.FindChild("pan");
		panTransform_lod1 = transform_lod1.FindChild("pan");
		panTransform_lod2 = transform_lod2.FindChild("pan");

		mRenderer_lod0_main = transform_lod0.FindChild("main").GetComponent<Renderer>();
		mRenderer_lod1_main = transform_lod1.FindChild("main").GetComponent<Renderer>();
		mRenderer_lod2_main = transform_lod2.FindChild("main").GetComponent<Renderer>();

		paoTransform_lod0 = panTransform_lod0.FindChild ("pao");
		paoTransform_lod1 = panTransform_lod1.FindChild ("pao");
		paoTransform_lod2 = panTransform_lod2.FindChild ("pao");

		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];

		if("0".Equals(playerId)){
			mPatrolArea = new RadiusArea (Random.Range(1,4));
		}else if("1".Equals(playerId)){
			mPatrolArea = new RadiusArea (Random.Range(5,8));
		}

		//test
		target = GameObject.FindGameObjectWithTag("player");
		run ();

		createNavCube ();

		InvokeRepeating("behaviorListener", 0.5f, 0.5f);
		InvokeRepeating ("updateCoordinate", 2.0f, 2.0f);
		//startNav (new Vector3 (3340, 60, -34925));
		//startNav (new Vector3 (0, 60, 0));
	}
	
	// Update is called once per frame
	void Update () {

		if(!isPanDestoryed){
			aimEnemy (target.transform);
		}
			
		if(isMoving){
			listenerRollAni ();
		}

	}

	void behaviorListener(){
		//if(nav != null){
			
		//}

		rayCastEnemy ();

		if(nav != null && behavior == 1 && !nav.hasPath && !nav.pathPending && mPatrolArea != null){
			startNav(mPatrolArea.getRandomPoint());
			//Debug.Log (nav.hasPath + "------>" + nav.isStopped + " ;" + nav.pathStatus + " ;" + nav.pathPending +　" ;" + nav.isPathStale);
		}
	}

	void rayCastEnemy(){
		if(target == null){
			return;
		}
		RaycastHit hit;
		if(Physics.Raycast (paoTransform_lod0.position, paoTransform_lod0.forward, out hit, 12000.0f)){
			//Debug.Log (paoTransform_lod0.forward + "gqb------>hit:" + hit.transform.root.name);
			if(hit.transform != null && hit.transform.root.name != "Plane" && (Time.time - lastFileTime) > fireInterval){
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
		((ShellType1)shell1.GetComponent<ShellType1> ()).shoot(10000, 250);
	}

    void aimEnemy(Transform enemyTransform){
		if(target == null){
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
		nav.destination = navPoint;//target.transform.position;
		float patrol = Random.Range(maxMoveSpeed*0.5f, maxMoveSpeed);
		nav.speed = patrol;
		nav.acceleration = patrol * 2f;
		nav.autoRepath = true;
		//nav.baseOffset = 50;
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

	/*public override void isFired(Collision collision, int type){
		if(type ==2){
			if(collision.gameObject.name.Equals("pan") && !isPanDestoryed){
				destoryPan (collision);
			}
		}
	}*/

	void destoryPan(Collision collision){
		panTransform_lod0.parent = null;
		panTransform_lod1.parent = panTransform_lod0;
		panTransform_lod2.parent = panTransform_lod0;
		panTransform_lod0.gameObject.AddComponent<Rigidbody> ();

		paoTransform_lod0.parent = null;
		paoTransform_lod1.parent = paoTransform_lod0;
		paoTransform_lod2.parent = paoTransform_lod0;
		paoTransform_lod0.gameObject.AddComponent<BoxCollider> ();
		paoTransform_lod0.gameObject.AddComponent<Rigidbody> ();

		Vector3 explosionPos = new Vector3 (collision.contacts[0].point.x, 
			(collision.contacts[0].point.y - 10), 
			collision.contacts[0].point.z);
		Collider[] colliders = Physics.OverlapSphere(explosionPos, 50.0f);
		foreach (Collider hit in colliders){
			if (hit.GetComponent<Rigidbody>()){ 
				hit.GetComponent<Rigidbody>().AddExplosionForce(5000.0f, explosionPos, 50.0f);
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
		isPanDestoryed = true;
	}

	void updateCoordinate(){
		currentCoordinate = Util.updateCoordinate (transform, currentCoordinate);
	}

}
