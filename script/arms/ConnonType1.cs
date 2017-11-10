using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnonType1 : MonoBehaviour {

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

	private float aimSpeed = 1.0f;
	private float aimMaxVer = 40.0f;
	private float aimMinVer = 6.0f;

	//test
	private GameObject target;

	// Use this for initialization
	void Start () {
		
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

}
