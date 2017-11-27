using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthMeter : MonoBehaviour {

	private float rawWidth = Screen.width/10;
	private float rawHeight = Screen.width/100;

	private float RawHeightSpace = Screen.height / 40;



	private int currentHealthy = 10;
	private int maxtHealthy = 100;

	private bool neeShow = true;

	void Start ()
	{
		InvokeRepeating ("point",1,1);
	}
 
	void Update ()
	{
	}
	

	void point(){
		Vector2 point = RectTransformUtility.WorldToScreenPoint (Camera.main, transform.position);
		if(currentHealthy < 100){
			currentHealthy = currentHealthy + 2;
		}
	}

	void OnBecameVisible(){
		neeShow = true;
	}

	void OnBecameInvisible(){
		neeShow = false;
	}


	void OnGUI()
	{
		if(!neeShow){
			return;
		}
		Vector2 point = RectTransformUtility.WorldToScreenPoint (Camera.main, transform.position);
		float scale = -(Camera.main.transform.position - transform.position).magnitude*0.00008f + 1;
		if(scale <= 0){
			return;
		}
		float width = rawWidth * scale;
		float height = rawHeight * scale;
		float heightSpace = RawHeightSpace * scale;
		float posX = point.x - width * 0.5f;
		float posY = Screen.height - point.y - heightSpace;
		GUI.DrawTexture (new Rect(posX,posY,width,height), GameInit.backgroundProgress);
		GUI.DrawTexture (new Rect(posX,posY,currentHealthy* 0.01f * width,height), GameInit.progress1);
	}

	
}
