using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportType1 : MonoBehaviour {

	private AudioSource mAudioSource;

	private Transform transform_lod0;
	private Transform transform_lod1;
	private Transform transform_lod2;

	private Animator mAnimator_lod0;
	private Animator mAnimator_lod1;
	private Animator mAnimator_lod2;

	private Transform doorTransform_lod0;
	private Transform doorTransform_lod1;
	private Transform doorTransform_lod2;

	private Renderer mRenderer_lod0_propeller1;
	private Renderer mRenderer_lod0_propeller2;
	private Renderer mRenderer_lod1_propeller1;
	private Renderer mRenderer_lod1_propeller2;
	//private Renderer mRenderer_lod2_door;

	private bool isMoving = false;
	private float openDoorSpeed = 0.4f;
	private float height = 190;
	private float maxMoveSpeed = GameInit.mach * 0.8f;

	private Vector3 park;
	public GameObject navCube;
	public UnityEngine.AI.NavMeshAgent nav;

	public string playerId = "";
	public string type = "";

	public int behavior = 0;//0: no behavior;

	private Vector3[] initPositions = new  Vector3[4];
	//private Vector3[] initRolation = new  Vector3[4];

	// Use this for initialization
	void Start () {
		initPositions [0] = new Vector3 (20000, height, -60000);
		//initPositions [0] = new Vector3 (-5500, height, -60000);
		initPositions [1] = new Vector3 (5500, height, -60000);
		initPositions [2] = new Vector3 (0, height, -50000);
		initPositions [3] = new Vector3 (0, height, -70000);
		/*initRolation [0] = new Vector3 (0, height, 0);
		initRolation [1] = new Vector3 (0, height, 0);
		initRolation [2] = new Vector3 (0, 0, 0);
		initRolation [3] = new Vector3 (0, 180, 0);*/
			
		//transform.position = new Vector3 (transform.position.x, height, transform.position.z);
		int initPositionsId = Random.Range(0,3);
		initPositionsId = 0;
		transform.position = initPositions[initPositionsId];

		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		transform_lod0 = transform.FindChild ("transport_type_lod0");
		transform_lod1 = transform.FindChild ("transport_type_lod1");
		transform_lod2 = transform.FindChild ("transport_type_lod2");

		mAnimator_lod0 = (Animator)transform_lod0.GetComponent<Animator> ();
		mAnimator_lod1 = (Animator)transform_lod1.GetComponent<Animator> ();
		mAnimator_lod2 = (Animator)transform_lod2.GetComponent<Animator> ();

		doorTransform_lod0 = transform_lod0.FindChild ("door");
		doorTransform_lod1 = transform_lod1.FindChild ("door");
		doorTransform_lod2 = transform_lod2.FindChild ("door");

		mRenderer_lod0_propeller1 = transform_lod0.FindChild ("propeller1").GetComponent<Renderer>();
		mRenderer_lod0_propeller2 = transform_lod0.FindChild ("propeller2").GetComponent<Renderer>();
		mRenderer_lod1_propeller1 = transform_lod1.FindChild ("propeller1").GetComponent<Renderer>();
		mRenderer_lod1_propeller2 = transform_lod1.FindChild ("propeller2").GetComponent<Renderer>();
		//mRenderer_lod2_door = doorTransform_lod2.GetComponent<Renderer>();

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
		if (mRenderer_lod0_propeller1.isVisible  && mRenderer_lod0_propeller2.isVisible && !mAnimator_lod0.GetBool ("roll")) {
			mAnimator_lod0.SetBool ("roll", true);
			mAnimator_lod1.SetBool ("roll", false);
		} else if (mRenderer_lod1_propeller1.isVisible  && mRenderer_lod1_propeller2.isVisible && !mAnimator_lod1.GetBool ("roll")) {
			mAnimator_lod1.SetBool ("roll", true);
			mAnimator_lod0.SetBool ("roll", false);
		} else if(!mRenderer_lod0_propeller1.isVisible  && !mRenderer_lod0_propeller2.isVisible
			&& !mRenderer_lod1_propeller1.isVisible  && !mRenderer_lod1_propeller2.isVisible){
			mAnimator_lod0.SetBool ("roll", false);
			mAnimator_lod1.SetBool ("roll", false);
		}
		if(nav != null && !nav.hasPath && !nav.pathPending){
			stop ();
		}
	}

	void run (){
		isMoving = true;
		mAnimator_lod0.SetBool ("roll", true);
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
		float patrol = Random.Range(maxMoveSpeed*0.6f, maxMoveSpeed);
		nav.speed = patrol;
		nav.acceleration = patrol * 1f;
		nav.autoRepath = true;
		//nav.baseOffset = 50;
		nav.angularSpeed = 100;
		Debug.Log ("gqb---->" + nav.radius);
		run ();
	}

	public void createNavCube(){
		navCube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		navCube.transform.position = transform.position;
		navCube.transform.localScale = new Vector3 (500, 500, 500);
		navCube.AddComponent<UnityEngine.AI.NavMeshAgent>();
		transform.parent = navCube.transform;

		MeshRenderer m = navCube.GetComponent<MeshRenderer>();
		//m.enabled = false;
		nav= navCube.GetComponent<UnityEngine.AI.NavMeshAgent> ();
		nav.radius = 10;
		//nav.baseOffset
	}

}
