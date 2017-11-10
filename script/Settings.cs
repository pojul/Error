using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings: MonoBehaviour {

	public Texture setBtnTexture;
	public Rect setWinRect;
	public bool showSetWin = false;
	public Vector2 scrollPosition = Vector2.zero;
    public GUIStyle style;
	public GUIStyle btstyle;
	public string ShowItemSettings = "mapSettings";

	public float verticalValue = 50.0f;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI(){

		if(GUI.Button (new Rect(Screen.width-Screen.height/10,
			0, Screen.height/10, Screen.height/10), setBtnTexture)){
			showSetWin = true;
		}
		if(showSetWin){
			setWinRect = new Rect (Screen.width/10, Screen.height/10, Screen.width*8/10, Screen.height*8/10);
			setWinRect = GUI.Window (0, setWinRect, setWin, "setting");
		}
	}

	void setWin(int windowId){
		if(GUI.Button(new Rect(Screen.width*8/10-Screen.height*2/10, 0, Screen.height*2/10, Screen.height/10), "close")){
			showSetWin = false;
		}
		//设置窗口选项
		GUI.skin.scrollView = style;
		scrollPosition = GUI.BeginScrollView(new Rect(0, Screen.height/10, Screen.height*2.5f/10, Screen.height*8/10),
				scrollPosition, new Rect(0, 0, Screen.height*2/10, Screen.height*12f/10));
		//子设置界面选项
		if(GUI.Button(new Rect(0, 0, Screen.height*2/10, Screen.height*1/10), "地 图")){
			ShowItemSettings = "mapSettings";
		}else if(GUI.Button(new Rect(0, Screen.height*1/10, Screen.height*2/10, Screen.height*1/10), "1")){
			ShowItemSettings = "1";
		}else if(GUI.Button(new Rect(0, Screen.height*2/10, Screen.height*2/10, Screen.height*1/10), "2")){
			ShowItemSettings = "2";
		}else if(GUI.Button(new Rect(0, Screen.height*3/10, Screen.height*2/10, Screen.height*1/10), "3")){
			ShowItemSettings = "3";
		}else if(GUI.Button(new Rect(0, Screen.height*4/10, Screen.height*2/10, Screen.height*1/10), "4")){
			ShowItemSettings = "4";
		}else if(GUI.Button(new Rect(0, Screen.height*5/10, Screen.height*2/10, Screen.height*1/10), "5")){
			ShowItemSettings = "5";
		}else if(GUI.Button(new Rect(0, Screen.height*6/10, Screen.height*2/10, Screen.height*1/10), "6")){
			ShowItemSettings = "6";
		}else if(GUI.Button(new Rect(0, Screen.height*7/10, Screen.height*2/10, Screen.height*1/10), "7")){
			ShowItemSettings = "7";
		}else if(GUI.Button(new Rect(0, Screen.height*8/10, Screen.height*2/10, Screen.height*1/10), "8")){
			ShowItemSettings = "8";
		}else if(GUI.Button(new Rect(0, Screen.height*9/10, Screen.height*2/10, Screen.height*1/10), "9")){
			ShowItemSettings = "9";
		}else if(GUI.Button(new Rect(0, Screen.height, Screen.height*2/10, Screen.height*1/10), "10")){
			ShowItemSettings = "10";
		}
        GUI.EndScrollView();
		//加载子设置界面
		loadItemSetUI();
	}
	
	void loadItemSetUI(){
		switch(ShowItemSettings){
			case "mapSettings" :
			loadSetUiMap();
			break;
			case "1" :
			loadSetUi1();
			break;
			case "2" :
			loadSetUi2();
			break;
			case "3" :
			loadSetUi3();
			break;
			case "4" :
			loadSetUi4();
			break;
			case "5" :
			loadSetUi5();
			break;
			case "6" :
			loadSetUi6();
			break;
			case "7" :
			loadSetUi7();
			break;
			case "8" :
			loadSetUi8();
			break;
			case "9" :
			loadSetUi9();
			break;
			case "10" :
			loadSetUi10();
			break;
		}
	}
	
	void loadSetUiMap(){
		GUI.Button (new Rect (Screen.height*2.6f/10, Screen.height*1.5f/10, Screen.width*1.2f/10, Screen.height/10), "地图大小：", btstyle);
		verticalValue = GUI.HorizontalSlider(new Rect(Screen.height*2.8f/10 + Screen.width*1.2f/12, Screen.height*1.6f/10 ,Screen.width/10, Screen.height/10),
			verticalValue, 0f, 100f);

		if(GUI.Button (new Rect (Screen.width*3f/10+Screen.height*1.25f/10, Screen.height*6.8f/10, Screen.width*2f/10, Screen.height/10), "确定")){
			GameObject terrain = GameObject.FindGameObjectWithTag ("terrain");
			float terrainWidth = terrain.GetComponent<Terrain> ().terrainData.size.x;
			float terrainLength = terrain.GetComponent<Terrain> ().terrainData.size.z;
			Debug.Log ("terrainWidth: " + terrainWidth);
			Debug.Log ("terrainLength: " + terrainLength);
			terrain.GetComponent<Terrain> ().terrainData.size = new Vector3(terrainWidth*verticalValue/5 ,1,terrainLength*verticalValue/5);
			showSetWin = false;
		}

		//terrain.GetComponent

	}
	
	void loadSetUi1(){
		if(GUI.Button(new Rect(Screen.height*2.5f/10, Screen.height*1/10, 
				Screen.width*8/10-Screen.height*2/10, Screen.height*8/10 - Screen.height*1/10), "1")){
			
		}
	}
	
	void loadSetUi2(){
		if(GUI.Button(new Rect(Screen.height*2.5f/10, Screen.height*1/10, 
				Screen.width*8/10-Screen.height*2/10, Screen.height*8/10 - Screen.height*1/10), "2")){
			
		}
	}
	
	void loadSetUi3(){
		if(GUI.Button(new Rect(Screen.height*2.5f/10, Screen.height*1/10, 
				Screen.width*8/10-Screen.height*2/10, Screen.height*8/10 - Screen.height*1/10), "3")){
			
		}
	}

	void loadSetUi4(){
		if(GUI.Button(new Rect(Screen.height*2.5f/10, Screen.height*1/10, 
				Screen.width*8/10-Screen.height*2/10, Screen.height*8/10 - Screen.height*1/10), "4")){
			
		}
	}
	
	void loadSetUi5(){
		if(GUI.Button(new Rect(Screen.height*2.5f/10, Screen.height*1/10, 
				Screen.width*8/10-Screen.height*2/10, Screen.height*8/10 - Screen.height*1/10), "5")){
			
		}
	}
	
	void loadSetUi6(){
		if(GUI.Button(new Rect(Screen.height*2.5f/10, Screen.height*1/10, 
				Screen.width*8/10-Screen.height*2/10, Screen.height*8/10 - Screen.height*1/10), "6")){
			
		}
	}
	
	void loadSetUi7(){
		if(GUI.Button(new Rect(Screen.height*2.5f/10, Screen.height*1/10, 
				Screen.width*8/10-Screen.height*2/10, Screen.height*8/10 - Screen.height*1/10), "7")){
			
		}
	}
	
	void loadSetUi8(){
		if(GUI.Button(new Rect(Screen.height*2.5f/10, Screen.height*1/10, 
				Screen.width*8/10-Screen.height*2/10, Screen.height*8/10 - Screen.height*1/10), "8")){
			
		}
	}
	
	void loadSetUi9(){
		if(GUI.Button(new Rect(Screen.height*2.5f/10, Screen.height*1/10, 
				Screen.width*8/10-Screen.height*2/10, Screen.height*8/10 - Screen.height*1/10), "9")){
			
		}
	}
	
	void loadSetUi10(){
		if(GUI.Button(new Rect(Screen.height*2.5f/10, Screen.height*1/10, 
				Screen.width*8/10-Screen.height*2/10, Screen.height*8/10 - Screen.height*1/10), "10")){
			
		}
	}
}
