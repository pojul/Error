using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldUIManager : MonoBehaviour {

	public Image fireAim;
	public Image magnifierAim;
	public static Transform fireAimTra;
	public static float fireAimDistance = 0.0f;
	public List<Transform> MyPaos = new List<Transform> ();
	public Transform mainPao;

	//private float scaleTimes = 0.00065f;
	private float scaleTimes = 0.0027f;
	public static Camera magnifierCamera;
	public float magnifierAimDis = 12000;
	private Transform magnifierTra;

	// Use this for initialization
	void Start () {
		for(int i =0; i< Camera.allCameras.Length; i++){
			if(Camera.allCameras [i].CompareTag("magnifierCamera")){
				magnifierCamera = Camera.allCameras [i];
				break;
			}
		}

		fireAim.rectTransform.sizeDelta = new Vector2 (Screen.height *0.1f, Screen.height *0.1f);
		//magnifierAim.rectTransform.sizeDelta = new Vector2 (Screen.height *0.04f, Screen.height *0.04f);
		magnifierAim.rectTransform.sizeDelta = new Vector2 (Screen.height *0.08f, Screen.height *0.08f);
		magnifierAim.rectTransform.localPosition = new Vector3 (0, 0, 0);
		//Debug.Log ("gqb------>dpi: " + Screen.dpi);
		if (Screen.dpi > 0) {
			scaleTimes = 0.0027f *96/  Screen.dpi;
		} else {
			scaleTimes = 0.00065f;
		}

	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (planMove.player + " :player gqb------>fireAimTra: " +fireAimTra);

		if(planMove.player != null ){
			PojulObject mPojulObject = planMove.player.GetComponent<PojulObject> ();
			if(mPojulObject != null && !mPojulObject.isDestoryed && mPojulObject.type.Equals("a10") && mPojulObject.MissileAimedTra != null){
				lookAtBack (planMove.player);
				return;
			}
		}

		if (planMove.player != null && fireAimTra != null) {
			rayCast (fireAimTra, fireAim, fireAimDistance, 1);

			//magnifierAim.rectTransform.localScale = new Vector3 (0, 0, 0);
			magnifierCamera.transform.position = fireAimTra.position;
			magnifierCamera.transform.rotation = fireAimTra.rotation;
			magnifierTra = null;
		} else {
			fireAim.rectTransform.localScale = new Vector3 (0, 0, 0);
			if (isMainPaoAttacked ()) {
				//rayCast (mainPao, magnifierAim, magnifierAimDis, 1);
				magnifierCamera.transform.position = mainPao.position;
				magnifierCamera.transform.rotation = mainPao.rotation;
			} else {
				if (magnifierTra == null) {
					findMagnifierTra ();
				}else{
					//rayCast (magnifierTra, magnifierAim, magnifierAimDis, 1);
					magnifierCamera.transform.position = magnifierTra.position;
					magnifierCamera.transform.rotation = magnifierTra.rotation;
				}
			}
		}
	}

	void rayCast(Transform direction, Image aim, float aimDis, float rScale){
		RaycastHit hit;
		bool isHit = Physics.Raycast (direction.position, direction.forward, out hit, aimDis);
		if (isHit) {
			aim.rectTransform.position = hit.point;
		} else {
			aim.rectTransform.position = direction.position + direction.forward * aimDis;
		}

		float dlength = (aim.rectTransform.position - direction.position).magnitude;

		float scale = dlength * scaleTimes;
		if (scale < 0.5f) {
			scale = 0.5f;
		}
		scale = scale * rScale;

		aim.rectTransform.localScale = new Vector3 (scale, scale, scale);
		aim.transform.rotation = direction.rotation;
	}

	bool isMainPaoAttacked(){
		if(mainPao != null && mainPao.root.GetComponent<HomePao>()){
			HomePao mHomePao = mainPao.root.GetComponent<HomePao> ();
			if(!mHomePao.isDestoryed && mHomePao.target != null && (mHomePao.target.position - mainPao.position).magnitude < 4800){
				return true;
			}
		}
		return false;
	}

	void findMagnifierTra(){
		for(int i = 0; i < MyPaos.Count; i ++){
			if(MyPaos[i] == null || !MyPaos[i].root.gameObject.GetComponent<LittleCannon1>()
				|| MyPaos[i].root.gameObject.GetComponent<LittleCannon1>().isDestoryed){
				continue;
			}
			magnifierTra = MyPaos [i];
			break;
		}
	}

	void lookAtBack(Transform lookAt){
		magnifierCamera.transform.position = lookAt.position + lookAt.forward *1500;
		magnifierCamera.transform.rotation = Quaternion.Slerp (magnifierCamera.transform.rotation, 
			Quaternion.LookRotation(lookAt.position - magnifierCamera.transform.position),
			Time.deltaTime * 48f);
	}

}
