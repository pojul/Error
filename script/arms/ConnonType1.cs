using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnonType1 : PojulObject {

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

	private float height = 42.0f;
	private float aimSpeed = 1.0f;
	private float aimMaxVer = 40.0f;
	private float aimMinVer = 6.0f;
	private float maxMoveSpeed = GameInit.mach * 0.3f;

	private Vector3 park;
	public GameObject navCube;
	public UnityEngine.AI.NavMeshAgent nav;

	//test
	private GameObject target;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (transform.position.x, height, transform.position.z);

		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		transform_lod0 = transform.FindChild ("cannon_type1_lod0");
		transform_lod1 = transform.FindChild ("cannon_type1_lod1");
		transform_lod2 = transform.FindChild ("cannon_type1_lod2");

		mAnimator_lod0 = (Animator)transform_lod0.GetComponent<Animator> ();
		mAnimator_lod1 = (Animator)transform_lod1.GetComponent<Animator> ();

		mRenderer_lod0_lunzi1 = transform_lod0.FindChild("lunzi1").GetComponent<Renderer>();
		mRenderer_lod1_lunzi1 = transform_lod1.FindChild("lunzi1").GetComponent<Renderer>();

		paoTransform_lod0 = transform_lod0.FindChild ("pao");
		paoTransform_lod1 = transform_lod1.FindChild ("pao");
		paoTransform_lod2 = transform_lod2.FindChild ("pao");

		//test
		target = GameObject.FindGameObjectWithTag("player");
		run ();

		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];

		if ("0".Equals (playerId)) {
			park = Util.getIdlePart ("0");
			GameInit.park0 [park] = 1;
		}else if ("1".Equals (playerId)){
			park = Util.getIdlePart ("1");
			GameInit.park1 [park] = 1;
		}
		startNav (park);

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

	void aimEnemy(Transform enemyTransform){
		paoTransform_lod0.rotation = Quaternion.Slerp(paoTransform_lod0.rotation, 
			Quaternion.LookRotation(enemyTransform.position - paoTransform_lod0.position), aimSpeed * Time.deltaTime);
		
		float paoAnglesX_lod0 = paoTransform_lod0.localEulerAngles.x;

		if(paoAnglesX_lod0 > aimMinVer && paoAnglesX_lod0 <= 180){
			paoAnglesX_lod0 = aimMinVer;
		}else if(paoAnglesX_lod0 > 180 && paoAnglesX_lod0 < (360 - aimMaxVer)){
			paoAnglesX_lod0 = 360 - aimMaxVer;
		}
		paoTransform_lod0.localEulerAngles = new Vector3(paoAnglesX_lod0, paoTransform_lod0.localEulerAngles.y, 0);
		paoTransform_lod1.localEulerAngles = new Vector3(paoAnglesX_lod0, paoTransform_lod0.localEulerAngles.y, 0);
		paoTransform_lod2.localEulerAngles = new Vector3(paoAnglesX_lod0, paoTransform_lod0.localEulerAngles.y, 0);

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
