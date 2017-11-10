using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarType5 : MonoBehaviour {

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
	private bool readyLaunch = false;

	private float aimSpeed = 0.2f;

	// Use this for initialization
	void Start () {

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
		readyLaunch = true;

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
			paoTransform_lod0.rotation = Quaternion.Slerp(paoTransform_lod0.rotation, 
				Quaternion.Euler(new Vector3(270, paoTransform_lod0.localEulerAngles.y, paoTransform_lod0.localEulerAngles.z)), aimSpeed * Time.deltaTime);
			paoTransform_lod1.localEulerAngles = new Vector3(paoTransform_lod0.localEulerAngles.x, paoTransform_lod0.localEulerAngles.y, paoTransform_lod0.localEulerAngles.z);
			paoTransform_lod2.localEulerAngles = new Vector3(paoTransform_lod0.localEulerAngles.x, paoTransform_lod0.localEulerAngles.y, paoTransform_lod0.localEulerAngles.z);
		}

		if(paoTransform_lod0.localEulerAngles.x <= 271 ){
			readyLaunch = false;
		}

	}

	void unRadyLaunch(){
		if(paoTransform_lod0.localEulerAngles.x > 1 && paoTransform_lod0.localEulerAngles.x < 359){
			paoTransform_lod0.rotation = Quaternion.Slerp(paoTransform_lod0.rotation, 
				Quaternion.Euler(new Vector3(0, paoTransform_lod0.localEulerAngles.y, paoTransform_lod0.localEulerAngles.z)), aimSpeed * Time.deltaTime);
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
