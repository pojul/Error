using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : PojulObject {

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

	private float aimSpeed = 1.0f;
	private float aimMaxVer = 10.0f;

	private bool isMoving = false;
	private float maxMoveSpeed = GameInit.mach * 0.4f;

	private bool isPanDestoryed = false;

	public string currentCoordinate = "";
	public List<Transform> nearEnemys;

	//test
	private GameObject target;

	// Use this for initialization
	void Start () {

		mAudioSource = (AudioSource)transform.GetComponent<AudioSource> ();

		transform_lod0 = transform.FindChild ("car_type3_lod0");
		transform_lod1 = transform.FindChild ("car_type3_lod1");
		transform_lod2 = transform.FindChild ("car_type3_lod2");

		mAnimator_lod0 = (Animator)transform_lod0.GetComponent<Animator> ();
		mAnimator_lod1 = (Animator)transform_lod1.GetComponent<Animator> ();
		mAnimator_lod2 = (Animator)transform_lod2.GetComponent<Animator> ();
		mAnimator_lod0.SetBool ("roll", false);
		mAnimator_lod1.SetBool ("roll", false);
		mAnimator_lod2.SetBool ("roll", false);

		panTransform_lod0 = transform_lod0.FindChild("pan");
		panTransform_lod1 = transform_lod1.FindChild("pan");
		panTransform_lod2 = transform_lod2.FindChild("pan");

		mRenderer_lod0_pan = panTransform_lod0.GetComponent<Renderer>();
		mRenderer_lod1_pan = panTransform_lod1.GetComponent<Renderer>();
		mRenderer_lod2_pan = panTransform_lod2.GetComponent<Renderer>();

		paoTransform_lod0 = panTransform_lod0.FindChild ("pao");
		paoTransform_lod1 = panTransform_lod1.FindChild ("pao");
		paoTransform_lod2 = panTransform_lod2.FindChild ("pao");

		InvokeRepeating ("updateCoordinate", 2.0f, 2.0f);
	}

	void updateCoordinate(){
		object[]  coordinateInfo = Util.updateCoordinate (transform, currentCoordinate, "0", true, 0);
		if(coordinateInfo[0] != null){
			currentCoordinate = (string)coordinateInfo [0];
		}
		if(coordinateInfo[1] != null){
			nearEnemys = (List<Transform>)coordinateInfo [1];
		}
		Debug.Log ("gqb------>currentCoordinate: " + currentCoordinate);
	}


	// Update is called once per frame
	void Update () {
		transform.position = transform.position + transform.forward * 250 * Time.deltaTime;
	}

	public override void isFired(RaycastHit hit, Collision collision, int type){

		if(type ==2){
			string hitName = "";
			if (collision != null) {
				hitName = collision.gameObject.name;
			} else {
				hitName = hit.transform.name;
			}
			if(hitName.Equals("pan") && !isPanDestoryed){
				destoryPan (collision);
			}
		}
	}

	void destoryPan(Collision collision){
		panTransform_lod0.parent = null;
		panTransform_lod1.parent = panTransform_lod0;
		panTransform_lod2.parent = panTransform_lod0;
		panTransform_lod0.gameObject.AddComponent<Rigidbody> ();

		paoTransform_lod0.parent = null;
		paoTransform_lod1.parent = paoTransform_lod0;
		paoTransform_lod2.parent = paoTransform_lod0;
		paoTransform_lod0.gameObject.AddComponent<BoxCollider> ();
		paoTransform_lod0.gameObject.AddComponent<Rigidbody> ();

		Vector3 explosionPos = new Vector3 (collision.contacts[0].point.x, 
			(collision.contacts[0].point.y - 10), 
			collision.contacts[0].point.z);
		Collider[] colliders = Physics.OverlapSphere(explosionPos, 50.0f);
		foreach (Collider hit in colliders){
            if (hit.GetComponent<Rigidbody>()){ 
				hit.GetComponent<Rigidbody>().AddExplosionForce(5000.0f, explosionPos, 50.0f);
            }  
        }
		Invoke ("destoryPan", 20);
	}

	void destoryPan(){
		if(panTransform_lod0 != null){
			Destroy (panTransform_lod0.gameObject);
		}
		if(paoTransform_lod0 != null){
			Destroy (paoTransform_lod0.gameObject);
		}
		isPanDestoryed = true;
	}

}
