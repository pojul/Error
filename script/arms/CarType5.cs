using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarType5 : PojulObject {

	private AudioSource mAudioSource;

	private Transform transform_lod0;
	private Transform transform_lod1;
	private Transform transform_lod2;

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
	private float height = 69.0f;
	private float maxMoveSpeed = GameInit.mach * 0.36f;

	public GameObject navCube;
	public UnityEngine.AI.NavMeshAgent nav;

	public string playerId = "";
	public string type = "";

	public int behavior = 0;
	private RadiusArea mPatrolArea;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (transform.position.x, height, transform.position.z);

		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		transform_lod0 = transform.FindChild ("car_type5_lod0");
		transform_lod1 = transform.FindChild ("car_type5_lod1");
		transform_lod2 = transform.FindChild ("car_type5_lod2");

		mAnimator_lod0 = (Animator)transform_lod0.GetComponent<Animator> ();
		mAnimator_lod1 = (Animator)transform_lod1.GetComponent<Animator> ();

		mRenderer_lod0_lunzi1 = transform_lod0.FindChild("lunzi1").GetComponent<Renderer>();
		mRenderer_lod1_lunzi1 = transform_lod1.FindChild("lunzi1").GetComponent<Renderer>();

		paoTransform_lod0 = transform_lod0.FindChild ("pao");
		paoTransform_lod1 = transform_lod1.FindChild ("pao");
		paoTransform_lod2 = transform_lod2.FindChild ("pao");


		run ();

		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];

		int areaId = -1;
		if ("0".Equals (playerId)) {
			areaId = Util.getCar5AreaId ("0");
			GameInit.Car5Area0.Add (areaId, gameObject);
		} else if ("1".Equals (playerId)) {
			areaId = Util.getCar5AreaId ("1");
			GameInit.Car5Area1.Add (areaId, gameObject);
		}
		mPatrolArea = new RadiusArea (areaId);
		mPatrolArea.maxRange = 40000;
		mPatrolArea.minRange = 20000;
		startNav(mPatrolArea.getRandomPoint());
	}
	
	// Update is called once per frame
	void Update () {

		if (readyLaunch) {
			radyLaunch ();
		} else {
			unRadyLaunch ();
		}

		if(isMoving){
			listenerRollAni ();
		}
	}

	void radyLaunch(){
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
		if(paoTransform_lod0.localEulerAngles.x > 1 && paoTransform_lod0.localEulerAngles.x < 359){
			paoTransform_lod0.localEulerAngles = new Vector3((paoTransform_lod0.localEulerAngles.x + aimSpeed * Time.deltaTime), 
				paoTransform_lod0.localEulerAngles.y, paoTransform_lod0.localEulerAngles.z);
			paoTransform_lod1.localEulerAngles = new Vector3(paoTransform_lod0.localEulerAngles.x, paoTransform_lod0.localEulerAngles.y, paoTransform_lod0.localEulerAngles.z);
			paoTransform_lod2.localEulerAngles = new Vector3(paoTransform_lod0.localEulerAngles.x, paoTransform_lod0.localEulerAngles.y, paoTransform_lod0.localEulerAngles.z);
		}
	}

	void listenerRollAni(){
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
		nav.destination = navPoint;//target.transform.position;
		float patrol = Random.Range(maxMoveSpeed*0.5f, maxMoveSpeed);
		nav.speed = patrol;
		nav.acceleration = patrol * 2f;
		nav.autoRepath = true;
		//nav.baseOffset = 50;
		nav.angularSpeed = 100;
		readyLaunch = false;
		run ();
	}

	public void createNavCube(){
		navCube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		navCube.transform.position = transform.position;
		navCube.transform.localScale = new Vector3 (200, 200, 200);
		navCube.AddComponent<UnityEngine.AI.NavMeshAgent>();
		transform.parent = navCube.transform;

		MeshRenderer m = navCube.GetComponent<MeshRenderer>();
		m.enabled = false;
		nav= navCube.GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

}
