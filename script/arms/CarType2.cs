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

	private float height = 68.0f;
	private float maxMoveSpeed = GameInit.mach * 0.6f;
	private Vector3 park;
	public GameObject navCube;
	public UnityEngine.AI.NavMeshAgent nav;

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
			park = Util.getIdlePart ("0");
			GameInit.park0 [park] = 1;
			myCenter = new Vector3 (0,0,-60000);
			enemyCenter = new Vector3 (0,0,60000);
			enemyId = "1";
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.251f, 0.647f, 0.78f);
		}else if ("1".Equals (playerId)){
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
		float patrol = Random.Range(maxMoveSpeed*0.5f, maxMoveSpeed);
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
		navCube.transform.position = transform.position;
		navCube.transform.localScale = new Vector3 (200, 200, 200);
		navCube.AddComponent<UnityEngine.AI.NavMeshAgent>();
		Destroy(navCube.GetComponent<BoxCollider> ());
		transform.parent = navCube.transform;

		MeshRenderer m = navCube.GetComponent<MeshRenderer>();
		m.enabled = false;
		nav= navCube.GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	public override void isFired(Collision collision, int type){
		if(type ==2){
			sliderHealth.value = sliderHealth.value - 68;
			if(sliderHealth.value <= 0 && !isDestoryed){
				isDestoryed = true;
				DestoryAll (collision, 120000.0f);
				return;
			}
		}
	}

	void DestoryAll(Collision collision, float power){
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
		Vector3 explosionPos = new Vector3 (collision.contacts[0].point.x, 
			(collision.contacts[0].point.y - 300), 
			collision.contacts[0].point.z);
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

		if(GameInit.currentInstance.ContainsKey((string)tag)){
			GameInit.currentInstance[tag] = (int)GameInit.currentInstance[tag] - 1;
		}
	}

	void destoryAll(){
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
