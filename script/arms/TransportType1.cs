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

	private Renderer mRenderer_lod0_door;
	private Renderer mRenderer_lod1_door;
	//private Renderer mRenderer_lod2_door;

	private bool isMoving = false;
	private float openDoorSpeed = 0.4f;

	// Use this for initialization
	void Start () {
		
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

		mRenderer_lod0_door = doorTransform_lod0.GetComponent<Renderer>();
		mRenderer_lod1_door = doorTransform_lod1.GetComponent<Renderer>();
		//mRenderer_lod2_door = doorTransform_lod2.GetComponent<Renderer>();

		run ();

	}
	
	// Update is called once per frame
	void Update () {
		
		if(isMoving){
			listenerRollAni ();
		}

		openDoor ();
		//closeDoor ();
	}

	void openDoor(){
		
		if(doorTransform_lod0.localEulerAngles.x > 301 || doorTransform_lod0.localEulerAngles.x < 1){
			Debug.Log ("gqb------>" + doorTransform_lod0.localEulerAngles.x);
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
		if (mRenderer_lod0_door.isVisible && !mAnimator_lod0.GetBool ("roll")) {
			mAnimator_lod0.SetBool ("roll", true);
			mAnimator_lod1.SetBool ("roll", false);
		} else if (mRenderer_lod1_door.isVisible && !mAnimator_lod1.GetBool ("roll")) {
			mAnimator_lod1.SetBool ("roll", true);
			mAnimator_lod0.SetBool ("roll", false);
		} else if(!mRenderer_lod0_door.isVisible && !mRenderer_lod1_door.isVisible){
			mAnimator_lod0.SetBool ("roll", false);
			mAnimator_lod1.SetBool ("roll", false);
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

}
