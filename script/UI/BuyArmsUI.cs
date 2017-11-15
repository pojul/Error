using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyArmsUI : MonoBehaviour {

	private GUIStyle buyStyle;
	public Texture2D buyBg1;
	public Texture2D buyBg2;
	private bool isBuyClicked = false;
	private float buyWidth = Screen.height/10;
	private float buyHeight = Screen.height/10;
	private float buyX = 0;
	private float buyY = Screen.height/6;

	private bool showShopWin = false;
	private Rect shopWinRect = new Rect (Screen.width*2.5f/10, Screen.height*2/10, Screen.width*5/10, Screen.height*6/10);
	private GUIStyle shopWinStyle;
	private Vector2 scrollPosition = Vector2.zero;
	private float closeBtX = Screen.width*5/10-Screen.height*2/10;
	private float closeBtY = 0;
	private float closeBtWidth = Screen.height*2/10;
	private float closeBtHeight = Screen.height / 10;

	private float scollViewX = 0;
	private float scollViewY = Screen.height*1/10;
	private float scollViewWidth = Screen.width*5f/10;
	private float scollViewHeight = Screen.height*6/10;
	private Rect scollViewRect = new Rect(0, 0, Screen.width*6/10, Screen.height*18.0f/10);

	private float itemImgX = Screen.height/20;
	private float itemImgWidth = Screen.height * 3 / 10;
	private float itemImgHeight = Screen.height * 1.6f / 10;
	private float itemSpace = Screen.height * 1.7f / 10;
	public Texture2D[] armImgs = new Texture2D[10];
	public string[] armNames = new string[10];
	private GUIStyle[] itemImgStys = new GUIStyle[10];
	private float itemBuyX = Screen.width * 5 / 10 - Screen.height * 3 / 10;
	private float itemBuyWidth = Screen.height*2/10;
	private float itemBuyheight = Screen.height/10;
	private float itemPricex = Screen.height * 7f / 20;
	private float itemPricewidth = Screen.height * 2 / 10;
	private float itemPriceheight = Screen.height * 1.6f / 10;
	public string[] buyStatus = new string[3];

	//private List<ArmInfo> armInfos = new List<ArmInfo>();

	//private itemHeight 


	// Use this for initialization
	void Start () {
		buyStyle = new GUIStyle ();
		buyStyle.normal.background = buyBg1;
		for (int i = 0; i < armImgs.Length; i++) {
			itemImgStys [i] = new GUIStyle ();
			itemImgStys [i].normal.background = armImgs [i];
		}
	}

	void OnGUI(){
		draw ();
	}

	void draw(){
		if (GUI.Button (new Rect (buyX, buyY, buyWidth, buyHeight), 
			   "", buyStyle)) {
			buyStyle.normal.background = buyBg2;
			isBuyClicked = true;
			showShopWin = true;
			Invoke ("setNormalBg", 0.2f);
		}

		if(showShopWin){
			shopWinRect = GUI.Window (1, shopWinRect, shopWin, "arms shop");
		}

	}

	void shopWin(int shopWin){
		if(GUI.Button(new Rect(closeBtX, 0, closeBtWidth, closeBtHeight), "close")){
			showShopWin = false;
		}
			
		GUI.skin.scrollView = shopWinStyle;
		scrollPosition = GUI.BeginScrollView(new Rect(scollViewX, scollViewY, scollViewWidth, scollViewHeight),
			scrollPosition, scollViewRect);
		
		for(int i = 0;i< armImgs.Length; i++){
			string str = GameInit.prices[armNames[i]] + "/" + GameInit.MyMoney.ToString ();
			string buyStr = buyStatus [0];
			if((int)GameInit.prices[armNames[i]] > GameInit.MyMoney){
				buyStr = buyStatus [1];
			}
			if((int)GameInit.currentInstance[("0_"+armNames[i])] >= (int)GameInit.maxInstance[armNames[i]]/2){
				buyStr = buyStatus [2];
			}

			if(GUI.Button(new Rect(itemImgX, itemSpace*i, itemImgWidth, itemImgHeight), "", itemImgStys[i])){
			}
			if(GUI.Button(new Rect(itemPricex, itemSpace*i, itemPricewidth, itemPriceheight), str)){
			}
			if(GUI.Button(new Rect(itemBuyX, (itemSpace*i + Screen.height * 0.3f / 10), itemBuyWidth, itemBuyheight), buyStr)){
				if(buyStatus [0].Equals(buyStr)){
					GameInit.instanceGameobject (0, armNames[i]);
					GameInit.MyMoney = GameInit.MyMoney - (int)GameInit.prices [armNames [i]];
				}
			}
		}
		GUI.EndScrollView();

	}

	void setNormalBg(){
		if(isBuyClicked){
			buyStyle.normal.background = buyBg1;
			isBuyClicked = false;
		}
	}

}
