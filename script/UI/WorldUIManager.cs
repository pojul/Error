using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldUIManager : MonoBehaviour {

	public Image fireAim;
	public static Transform fireAimTra;
	public static float fireAimDistance = 0.0f;

	//private float scaleTimes = 0.00065f;
	private float scaleTimes = 0.0027f;

	// Use this for initialization
	void Start () {
		fireAim.rectTransform.sizeDelta = new Vector2 (Screen.height *0.1f, Screen.height *0.1f);
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (planMove.player + " :player gqb------>fireAimTra: " +fireAimTra);
		if(planMove.player != null && fireAimTra != null){
			RaycastHit hit;
			bool isHit = Physics.Raycast (fireAimTra.position, fireAimTra.forward, out hit, fireAimDistance);
			if (isHit) {
				fireAim.rectTransform.position = hit.point;
			} else {
				fireAim.rectTransform.position = fireAimTra.position + fireAimTra.forward * fireAimDistance;
			}

			float dlength = (fireAim.rectTransform.position - fireAimTra.position).magnitude;

			float scale = dlength * scaleTimes;
			if(scale < 0.5f){scale = 0.5f;}

			//if(scale > 1){
			fireAim.rectTransform.localScale = new Vector3(scale, scale, scale);
			//}
			//Debug.Log (scale + " :scale gqb------>dlength: " + dlength + "scaleTimes: " + scaleTimes);

			//fireAim.rectTransform.position = planMove.player.position + planMove.player.transform.forward * 1000;
			fireAim.transform.rotation = planMove.player.rotation;
		}
	}
}
