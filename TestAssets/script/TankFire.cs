using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankFire : PojulObject {

	private Transform fireTransform;

	private float height = 60.0f;

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

	private float aimSpeed = 10.0f;
	private float aimMaxVer = 10.0f;

	private bool isMoving = false;
	private float maxMoveSpeed = GameInit.mach * 0.4f;

	//test
	public GameObject target;
	private float fireInterval = 5f;
	private float lastFileTime = 0.0f;

	// Use this for initialization
	void Start () {

		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		transform_lod0 = transform.Find ("car_type3_lod0");
		transform_lod1 = transform.Find ("car_type3_lod1");
		transform_lod2 = transform.Find ("car_type3_lod2");

		mAnimator_lod0 = (Animator)transform_lod0.GetComponent<Animator> ();
		mAnimator_lod1 = (Animator)transform_lod1.GetComponent<Animator> ();
		mAnimator_lod2 = (Animator)transform_lod2.GetComponent<Animator> ();
		mAnimator_lod0.SetBool ("roll", false);
		mAnimator_lod1.SetBool ("roll", false);
		mAnimator_lod2.SetBool ("roll", false);

		panTransform_lod0 = transform_lod0.Find("pan");
		panTransform_lod1 = transform_lod1.Find("pan");
		panTransform_lod2 = transform_lod2.Find("pan");

		mRenderer_lod0_pan = panTransform_lod0.GetComponent<Renderer>();
		mRenderer_lod1_pan = panTransform_lod1.GetComponent<Renderer>();
		mRenderer_lod2_pan = panTransform_lod2.GetComponent<Renderer>();

		paoTransform_lod0 = panTransform_lod0.Find ("pao");
		paoTransform_lod1 = panTransform_lod1.Find ("pao");
		paoTransform_lod2 = panTransform_lod2.Find ("pao");

		fireTransform = transform.Find ("car_type3_lod0").Find ("pan").Find("pao").Find("fire");

		//test
		target = GameObject.FindGameObjectWithTag("player");
		run ();

		//InvokeRepeating ("fire", 22, 22);

		//InvokeRepeating ("bomb", 23, 23);

		InvokeRepeating ("rayCastEnemy", 0.5f, 0.5f);

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

	void bomb(){
		Debug.Log ("gqb------>bomb");

		GameObject bomb = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bomb1"), 
			new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
		bomb.tag = "bomb1";

		bomb.transform.position = transform.position;
		bomb.transform.rotation = transform.rotation;
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
		//shell1.transform.rotation = fireTransform.rotation;
		shell1.tag = "shell1";
		((ShellType1)shell1.GetComponent<ShellType1> ()).shoot(10000, 0, 0);

		/*GameObject bomb2 = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/bomb2"), 
			new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
		bomb2.tag = "bomb2";*/

		//bomb2.transform.position = hit.point;
		//bomb2.transform.parent = hit.transform;
	}

	// Update is called once per frame
	void Update () {

		//Ray ray = new Ray (fireTransform.position, fireTransform.localEulerAngles);
		/*Ray ray = new Ray (fireTransform.position, Vector3.forward);
		RaycastHit hit;
		Physics.Raycast (ray, out hit);
		Debug.DrawLine (fireTransform.position, fireTransform.position +  fireTransform.rotation.eulerAngles*300, Color.red);*/

		//Debug.Log (fireTransform.transform.forward + "gqb------>fire" + fireTransform.rotation.eulerAngles);

		//Debug.DrawRay(paoTransform_lod0.position, paoTransform_lod0.forward*12000, Color.white);

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

	void run(){
		isMoving = true;
		if(mAudioSource != null && !mAudioSource.isPlaying){
			mAudioSource.Play ();
		}
	}
		
	public override void isFired(RaycastHit hit, Collision collision, int type){
	}

}
