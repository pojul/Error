using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarType6 : MonoBehaviour {

	private AudioSource mAudioSource;

	private Transform transform_lod0;
	private Transform transform_lod1;
	private Transform transform_lod2;

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

	private float height = 70.0f;
	private float aimSpeed = 1.0f;
	private float aimMaxVer = 10.0f;
	private float maxMoveSpeed = GameInit.mach * 0.4f;

	private bool isMoving = false;

	public GameObject navCube;
	public UnityEngine.AI.NavMeshAgent nav;

	public string playerId = "";
	public string type = "";

	public int behavior = 1;//1: patrol;
	private RadiusArea mPatrolArea;

	//test
	private GameObject target;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (transform.position.x, height, transform.position.z);

		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		transform_lod0 = transform.FindChild ("car_type6_lod0");
		transform_lod1 = transform.FindChild ("car_type6_lod1");
		transform_lod2 = transform.FindChild ("car_type6_lod2");

		mAnimator_lod0 = (Animator)transform_lod0.GetComponent<Animator> ();
		mAnimator_lod1 = (Animator)transform_lod1.GetComponent<Animator> ();
		mAnimator_lod2 = (Animator)transform_lod2.GetComponent<Animator> ();

		panTransform_lod0 = transform_lod0.FindChild("pan");
		panTransform_lod1 = transform_lod1.FindChild("pan");
		panTransform_lod2 = transform_lod2.FindChild("pan");

		mRenderer_lod0_pan = panTransform_lod0.GetComponent<Renderer>();
		mRenderer_lod1_pan = panTransform_lod1.GetComponent<Renderer>();
		mRenderer_lod2_pan = panTransform_lod2.GetComponent<Renderer>();

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

	}
	
	// Update is called once per frame
	void Update () {

		if(target == null){
			target = GameObject.FindGameObjectWithTag("player");
		}else{
			aimEnemy (target.transform);
		}

		if(isMoving){
			listenerRollAni ();
		}

	}

	void behaviorListener(){
		if(nav != null && behavior == 1 && !nav.hasPath && !nav.pathPending && mPatrolArea != null){
			startNav(mPatrolArea.getRandomPoint());
		}
	}


	void aimEnemy(Transform enemyTransform){
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
	}

	public void createNavCube(){
		navCube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		navCube.transform.position = transform.position;
		navCube.transform.localScale = new Vector3 (100, 100, 100);
		navCube.AddComponent<UnityEngine.AI.NavMeshAgent>();
		transform.parent = navCube.transform;

		MeshRenderer m = navCube.GetComponent<MeshRenderer>();
		m.enabled = false;
		nav= navCube.GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

}
