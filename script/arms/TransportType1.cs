using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportType1 : PojulObject {

	private AudioSource mAudioSource;

	private Transform transform_lod0;
	private Transform transform_lod1;
	private Transform transform_lod2;

	//private Animator mAnimator_lod0;
	//private Animator mAnimator_lod1;
	//private Animator mAnimator_lod2;

	private Transform doorTransform_lod0;
	private Transform doorTransform_lod1;
	private Transform doorTransform_lod2;

	private Transform propeller1Transform_lod0;
	private Transform propeller2Transform_lod0;
	private Transform propeller1Transform_lod1;
	private Transform propeller2Transform_lod1;
	private Transform propeller1Transform_lod2;
	private Transform propeller2Transform_lod2;

	/*private Renderer mRenderer_lod0_propeller1;
	/*private Renderer mRenderer_lod0_propeller2;
	private Renderer mRenderer_lod1_propeller1;
	private Renderer mRenderer_lod1_propeller2;
	private Renderer mRenderer_lod0_propeller1;
	private Renderer mRenderer_lod0_propeller2;*/
	//private Renderer mRenderer_lod2_door;

	private bool isMoving = false;
	private float openDoorSpeed = 0.4f;
	private int initHeight = 2500;
	private float height = 84; //190;
	private float maxMoveSpeed = GameInit.mach * 0.6f;
	private float aniSpeed = 0.0f;
	private float navPathDistance = -1f;
	private float maxFlyHeight = 9000.0f;
	private float minFlyHeight = 7000.0f;
	//private float flyHeight = 190.0f;
	private float flyHeight = 0f;
	private float flyHeightSpeed = 480f;

	private Vector3 park;
	public GameObject navCube;
	public UnityEngine.AI.NavMeshAgent nav;

	void Start () {
		transform.position = new Vector3 (transform.position.x, initHeight, transform.position.z);

		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		transform_lod0 = transform.FindChild ("transport_type_lod0");
		transform_lod1 = transform.FindChild ("transport_type_lod1");
		transform_lod2 = transform.FindChild ("transport_type_lod2");

		//mAnimator_lod0 = (Animator)transform_lod0.GetComponent<Animator> ();
		//mAnimator_lod1 = (Animator)transform_lod1.GetComponent<Animator> ();
		//mAnimator_lod2 = (Animator)transform_lod2.GetComponent<Animator> ();

		doorTransform_lod0 = transform_lod0.FindChild ("door");
		doorTransform_lod1 = transform_lod1.FindChild ("door");
		doorTransform_lod2 = transform_lod2.FindChild ("door");


		propeller1Transform_lod0 = transform_lod0.FindChild ("propeller1");
		propeller2Transform_lod0 = transform_lod0.FindChild ("propeller2");
		propeller1Transform_lod1 = transform_lod1.FindChild ("propeller1");
		propeller2Transform_lod1 = transform_lod1.FindChild ("propeller2");
		propeller1Transform_lod2 = transform_lod2.FindChild ("propeller1");
		propeller2Transform_lod2 = transform_lod2.FindChild ("propeller2");

		/*mRenderer_lod0_propeller1 = propeller1Transform_lod0.GetComponent<Renderer>();
		mRenderer_lod0_propeller2 = propeller2Transform_lod0.GetComponent<Renderer>();
		mRenderer_lod1_propeller1 = propeller1Transform_lod1.GetComponent<Renderer>();
		mRenderer_lod1_propeller2 = propeller2Transform_lod1.GetComponent<Renderer>();*/
		//mRenderer_lod2_door = doorTransform_lod2.GetComponent<Renderer>();
		createNavCube();

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

		//startNav (park);

	}
	
	// Update is called once per frame
	void Update () {
		
		if(isMoving){
			listenerRollAni ();
		}
		//openDoor ();
		//closeDoor ();

	}

	void openDoor(){
		
		if(doorTransform_lod0.localEulerAngles.x > 301 || doorTransform_lod0.localEulerAngles.x < 1){
			doorTransform_lod0.rotation = Quaternion.Slerp(doorTransform_lod0.rotation, 
				Quaternion.Euler(new Vector3(300, doorTransform_lod0.localEulerAngles.y, doorTransform_lod0.localEulerAngles.z)), openDoorSpeed * Time.deltaTime);
			doorTransform_lod1.localEulerAngles = new Vector3(doorTransform_lod0.localEulerAngles.x, doorTransform_lod0.localEulerAngles.y, doorTransform_lod0.localEulerAngles.z);
			doorTransform_lod2.localEulerAngles = new Vector3(doorTransform_lod0.localEulerAngles.x, doorTransform_lod0.localEulerAngles.y, doorTransform_lod0.localEulerAngles.z);
		}
	}

	void closeDoor(){
		if(doorTransform_lod0.localEulerAngles.x < 359 && doorTransform_lod0.localEulerAngles.x > 1){
			doorTransform_lod0.rotation = Quaternion.Slerp(doorTransform_lod0.rotation, 
				Quaternion.Euler(new Vector3(0, doorTransform_lod0.localEulerAngles.y, doorTransform_lod0.localEulerAngles.z)), openDoorSpeed * Time.deltaTime);
			doorTransform_lod1.localEulerAngles = new Vector3(doorTransform_lod0.localEulerAngles.x, doorTransform_lod0.localEulerAngles.y, doorTransform_lod0.localEulerAngles.z);
			doorTransform_lod2.localEulerAngles = new Vector3(doorTransform_lod0.localEulerAngles.x, doorTransform_lod0.localEulerAngles.y, doorTransform_lod0.localEulerAngles.z);
		}
	}

	void listenerRollAni(){
		float posY = transform.position.y;
		if (Mathf.Abs (flyHeight - transform.position.y +initHeight) > 10) {
			posY = posY + flyHeightSpeed * Time.deltaTime;
		}
		transform.position = new Vector3 (navCube.transform.position.x, 
			posY,
			navCube.transform.position.z);
		transform.rotation = navCube.transform.rotation;

		propeller1Transform_lod0.localEulerAngles = new Vector3(propeller1Transform_lod0.localEulerAngles.x, 
			(propeller1Transform_lod0.localEulerAngles.y + aniSpeed * Time.deltaTime), propeller1Transform_lod0.localEulerAngles.z);
		
		propeller2Transform_lod0.localEulerAngles = new Vector3(propeller1Transform_lod0.localEulerAngles.x, 
			propeller1Transform_lod0.localEulerAngles.y, propeller1Transform_lod0.localEulerAngles.z);
		
		propeller1Transform_lod1.localEulerAngles = new Vector3(propeller1Transform_lod0.localEulerAngles.x, 
			propeller1Transform_lod0.localEulerAngles.y, propeller1Transform_lod0.localEulerAngles.z);
		
		propeller2Transform_lod1.localEulerAngles = new Vector3(propeller1Transform_lod0.localEulerAngles.x, 
			propeller1Transform_lod0.localEulerAngles.y, propeller1Transform_lod0.localEulerAngles.z);

		propeller1Transform_lod2.localEulerAngles = new Vector3(propeller1Transform_lod0.localEulerAngles.x, 
			propeller1Transform_lod0.localEulerAngles.y, propeller1Transform_lod0.localEulerAngles.z);

		propeller2Transform_lod2.localEulerAngles = new Vector3(propeller1Transform_lod0.localEulerAngles.x, 
			propeller1Transform_lod0.localEulerAngles.y, propeller1Transform_lod0.localEulerAngles.z);

		if(nav != null && !nav.hasPath && !nav.pathPending){
			if (navPathDistance < 0) {
				startMove ();
			} else if(Mathf.Abs (flyHeight - transform.position.y) < 10){
				transform.position = new Vector3 (navCube.transform.position.x, 
					height,
					navCube.transform.position.z);
				endMove ();
				//stop ();
			}
		}else if(nav != null && nav.hasPath && !nav.pathPending && navPathDistance > 0){
			if((nav.remainingDistance / navPathDistance) < 0.3){
				flyHeightSpeed = -Mathf.Abs (flyHeightSpeed);
				flyHeight = height;
			}
		}
	}

	void startMove(){
		//2000
		if (aniSpeed < 1000) {
			aniSpeed = aniSpeed + 2f;
			mAudioSource.volume = aniSpeed * 0.0007f;
		} else {
			startNav (park);
		}
	}

	void endMove(){
		if (aniSpeed > 2) {
			aniSpeed = aniSpeed - 2f;
			mAudioSource.volume = aniSpeed * 0.0007f;
		} else {
			navPathDistance = -1;
			isMoving = false;
			aniSpeed = 0.0f;
			if(mAudioSource != null && mAudioSource.isPlaying){
				mAudioSource.volume = aniSpeed;
				mAudioSource.Stop ();
			}
		}
	}

	void run (){
		isMoving = true;
		aniSpeed = 0.0f;
		//flyHeight = 190;
		//mAnimator_lod0.speed = 4;
		//mAnimator_lod1.speed = 4;
		//mAnimator_lod0.SetBool ("roll", true);
		if(mAudioSource != null && !mAudioSource.isPlaying){
			mAudioSource.volume = aniSpeed;
			mAudioSource.Play ();
		}
	}

	public void startNav(Vector3 navPoint){
		if(navCube == null){
			createNavCube ();
		}
		nav.destination = navPoint;//target.transform.position;
		//float patrol = Random.Range(maxMoveSpeed*0.6f, maxMoveSpeed);
		nav.speed = maxMoveSpeed;
		nav.acceleration = maxMoveSpeed ;
		nav.autoRepath = true;
		//nav.baseOffset = 50;
		nav.angularSpeed = 20;
		navPathDistance = Mathf.Abs((new Vector3(transform.position.x, 0, transform.position.z) - park).magnitude);
		flyHeight = Random.Range (minFlyHeight, maxFlyHeight);
		flyHeightSpeed = Mathf.Abs (flyHeightSpeed);
		initHeight = 0;
	}

	public void createNavCube(){
		navCube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		navCube.transform.position = new Vector3(transform.position.x, height, transform.position.z);
		navCube.transform.localScale = new Vector3 (500, 500, 500);
		navCube.AddComponent<UnityEngine.AI.NavMeshAgent>();
		//transform.parent = navCube.transform;

		MeshRenderer m = navCube.GetComponent<MeshRenderer>();
		m.enabled = false;
		nav= navCube.GetComponent<UnityEngine.AI.NavMeshAgent> ();
		//nav.baseOffset
	}

}
