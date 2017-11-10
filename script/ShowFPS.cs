using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFPS : MonoBehaviour {

	//每次刷新计算的时间
	public float updateInterval = 0.5f;
	//最后间隔时间
	private double lastInterval;
	private int frames = 0;
	private float currFPS;

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 30;
		lastInterval = Time.realtimeSinceStartup;
		frames = 0;
	}
	
	// Update is called once per frame
	void Update () {
		++frames;
		float timeNow = Time.realtimeSinceStartup;
		if(timeNow > lastInterval + updateInterval){
			currFPS = (float)(frames / (timeNow - lastInterval));
			frames = 0;
			lastInterval = timeNow;
		}
	}

	private void OnGUI(){
		GUILayout.Label ("1FPS：" + currFPS.ToString("f2"));
	}
}
