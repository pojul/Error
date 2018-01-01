﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UImanager : MonoBehaviour {

	public Canvas mainUI;
	public Image thubmnail;
	public Slider accelerate;

	public Image fireMissile;
	public Image aimNext;
	public Image fire;
	public Image missileAim;
	public Image mselect;
	public Image buy;
	public Image touchMove;
	public Image leave;


	public Image thubmnailPoint;
	public Text text;
	private Image cameraThubmnailPoint;

	public Sprite redThubmnailPoint;
	public Sprite blueThubmnailPoint;
	public Sprite yellowThubmnailPoint;
	public Sprite selectedThubmnailPoint;
	public Sprite cameraThubmnailBg;

	public Sprite fireMissileBg1;
	public Sprite fireMissileBg2;
	public Sprite fireMissileBg3;
	public Sprite aimNextBg1;
	public Sprite aimNextBg2;
	public Sprite fireBg1;
	public Sprite fireBg2;
	public Sprite fireBg3;
	public Sprite mselect1;
	public Sprite mselect2;
	public Sprite buy1;
	public Sprite buy2;
	public Sprite leave1;
	public Sprite leave2;

	private Dictionary<int, object[]> thubmnailPoints = new Dictionary<int, object[]> ();//GameObject, Image
	private Dictionary<int, object[]> enemythubmnailPoints = new Dictionary<int, object[]> ();

	public Transform missileAimedTra;

	public Image fireAimPre;
	public static Image fireAim;

	private string playerType = "";
	private bool canFire = true;
	private float lastFireTime = 0;
	private bool isFireDown = false;
	private bool isSelectMode = false;
	public static bool isCamreaMoveTo = false;
	public static bool isCamreaHeightMove = false;
	public static Vector3 currentCamreaMoveTo = new Vector3(0, 12000, -60000);
	private float lastCamreaMoveToTime = 0;
	private Vector2 touch1;
	private Vector2 touch2;
	private bool touch1Ended = false;
	private bool touch2Ended = false;
	private float lastTouch1Time = 0;
	private float lastTouch2Time = 0;
	private float lastTouchTime = 0;
	private Vector3 touchMoveDown;
	private float cameraY = 12000;
	private float frontCameraY = 12000;
	private Transform selectedTra;
	private float scrollPos = 0.0f;
	private bool isOnLeave = false;
	private float onLeaveHeight = 12000;

	private bool showShopWin = false;
	public string[] armNames = new string[10];
	private float armShopItemImgHeight = Screen.height * 0.16f;
	private float armShopItemImgWidth = Screen.height * 0.3f;
	private float armShopItemPriceHeight = Screen.height * 0.16f;
	private float armShopItemPriceWidth = Screen.height * 0.48f;
	private float armShopItemBuyheight = Screen.height* 0.09f;
	private float armShopItemBuywidth = Screen.height* 0.17f;
	private float armShopItemSpace = Screen.height * 0.01f;
	public Image armShopPanel;
	public Image armShopTitle;
	public Image armShopClose;
	public GameObject armShopScrollView;
	public GameObject armShopContent;
	public Image[] armShopImgs = new Image[10];
	public Text[] armShopPrices = new Text[10];
	public List<Image> armShopBuys = new List<Image>();
	private List<GameObject> armShopBuyObjs = new List<GameObject>();
	public Sprite[] armShopBgs = new Sprite[10];
	private bool[] couldArmBuy = new bool[10];

	public Sprite armShopClose1;
	public Sprite armShopClose2;
	public Sprite armShopBuy1;
	public Sprite armShopBuy2;
	public Sprite armShopBuy3;


	private bool showArmInfo = false;
	private int textSize = (int)(Screen.height* 0.045f);
	private int textSize1 = (int)(Screen.height* 0.028f);
	private List<string> patrolAreaValueStr1 = new List<string> ();
	private List<string> patrolAreaValueStr2 = new List<string> ();
	private Color textEnableColor = new Color(160/255f, 226/255f, 1, 1);
	private Color textDisableColor = new Color(137/255f, 137/255f, 137/255f, 1);
	private float sellingInv = 0.0f;
	private int sellingNoteIndex = 0;
	private string[] sellingNotes = new string[]{".  ", ".. ", "..."};


	public Image armInfoPanel;
	public Image armInfoClose;
	public Image armInfoTitle;
	public Image armInfoContent;
	public Text armInfoType;
	public Text health;
	public Text patrolArea;
	public Text attackArmy;
	public Dropdown patrolAreaValue;
	public Toggle attackArmyValue;
	public Text maxMountMissile;
	public Text supplyPriority;
	public Dropdown maxMountMissileValue;
	public Dropdown supplyPriorityValue;
	public Image armInfoControl;
	public Image armInfoSell;
	public Text armInfoArms;
	public Text armInfoSellNote;
	private Transform sellingTra;


	public Sprite armInfoClose1;
	public Sprite armInfoClose2;
	public Sprite armInfoControl1;
	public Sprite armInfoControl2;
	public Sprite armInfoControl3;
	public Sprite armInfoLeave1;
	public Sprite armInfoLeave2;
	public Sprite armInfoSell1;
	public Sprite armInfoSell2;
	public Sprite armInfoSell3;
	public Sprite armInfoSell4;

	// Use this for initialization
	void Start () {
		fireAim = fireAimPre;

		thubmnail.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.32f, Screen.height * 0.32f);
		thubmnail.rectTransform.position = new Vector3 (thubmnail.rectTransform.sizeDelta .x * 0.5f, 
			(Screen.height - thubmnail.rectTransform.sizeDelta .y * 0.5f),thubmnail.rectTransform.position.z);

		cameraThubmnailPoint = (Image)Instantiate (thubmnailPoint);
		cameraThubmnailPoint.GetComponent<Transform> ().SetParent (mainUI.GetComponent<Transform> (), true);
		cameraThubmnailPoint.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.01f, Screen.height * 0.01f);
		cameraThubmnailPoint.sprite = cameraThubmnailBg;

		fire.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.12f, Screen.height * 0.12f);
		fire.rectTransform.position = new Vector3 ((Screen.width - fire.rectTransform.sizeDelta .x * 0.56f), 
			fire.rectTransform.sizeDelta .y * 2.15f,fire.rectTransform.position.z);

		leave.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.12f, Screen.height * 0.12f);
		leave.rectTransform.position = new Vector3 ((Screen.width - leave.rectTransform.sizeDelta .x * 0.56f), 
			leave.rectTransform.sizeDelta .y * 3.25f,leave.rectTransform.position.z);

		aimNext.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.12f, Screen.height * 0.12f);
		aimNext.rectTransform.position = new Vector3 ((Screen.width - aimNext.rectTransform.sizeDelta .x * 2.15f), 
			aimNext.rectTransform.sizeDelta .y * 0.56f,aimNext.rectTransform.position.z);

		fireMissile.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.18f, Screen.height * 0.18f);
		fireMissile.rectTransform.position = new Vector3 ((Screen.width - fireMissile.rectTransform.sizeDelta .x * 0.52f), 
			fireMissile.rectTransform.sizeDelta .y * 0.52f,thubmnail.rectTransform.position.z);

		missileAim.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.05f, Screen.height * 0.05f);

		PlanControls.control3CircleCenterX = Screen.width*9.4f/10 - Screen.height*3.0f/10;
		PlanControls.control3CircleX = Screen.width*9.4f/10 - Screen.height*5f/10;

		RectTransform mRectTransform = accelerate.GetComponent<RectTransform>();
		mRectTransform.sizeDelta = new Vector2 (Screen.height * 0.4f, Screen.height * 0.1f);
		mRectTransform.position = new Vector3 ((Screen.width - mRectTransform.sizeDelta .y * 0.6f), 
			Screen.height * 0.655f, mRectTransform.position.z);
		accelerate.value = accelerate.maxValue * 0.5f;
		accelerate.handleRect.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.height * 0.06f, Screen.height * 0.05f);;

		buy.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.11f, Screen.height * 0.11f);
		buy.rectTransform.position = new Vector3 (buy.rectTransform.sizeDelta .x * 0.5f, 
			buy.rectTransform.sizeDelta .y * 5f,buy.rectTransform.position.z);

		mselect.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.11f, Screen.height * 0.11f);
		mselect.rectTransform.position = new Vector3 (mselect.rectTransform.sizeDelta .x * 0.5f, 
			mselect.rectTransform.sizeDelta .y * 4f,mselect.rectTransform.position.z);
		
		touchMove.rectTransform.sizeDelta = new Vector2 ( (Screen.width - Screen.height * 0.64f), Screen.height);
		touchMove.rectTransform.position = new Vector3 ((Screen.height * 0.32f + touchMove.rectTransform.sizeDelta .x * 0.5f), 
			touchMove.rectTransform.sizeDelta .y * 0.5f ,touchMove.rectTransform.position.z);

		initArmShop ();
		initArmInfo ();

		InvokeRepeating("updateThubmnail", 0.5f, 0.5f);

		InvokeRepeating("updateMissileAim", 0.2f, 0.2f);
		InvokeRepeating("clearNullThubmnail", 2f, 2f);
	}

	void initArmShop(){
		armShopPanel.rectTransform.sizeDelta = new Vector2 (Screen.height * 1.12f, Screen.height * 0.7f);
		armShopPanel.rectTransform.position = new Vector3 (
			(armShopPanel.rectTransform.sizeDelta .x * 0.5f + (Screen.width - Screen.height * 1.12f)*0.5f), 
			(Screen.height - armShopPanel.rectTransform.sizeDelta .y * 0.5f - Screen.height * 0.15f), 
			armShopPanel.rectTransform.position.z);

		armShopTitle.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.28435f, Screen.height * 0.11f);
		armShopTitle.rectTransform.position = new Vector3 (
			(armShopPanel.rectTransform.position.x), 
			(armShopPanel.rectTransform.position.y + (armShopPanel.rectTransform.sizeDelta.y - armShopTitle.rectTransform.sizeDelta.y)*0.5f), 
			armShopTitle.rectTransform.position.z);

		armShopClose.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.19682f, Screen.height * 0.11f);
		armShopClose.rectTransform.position = new Vector3 (
			(armShopPanel.rectTransform.position.x + (armShopPanel.rectTransform.sizeDelta.x - armShopClose.rectTransform.sizeDelta.x)*0.5f - Screen.height * 0.01f), 
			(armShopPanel.rectTransform.position.y + (armShopPanel.rectTransform.sizeDelta.y - armShopClose.rectTransform.sizeDelta.y)*0.5f - Screen.height * 0.01f), 
			armShopClose.rectTransform.position.z);

		RectTransform armShopScrollViewRect = armShopScrollView.GetComponent<RectTransform> ();
		armShopScrollViewRect.sizeDelta = new Vector2 (Screen.height * 1.11f, Screen.height * 0.56f);
		armShopScrollViewRect.position = new Vector3 (
			(armShopPanel.rectTransform.position.x), 
			(armShopPanel.rectTransform.position.y - (armShopPanel.rectTransform.sizeDelta.y - armShopScrollViewRect.sizeDelta.y)*0.5f + Screen.height * 0.01f), 
			armShopScrollViewRect.position.z);

		RectTransform armShopContentRect = armShopContent.GetComponent<RectTransform> ();
		armShopContentRect.sizeDelta = new Vector2 (armShopContentRect.sizeDelta.x, (armShopItemImgHeight*10 + armShopItemSpace*9));
		scrollPos = armShopContentRect.position.y;

		for(int i = 0; i< armShopImgs.Length; i++){
			armShopImgs[i] = (Image)Instantiate (thubmnailPoint);
			armShopImgs[i].GetComponent<Transform> ().SetParent (armShopContent.GetComponent<Transform> (), true);
			armShopImgs[i].rectTransform.sizeDelta = new Vector2 (armShopItemImgWidth, armShopItemImgHeight);
			armShopImgs [i].rectTransform.position = new Vector3 (
				(armShopContentRect.position.x - ((armShopScrollViewRect.sizeDelta.x - armShopImgs [i].rectTransform.sizeDelta.x)*0.5f) + Screen.height * 0.05f), 
				(armShopContentRect.position.y - armShopItemImgHeight *0.5f - (armShopItemImgHeight + armShopItemSpace)*i),
				armShopImgs[i].rectTransform.position.z);
			armShopImgs[i].sprite = armShopBgs[i];

			armShopPrices[i] = (Text)Instantiate (text);
			armShopPrices[i].GetComponent<Transform> ().SetParent (armShopContent.GetComponent<Transform> (), true);
			armShopPrices[i].rectTransform.sizeDelta = new Vector2 (armShopItemPriceWidth, armShopItemPriceHeight);
			armShopPrices [i].rectTransform.position = new Vector3 (
				(armShopContentRect.position.x - ((armShopScrollViewRect.sizeDelta.x - armShopPrices [i].rectTransform.sizeDelta.x)*0.5f) + Screen.height * 0.45f), 
				(armShopContentRect.position.y - armShopItemImgHeight *0.5f - (armShopItemImgHeight + armShopItemSpace)*i),
				armShopImgs[i].rectTransform.position.z);
			armShopPrices [i].fontSize = textSize;

			armShopBuys[i].rectTransform.sizeDelta = new Vector2 (armShopItemBuywidth, armShopItemBuyheight);
			armShopBuys [i].rectTransform.position = new Vector3 (
				(armShopContentRect.position.x + ((armShopScrollViewRect.sizeDelta.x - armShopBuys [i].rectTransform.sizeDelta.x)*0.5f) - Screen.height * 0.05f), 
				(armShopContentRect.position.y - armShopItemImgHeight *0.5f - (armShopItemImgHeight + armShopItemSpace)*i),
				armShopBuys[i].rectTransform.position.z);

			EventTriggerListener.Get(armShopBuys[i].gameObject).onClick = OnArmShopItemClick;
			EventTriggerListener.Get(armShopBuys[i].gameObject).onDown = OnArmShopItemDown;
			EventTriggerListener.Get(armShopBuys[i].gameObject).onUp = OnArmShopItemUp;
			armShopBuyObjs.Add (armShopBuys[i].gameObject);

			couldArmBuy [i] = false;
		}
		armShopPanel.rectTransform.localScale = new Vector3 (0, 0, 0);
	}

	void initArmInfo(){
		armInfoPanel.rectTransform.sizeDelta = new Vector2 (Screen.height * 1.0f, Screen.height * 0.55f);
		armInfoPanel.rectTransform.position = new Vector3 (
			Screen.width * 0.5f,
			Screen.height * 0.5f, 
			armInfoPanel.rectTransform.position.z);
		
		armInfoClose.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.16582f, Screen.height * 0.09f);
		armInfoClose.rectTransform.position = new Vector3 (
			(armInfoPanel.rectTransform.position.x + (armInfoPanel.rectTransform.sizeDelta.x - armInfoClose.rectTransform.sizeDelta.x)*0.5f - Screen.height * 0.005f), 
			(armInfoPanel.rectTransform.position.y + (armInfoPanel.rectTransform.sizeDelta.y - armInfoClose.rectTransform.sizeDelta.y)*0.5f - Screen.height * 0.005f), 
			armInfoClose.rectTransform.position.z);

		armInfoTitle.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.2425f, Screen.height * 0.1f);
		armInfoTitle.rectTransform.position = new Vector3 (
			(armInfoPanel.rectTransform.position.x), 
			(armInfoPanel.rectTransform.position.y + (armInfoPanel.rectTransform.sizeDelta.y - armInfoTitle.rectTransform.sizeDelta.y)*0.5f), 
			armInfoTitle.rectTransform.position.z);

		armInfoContent.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.97f, Screen.height * 0.44f);
		armInfoContent.rectTransform.position = new Vector3 (
			(armInfoPanel.rectTransform.position.x), 
			(armInfoPanel.rectTransform.position.y - (armInfoPanel.rectTransform.sizeDelta.y - armInfoContent.rectTransform.sizeDelta.y)*0.5f + Screen.height * 0.012f), 
			armInfoContent.rectTransform.position.z);

		armInfoType.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.4f, armInfoContent.rectTransform.sizeDelta.y * 0.25f);
		armInfoType.rectTransform.position = new Vector3 (
			(armInfoContent.rectTransform.position.x - (armInfoContent.rectTransform.sizeDelta.x - armInfoType.rectTransform.sizeDelta.x)*0.5f + Screen.height*0.02f), 
			(armInfoContent.rectTransform.position.y + (armInfoContent.rectTransform.sizeDelta.y - armInfoType.rectTransform.sizeDelta.y)*0.5f), 
			armInfoType.rectTransform.position.z);
		armInfoType.fontSize = textSize;

		health.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.4f, armInfoContent.rectTransform.sizeDelta.y * 0.25f);
		health.rectTransform.position = new Vector3 (
			(armInfoContent.rectTransform.position.x + (armInfoContent.rectTransform.sizeDelta.x - health.rectTransform.sizeDelta.x)*0.5f - Screen.height * 0.025f), 
			(armInfoContent.rectTransform.position.y + (armInfoContent.rectTransform.sizeDelta.y - health.rectTransform.sizeDelta.y)*0.5f), 
			health.rectTransform.position.z);
		health.fontSize = textSize;

		patrolArea.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.4f, armInfoContent.rectTransform.sizeDelta.y * 0.25f);
		patrolArea.rectTransform.position = new Vector3 (
			(armInfoContent.rectTransform.position.x - (armInfoContent.rectTransform.sizeDelta.x - patrolArea.rectTransform.sizeDelta.x)*0.5f + Screen.height*0.02f), 
			(armInfoContent.rectTransform.position.y + (armInfoContent.rectTransform.sizeDelta.y - patrolArea.rectTransform.sizeDelta.y)*0.5f - patrolArea.rectTransform.sizeDelta.y), 
			patrolArea.rectTransform.position.z);
		patrolArea.fontSize = textSize;

		attackArmy.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.4f, armInfoContent.rectTransform.sizeDelta.y * 0.25f);
		attackArmy.rectTransform.position = new Vector3 (
			(armInfoContent.rectTransform.position.x + (armInfoContent.rectTransform.sizeDelta.x - attackArmy.rectTransform.sizeDelta.x)*0.5f - Screen.height * 0.025f), 
			(armInfoContent.rectTransform.position.y + (armInfoContent.rectTransform.sizeDelta.y - attackArmy.rectTransform.sizeDelta.y)*0.5f - attackArmy.rectTransform.sizeDelta.y), 
			attackArmy.rectTransform.position.z);
		attackArmy.fontSize = textSize;


		patrolAreaValueStr1.Add ("1 district");patrolAreaValueStr1.Add ("2 district");patrolAreaValueStr1.Add ("3 district");patrolAreaValueStr1.Add ("4 district");
		patrolAreaValueStr2.Add ("5 district");patrolAreaValueStr2.Add ("6 district");patrolAreaValueStr2.Add ("7 district");patrolAreaValueStr2.Add ("8 district");
		RectTransform patrolAreaValueRect = patrolAreaValue.GetComponent<RectTransform> ();
		patrolAreaValue.AddOptions (patrolAreaValueStr1);
		patrolAreaValueRect.sizeDelta = new Vector2 (Screen.height * 0.21f, armInfoContent.rectTransform.sizeDelta.y * 0.17f);
		patrolAreaValueRect.position = new Vector3 (
			(armInfoContent.rectTransform.position.x - (armInfoContent.rectTransform.sizeDelta.x - patrolAreaValueRect.sizeDelta.x)*0.5f + Screen.height*0.262f), 
			(patrolArea.rectTransform.position.y), 
			patrolAreaValueRect.position.z);
		patrolAreaValue.transform.FindChild ("Label").GetComponent<Text>().fontSize = textSize1;
		patrolAreaValue.transform.FindChild ("Arrow").GetComponent<RectTransform>().sizeDelta = new Vector2 (textSize1*1.8f, textSize1*1.8f);
		patrolAreaValue.transform.FindChild ("Arrow").GetComponent<RectTransform>().localPosition = new Vector3 ((patrolAreaValueRect.sizeDelta.x - textSize1*1.8f)*0.5f, 0, 0);
		RectTransform patrolAreaValueItemRect = patrolAreaValue.transform.FindChild ("Template/Viewport/Content/Item").GetComponent<RectTransform> ();
		patrolAreaValueItemRect.sizeDelta = new Vector2 (patrolAreaValueItemRect.sizeDelta.x, textSize1*2f);
		RectTransform patrolAreaValueItemRect1 = patrolAreaValue.transform.FindChild ("Template/Viewport/Content/Item/Item Checkmark").GetComponent<RectTransform> ();
		patrolAreaValueItemRect1.sizeDelta = new Vector2 (textSize1*1.6f, textSize1*1.6f);//
		patrolAreaValueItemRect1.position = new Vector3 ((patrolAreaValueRect.position.x - (patrolAreaValueRect.sizeDelta.x - textSize1*1.6f)*0.5f), 
			patrolAreaValueItemRect1.position.y,patrolAreaValueItemRect1.position.z);
		RectTransform patrolAreaValueItemRect2 = patrolAreaValue.transform.FindChild ("Template/Viewport/Content/Item/Item Label").GetComponent<RectTransform> ();
		patrolAreaValueItemRect2.localPosition = new Vector3 (textSize1*0.8f,patrolAreaValueItemRect2.localPosition.y,patrolAreaValueItemRect2.localPosition.z);
		patrolAreaValue.transform.FindChild ("Template/Viewport/Content/Item/Item Label").GetComponent<Text> ()
			.fontSize = textSize1;


		
		RectTransform attackArmyValueRect = attackArmyValue.GetComponent<RectTransform> ();
		attackArmyValueRect.sizeDelta = new Vector2 (armInfoContent.rectTransform.sizeDelta.y * 0.2f, armInfoContent.rectTransform.sizeDelta.y * 0.2f);
		attackArmyValueRect.position = new Vector3 (
			(armInfoContent.rectTransform.position.x + (armInfoContent.rectTransform.sizeDelta.x - attackArmy.rectTransform.sizeDelta.x)*0.5f + Screen.height*0.12f), 
			(attackArmy.rectTransform.position.y - attackArmyValueRect.sizeDelta.x*0.5f), 
			attackArmyValueRect.position.z);
		attackArmyValue.transform.FindChild ("Background").GetComponent<RectTransform> ().sizeDelta = new Vector2(attackArmyValueRect.sizeDelta.x*0.7f, attackArmyValueRect.sizeDelta.y*0.7f);
		attackArmyValue.transform.FindChild("Background/Checkmark").GetComponent<RectTransform> ().sizeDelta = new Vector2(attackArmyValueRect.sizeDelta.x*0.7f, attackArmyValueRect.sizeDelta.y*0.7f);

		maxMountMissile.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.4f, armInfoContent.rectTransform.sizeDelta.y * 0.25f);
		maxMountMissile.rectTransform.position = new Vector3 (
			(armInfoContent.rectTransform.position.x - (armInfoContent.rectTransform.sizeDelta.x - maxMountMissile.rectTransform.sizeDelta.x)*0.5f + Screen.height*0.02f), 
			(armInfoContent.rectTransform.position.y + (armInfoContent.rectTransform.sizeDelta.y - maxMountMissile.rectTransform.sizeDelta.y*5)*0.5f), 
			maxMountMissile.rectTransform.position.z);
		maxMountMissile.fontSize = textSize;
		supplyPriority.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.4f, armInfoContent.rectTransform.sizeDelta.y * 0.25f);
		supplyPriority.rectTransform.position = new Vector3 (
			(armInfoContent.rectTransform.position.x + (armInfoContent.rectTransform.sizeDelta.x - supplyPriority.rectTransform.sizeDelta.x)*0.5f - Screen.height * 0.025f), 
			(armInfoContent.rectTransform.position.y + (armInfoContent.rectTransform.sizeDelta.y - supplyPriority.rectTransform.sizeDelta.y*5)*0.5f), 
			supplyPriority.rectTransform.position.z);
		supplyPriority.fontSize = textSize;


		RectTransform maxMountMissileValueRect = maxMountMissileValue.GetComponent<RectTransform> ();
		maxMountMissileValueRect.sizeDelta = new Vector2 (Screen.height * 0.14f, armInfoContent.rectTransform.sizeDelta.y * 0.17f);
		maxMountMissileValueRect.position = new Vector3 (
			(armInfoContent.rectTransform.position.x - (armInfoContent.rectTransform.sizeDelta.x - maxMountMissileValueRect.sizeDelta.x)*0.5f + Screen.height*0.298f), 
			(maxMountMissile.rectTransform.position.y), 
			maxMountMissileValueRect.position.z);
		maxMountMissileValue.transform.FindChild ("Label").GetComponent<Text>().fontSize = textSize1;
		maxMountMissileValue.transform.FindChild ("Arrow").GetComponent<RectTransform>().sizeDelta = new Vector2 (textSize1*1.8f, textSize1*1.8f);
		maxMountMissileValue.transform.FindChild ("Arrow").GetComponent<RectTransform>().localPosition = new Vector3 ((maxMountMissileValueRect.sizeDelta.x - textSize1*1.8f)*0.5f, 0, 0);

		RectTransform maxMountMissileItemRect = maxMountMissileValue.transform.FindChild ("Template/Viewport/Content/Item").GetComponent<RectTransform> ();
		maxMountMissileItemRect.sizeDelta = new Vector2 (maxMountMissileItemRect.sizeDelta.x, textSize1*2f);
		RectTransform maxMountMissileItemRect1 = maxMountMissileValue.transform.FindChild ("Template/Viewport/Content/Item/Item Checkmark").GetComponent<RectTransform> ();
		maxMountMissileItemRect1.sizeDelta = new Vector2 (textSize1*1.6f, textSize1*1.6f);//
		maxMountMissileItemRect1.position = new Vector3 ((maxMountMissileValueRect.position.x - (maxMountMissileValueRect.sizeDelta.x - textSize1*1.6f)*0.5f), 
			maxMountMissileItemRect1.position.y,maxMountMissileItemRect1.position.z);
		RectTransform maxMountMissileItemRect2 = maxMountMissileValue.transform.FindChild ("Template/Viewport/Content/Item/Item Label").GetComponent<RectTransform> ();
		maxMountMissileItemRect2.localPosition = new Vector3 (textSize1*0.8f,maxMountMissileItemRect2.localPosition.y,maxMountMissileItemRect2.localPosition.z);
		maxMountMissileValue.transform.FindChild ("Template/Viewport/Content/Item/Item Label").GetComponent<Text> ()
			.fontSize = textSize1;
		




		RectTransform supplyPriorityValueRect = supplyPriorityValue.GetComponent<RectTransform> ();
		supplyPriorityValueRect.sizeDelta = new Vector2 (Screen.height * 0.13f, armInfoContent.rectTransform.sizeDelta.y * 0.17f);
		supplyPriorityValueRect.position = new Vector3 (
			(armInfoContent.rectTransform.position.x + (armInfoContent.rectTransform.sizeDelta.x - supplyPriorityValueRect.sizeDelta.x)*0.5f - Screen.height*0.005f), 
			(supplyPriority.rectTransform.position.y), 
			supplyPriorityValueRect.position.z);
		supplyPriorityValue.transform.FindChild ("Label").GetComponent<Text>().fontSize = textSize1;
		supplyPriorityValue.transform.FindChild ("Arrow").GetComponent<RectTransform>().sizeDelta = new Vector2 (textSize1*1.8f, textSize1*1.8f);
		supplyPriorityValue.transform.FindChild ("Arrow").GetComponent<RectTransform>().localPosition = new Vector3 ((supplyPriorityValueRect.sizeDelta.x - textSize1*1.8f)*0.5f, 0, 0);
		RectTransform supplyPriorityItemRect = supplyPriorityValue.transform.FindChild ("Template/Viewport/Content/Item").GetComponent<RectTransform> ();
		supplyPriorityItemRect.sizeDelta = new Vector2 (supplyPriorityItemRect.sizeDelta.x, textSize1*2f);
		RectTransform supplyPriorityItemRect1 = supplyPriorityValue.transform.FindChild ("Template/Viewport/Content/Item/Item Checkmark").GetComponent<RectTransform> ();
		supplyPriorityItemRect1.sizeDelta = new Vector2 (textSize1*1.6f, textSize1*1.6f);//
		supplyPriorityItemRect1.position = new Vector3 ((supplyPriorityValueRect.position.x - (supplyPriorityValueRect.sizeDelta.x - textSize1*1.6f)*0.5f), 
			supplyPriorityItemRect1.position.y,supplyPriorityItemRect1.position.z);
		RectTransform supplyPriorityItemRect2 = supplyPriorityValue.transform.FindChild ("Template/Viewport/Content/Item/Item Label").GetComponent<RectTransform> ();
		supplyPriorityItemRect2.localPosition = new Vector3 (textSize1*0.8f,supplyPriorityItemRect2.localPosition.y,supplyPriorityItemRect2.localPosition.z);
		supplyPriorityValue.transform.FindChild ("Template/Viewport/Content/Item/Item Label").GetComponent<Text> ()
			.fontSize = textSize1;

		armInfoArms.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.4f, armInfoContent.rectTransform.sizeDelta.y * 0.125f);
		armInfoArms.rectTransform.position = new Vector3 (
			(armInfoContent.rectTransform.position.x), 
			(armInfoContent.rectTransform.position.y + (armInfoContent.rectTransform.sizeDelta.y - supplyPriority.rectTransform.sizeDelta.y*6.15f)*0.5f), 
			armInfoArms.rectTransform.position.z);
		armInfoArms.text = "hello kkp";
		armInfoArms.fontSize = (int)(textSize *0.6f);

		armInfoControl.rectTransform.sizeDelta = new Vector2 (armInfoContent.rectTransform.sizeDelta.y * 0.3465f, armInfoContent.rectTransform.sizeDelta.y * 0.22f);
		armInfoControl.rectTransform.position = new Vector3 (
			(armInfoContent.rectTransform.position.x  - Screen.height * 0.18f), 
			(armInfoContent.rectTransform.position.y + (armInfoContent.rectTransform.sizeDelta.y - supplyPriority.rectTransform.sizeDelta.y*7)*0.5f), 
			armInfoControl.rectTransform.position.z);
		
		armInfoSell.rectTransform.sizeDelta = new Vector2 (armInfoContent.rectTransform.sizeDelta.y * 0.3465f, armInfoContent.rectTransform.sizeDelta.y * 0.22f);
		armInfoSell.rectTransform.position = new Vector3 (
			(armInfoContent.rectTransform.position.x  + Screen.height * 0.18f), 
			(armInfoContent.rectTransform.position.y + (armInfoContent.rectTransform.sizeDelta.y - supplyPriority.rectTransform.sizeDelta.y*7)*0.5f), 
			armInfoSell.rectTransform.position.z);

		armInfoSellNote.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.4f, armInfoContent.rectTransform.sizeDelta.y * 0.22f);
		armInfoSellNote.rectTransform.position = new Vector3 (
			(armInfoSell.rectTransform.position.x  + armInfoSell.rectTransform.sizeDelta.x * 0.55f + armInfoSellNote.rectTransform.sizeDelta.x * 0.5f), 
			(armInfoSell.rectTransform.position.y), 
			armInfoSellNote.rectTransform.position.z);
		armInfoSellNote.text = "";
		armInfoSellNote.fontSize = (int)(textSize * 0.7f);


		armInfoPanel.GetComponent<RectTransform> ().localScale = new Vector3 (0,0,0);
	}

	void Update(){

		if(isSelectMode){
			listenerScreenTap ();
		}

		if(planMove.player == null){
			if(cameraThubmnailPoint.enabled){
				updatyXZ (Camera.main.transform, cameraThubmnailPoint);
			}
			if(isOnLeave){
				Camera.main.transform.position = Vector3.Slerp (Camera.main.transform.position, 
					new Vector3(Camera.main.transform.position.x, onLeaveHeight, Camera.main.transform.position.z), Time.deltaTime*0.45f);
			}
			Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, 
				Quaternion.Euler(90, 
					0,
					0), 
				Time.deltaTime * 1.2f);
		}

		if(isCamreaMoveTo || isCamreaHeightMove){
			Camera.main.transform.position = Vector3.Slerp (new Vector3(Camera.main.transform.position.x, frontCameraY, 
				Camera.main.transform.position.z), 
				new Vector3(currentCamreaMoveTo.x, cameraY, currentCamreaMoveTo.z), Time.deltaTime * 2);
			//Camera.main.transform.position = Vector3.Slerp (Camera.main.transform.position, currentCamreaMoveTo, Time.deltaTime * 2);

		}

		if(planMove.player != null && "a10".Equals(playerType) && isFireDown){
			PojulObject mPojulObject = planMove.player.gameObject.GetComponent<PojulObject> ();
			if (mPojulObject != null && (Time.time - lastFireTime) > 0.12f) {
				lastFireTime = Time.time;
				mPojulObject.fireOfPlayer ();
			}
		}

		if(missileAimedTra != null){
			missileAim.enabled = true;
			Vector2 screenPos = Camera.main.WorldToScreenPoint (missileAimedTra.position);
			missileAim.rectTransform.position = new Vector3 (screenPos.x, 
				screenPos.y, missileAim.rectTransform.position.z);
		} else {
			missileAim.enabled = false;
		}
	}

	void listenerScreenTap(){
		if(showArmInfo){
			return;
		}
		if (Application.isMobilePlatform) {
			if (Input.touchCount > 0 && Input.touches [0].phase == TouchPhase.Began) {
				touch1 = Input.touches [0].position;
				lastTouch1Time = Time.time;
				touch1Ended = false;
				//Debug.Log ("gqb------>touch Began x: " + Input.touches [0].position.x + "; y: " + Input.touches [0].position.y);
			}
			if (Input.touchCount > 1 && Input.touches [1].phase == TouchPhase.Began) {
				touch2 = Input.touches [1].position;
				lastTouch2Time = Time.time;
				touch2Ended = false;
			}
			if (Input.touchCount > 0 && Input.touches [0].phase == TouchPhase.Ended && !touch1Ended) {
				//Debug.Log ("gqb------>end touch x: " + Input.touches [0].position.x + "; y: " + Input.touches [0].position.y);
				onEndTouch (Input.touches [0].position);
				touch1Ended = true;
			}
			if (Input.touchCount > 1 && Input.touches [1].phase == TouchPhase.Ended && !touch2Ended) {
				onEndTouch (Input.touches [1].position);
				touch2Ended = true;
			}
		} else {
			if(Input.GetMouseButtonDown(0)){
				lastTouchTime = Time.time;
			}else if(Input.GetMouseButtonUp(0)){
				if((Time.time - lastTouchTime) < 0.3f){
					//Debug.Log ("gqb------>touch x: " + Input.mousePosition.x + "; y: " + Input.mousePosition.y + "; z: " + Input.mousePosition.z );
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					RaycastHit hit;
					if(Physics.Raycast(ray, out hit)){
						Vector3 v3 = hit.point;
						onScreenHit (hit);
						//Debug.Log (hit.collider.transform.name + "gqb------>touchworld x: " + v3.x + "; y: " + v3.y + "; z: " + v3.z );
					}
				}
			}
		}
	}

	void onEndTouch(Vector2 tempTouch){
		bool needRay = false;
		if((tempTouch - touch1).magnitude < 5 && (Time.time - lastTouch1Time) < 0.3f){
			needRay = true;
		}else if((tempTouch - touch2).magnitude < 5 && (Time.time - lastTouch2Time) < 0.3f){
			needRay = true;
		}
		if(needRay){
			Ray ray = Camera.main.ScreenPointToRay (new Vector3(tempTouch.x, tempTouch.y, 0));
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)){
				Vector3 v3 = hit.point;
				onScreenHit (hit);
				//Debug.Log (hit.collider.transform.name + "gqb------>touchworld x: " + v3.x + "; y: " + v3.y + "; z: " + v3.z );
			}
		}
	}

	void onScreenHit(RaycastHit hit){

		Vector3 hitPoint = new Vector3 (hit.point.x, 6000, hit.point.z);

		if (!hit.transform.root.CompareTag ("Untagged") ||
		   (hit.transform.root.childCount > 0 && !hit.transform.root.GetChild (0).CompareTag ("Untagged"))) {
			Transform root = hit.transform.root.GetChild(0);
			PojulObject mPojulObject = root.GetComponent<PojulObject> ();
			if(mPojulObject == null && hit.transform != null){
				root = hit.transform;
				mPojulObject = root.gameObject.GetComponent<PojulObject> ();
			}
			if(mPojulObject != null){
				onArmSelected (root);
				//Debug.Log (hit.collider.transform.name + "gqb------>onScreenHit1");
			}
		} else {
			Collider[] colliders = Physics.OverlapSphere (hitPoint, 7000);
			float minDistance = 100000;
			Transform selected = null;
			for (int i = 0; i < colliders.Length; i++) {
				if (colliders [i].transform.root.childCount <= 0) {
					continue;
				}
				Transform tempTransform = colliders [i].transform.root.GetChild (0);
				string tag = tempTransform.tag;
				if (tag.Equals ("Untagged")) {
					if (colliders [i].transform == null) {
						continue;
					}
					if (colliders [i].transform.tag.Equals ("Untagged")) {
						continue;
					}
					tempTransform = colliders [i].transform;
				}
				tag = tempTransform.tag;
				string[] strs = tempTransform.tag.Split ('_');
				//Debug.Log (tag + "gqb------>onScreenHit0000");
				if (strs.Length == 2) {
					float tempDistance = (tempTransform.position - hit.point).magnitude;
					//Debug.Log ("1".Equals(tempTransform.GetComponent<PojulObject>().playerType) + "gqb------>onScreenHit0000: " + "0".Equals(strs[0]) 
						//+ ":::" + tempTransform.GetComponent<PojulObject>());
					if ("0".Equals(strs[0]) && tempTransform.GetComponent<PojulObject>() && 
							tempTransform.GetComponent<PojulObject>().playerType == 1 && tempDistance < minDistance) {
						selected = tempTransform;
						minDistance = tempDistance;
					}
				}
			}
			if(selected != null){
				onArmSelected (selected);
			}

		}
	}

	void onArmSelected(Transform tra){
		if(showShopWin){
			return;
		}
		if (tra == null) {
			return;
		}
		PojulObject mPojulObject;
		if(!tra.GetComponent<PojulObject>() || "1".Equals(tra.GetComponent<PojulObject>().playerId)
			|| tra.GetComponent<PojulObject>().playerType == 0 || tra.GetComponent<PojulObject>().isDestoryed){
			return;
		}

		mPojulObject = tra.GetComponent<PojulObject> ();
		if("car2".Equals(mPojulObject.type) || "car3".Equals(mPojulObject.type) || "car5".Equals(mPojulObject.type)
			|| "a10".Equals(mPojulObject.type)){
			if(selectedTra != null && selectedTra.GetComponent<PojulObject>()){
				selectedTra.GetComponent<PojulObject> ().isSelected = false;
			}
			selectedTra = tra;
			tra.GetComponent<PojulObject> ().isSelected = true;
			armInfoPanel.GetComponent<RectTransform> ().localScale = new Vector3 (1,1,1);
			showArmInfo = true;

			if(mPojulObject.isSelling){
				armInfoSell.sprite = armInfoSell3;
			}else if(sellingTra != null){
				armInfoSell.sprite = armInfoSell4;
			}else{
				armInfoSell.sprite = armInfoSell1;
			}

			if("car2".Equals(mPojulObject.type)){
				armInfoType.text = "type: supply car";
				health.text = "health: " + ((CarType2)mPojulObject).sliderHealth.value;
				patrolArea.color = textDisableColor;
				patrolAreaValue.gameObject.GetComponent<Image> ().color = textDisableColor;
				patrolAreaValue.enabled = false;
				attackArmy.color = textDisableColor;
				attackArmyValue.transform.FindChild("Background").GetComponent<Image>().color = textDisableColor;
				attackArmyValue.enabled = false;
				maxMountMissile.color = textDisableColor;
				maxMountMissileValue.gameObject.GetComponent<Image>().color = textDisableColor;
				maxMountMissileValue.enabled = false;
				supplyPriority.color = textDisableColor;
				supplyPriorityValue.gameObject.GetComponent<Image>().color = textDisableColor;
				supplyPriorityValue.enabled = false;
				armInfoArms.text = ((CarType2)mPojulObject).currentMissiles[0] + " S-A missiles, "
					+ ((CarType2)mPojulObject).currentMissiles[2] + " F missiles";
				armInfoSellNote.text = (int)(GameInit.prices["car2"]*0.5f*((CarType2)mPojulObject).sliderHealth.value/((CarType2)mPojulObject).sliderHealth.maxValue)
					+ " golds";
				armInfoControl.sprite = armInfoControl3;
			}else if("car3".Equals(mPojulObject.type)){
				armInfoType.text = "type: tank";
				health.text = "health: " + ((CarType3)mPojulObject).sliderHealth.value;
				patrolArea.color = textEnableColor;
				patrolAreaValue.gameObject.GetComponent<Image> ().color = Color.white;
				patrolAreaValue.enabled = true;
				if(((CarType3)mPojulObject).mPatrolArea.areaId <= 4){
					patrolAreaValue.value = ((CarType3)mPojulObject).mPatrolArea.areaId -1;
				}
				attackArmy.color = textEnableColor;
				attackArmyValue.transform.FindChild("Background").GetComponent<Image>().color = Color.white;
				attackArmyValue.enabled = true;
				attackArmyValue.isOn = mPojulObject.isAttackArmy;
				maxMountMissile.color = textDisableColor;
				maxMountMissileValue.gameObject.GetComponent<Image>().color = textDisableColor;
				maxMountMissileValue.enabled = false;
				supplyPriority.color = textDisableColor;
				supplyPriorityValue.gameObject.GetComponent<Image>().color = textDisableColor;
				supplyPriorityValue.enabled = false;
				armInfoArms.text = "";
				armInfoSellNote.text = (int)(GameInit.prices["car3"]*0.5f*((CarType3)mPojulObject).sliderHealth.value/((CarType3)mPojulObject).sliderHealth.maxValue)
					+ " golds";
				if (planMove.player != null) {
					armInfoControl.sprite = armInfoControl3;
				} else {
					armInfoControl.sprite = armInfoControl1;
				}
			}else if("car5".Equals(mPojulObject.type)){
				armInfoType.text = "type: missile car";
				health.text = "health: " + ((CarType5)mPojulObject).sliderHealth.value;
				patrolArea.color = textEnableColor;
				patrolAreaValue.gameObject.GetComponent<Image> ().color = Color.white;
				patrolAreaValue.enabled = true;
				if(((CarType5)mPojulObject).mPatrolArea.areaId <= 4){
					patrolAreaValue.value = ((CarType5)mPojulObject).mPatrolArea.areaId -1;
				}
				attackArmy.color = textEnableColor;
				attackArmyValue.transform.FindChild("Background").GetComponent<Image>().color = Color.white;
				attackArmyValue.enabled = true;
				attackArmyValue.isOn = mPojulObject.isAttackArmy;
				maxMountMissile.color = textEnableColor;
				maxMountMissileValue.gameObject.GetComponent<Image>().color = Color.white;
				maxMountMissileValue.enabled = true;
				maxMountMissileValue.value = ((CarType5)mPojulObject).maxMountMissle;
				supplyPriority.color = textEnableColor;
				supplyPriorityValue.gameObject.GetComponent<Image>().color = Color.white;
				supplyPriorityValue.enabled = true;
				supplyPriorityValue.value = ((CarType5)mPojulObject).priority;
				armInfoArms.text = ((CarType5)mPojulObject).currentMountMissle + " S-A missiles, ";

				armInfoSellNote.text = (int)(GameInit.prices["car5"]*0.5f*((CarType5)mPojulObject).sliderHealth.value/((CarType5)mPojulObject).sliderHealth.maxValue)
					+ " golds";
				armInfoControl.sprite = armInfoControl3;
			}else if("a10".Equals(mPojulObject.type)){
				armInfoType.text = "type: fighter";
				health.text = "health: " + ((A10aPlan)mPojulObject).sliderHealth.value;
				patrolArea.color = textEnableColor;
				patrolAreaValue.gameObject.GetComponent<Image> ().color = Color.white;
				patrolAreaValue.enabled = true;
				if(((A10aPlan)mPojulObject).mPatrolArea.areaId <= 4){
					patrolAreaValue.value = ((A10aPlan)mPojulObject).mPatrolArea.areaId -1;
				}
				attackArmy.color = textEnableColor;
				attackArmyValue.transform.FindChild("Background").GetComponent<Image>().color = Color.white;
				attackArmyValue.enabled = true;
				attackArmyValue.isOn = mPojulObject.isAttackArmy;
				maxMountMissile.color = textEnableColor;
				maxMountMissileValue.gameObject.GetComponent<Image>().color = Color.white;
				maxMountMissileValue.enabled = true;
				maxMountMissileValue.value = ((A10aPlan)mPojulObject).maxMountMissle;
				supplyPriority.color = textDisableColor;
				supplyPriorityValue.gameObject.GetComponent<Image>().color = textDisableColor;
				supplyPriorityValue.enabled = false;
				armInfoArms.text = ((A10aPlan)mPojulObject).currentMountMissle + " F missiles";

				armInfoSellNote.text = (int)(GameInit.prices["a10"]*0.5f*((A10aPlan)mPojulObject).sliderHealth.value/((A10aPlan)mPojulObject).sliderHealth.maxValue)
					+ " golds";
				if (planMove.player != null) {
					armInfoControl.sprite = armInfoControl3;
				} else {
					armInfoControl.sprite = armInfoControl1;
				}
			}
		}
	}
		

	void updateThubmnail(){

		lock (GameInit.locker) {
			//selfs
			for(int i = 0; i < GameInit.myThumbnailObjs.Count; i++){
				if(GameInit.myThumbnailObjs[i] == null){
					continue;
				}
				PojulObject mPojulObject = GameInit.myThumbnailObjs [i].GetComponent<PojulObject> ();
				if(mPojulObject == null){
					continue;
				}
				string[] strs = GameInit.myThumbnailObjs [i].tag.Split ('_');
				if(strs.Length != 2){
					continue;
				}
				if(!thubmnailPoints.ContainsKey(GameInit.myThumbnailObjs[i].GetHashCode())){
					Image temp = (Image)Instantiate (thubmnailPoint);
					object[] objs = new object[2];
					objs [0] = GameInit.myThumbnailObjs[i];	
					objs [1] = temp;
					thubmnailPoints.Add (GameInit.myThumbnailObjs[i].GetHashCode(), objs);
					temp.GetComponent<Transform> ().SetParent (mainUI.GetComponent<Transform> (), true);
					temp.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.006f, Screen.height * 0.006f);
				}
				object[] tempObjs = thubmnailPoints[GameInit.myThumbnailObjs [i].GetHashCode ()];
				if(tempObjs[0] == null || tempObjs[1] == null){
					continue;
				}
				Image tempImage = (Image)tempObjs [1];
				if(mPojulObject.playerType == 0){
					tempImage.sprite = yellowThubmnailPoint;
				}else if(mPojulObject.playerType == 1){
					if (mPojulObject.isSelected) {
						tempImage.sprite = selectedThubmnailPoint;
					} else {
						tempImage.sprite = blueThubmnailPoint;
					}
				}
				updatyXZ (GameInit.myThumbnailObjs[i], 0);
			}
				
			int[] keys = new int[GameInit.allNearEnemys_0.Keys.Count];
			GameInit.allNearEnemys_0.Keys.CopyTo (keys, 0);
			for (int i = 0; i < keys.Length; i++) {
				object[] allNearEnemysObjs = GameInit.allNearEnemys_0 [keys [i]];
				Transform allNearEnemyTra = (Transform)allNearEnemysObjs [0];
				if(allNearEnemyTra == null){
					continue;
				}
				GameObject allNearEnemyObj = allNearEnemyTra.gameObject;
				float allNearEnemyTimes = (float)allNearEnemysObjs [1];
				if(allNearEnemyObj == null){
					continue;
				}
				if ((Time.time - allNearEnemyTimes) < 3.6f) {
					if (!enemythubmnailPoints.ContainsKey (allNearEnemyObj.GetHashCode())) {
						Image temp = (Image)Instantiate (thubmnailPoint);
						object[] objs = new object[2];
						objs [0] = allNearEnemyObj;
						objs [1] = temp;
						enemythubmnailPoints.Add (allNearEnemyObj.GetHashCode(), objs);
						temp.GetComponent<Transform> ().SetParent (mainUI.GetComponent<Transform> (), true);
						temp.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.006f, Screen.height * 0.006f);
						temp.sprite = redThubmnailPoint;
					} 
					updatyXZ (allNearEnemyObj, 1);
				} else {
					if(enemythubmnailPoints.ContainsKey(allNearEnemyObj.GetHashCode())){
						object[] objs = enemythubmnailPoints [allNearEnemyObj.GetHashCode()];
						Image tempImage = (Image)objs [1];
						if(tempImage == null){
							return;
						}
						Destroy (tempImage.gameObject);
						enemythubmnailPoints.Remove (allNearEnemyObj.GetHashCode());
					}
				}
			}
		}

	}

	void updatyXZ(Transform key, Image img){
		float x = key.position.x;
		float y = key.position.z;
		if(Mathf.Abs(x) > 120000 || Mathf.Abs(y) > 120000){
			return;
		}
		float pointX = 0.0f;
		float pointY = 0.0f;
		pointX = thubmnail.rectTransform.sizeDelta.x * (120000 + x)*1.0f / 240000;
		pointY =Screen.height - thubmnail.rectTransform.sizeDelta.y * (120000 - y)*1.0f / 240000;
		img.rectTransform.position = new Vector3(pointX, pointY, img.rectTransform.position.z);
	}

	void updatyXZ(GameObject key, int type){
		Dictionary<int, object[]> tempThubmnailPoints = null;
		if(type == 0){
			tempThubmnailPoints = thubmnailPoints;
		}else if(type ==1){
			tempThubmnailPoints = enemythubmnailPoints;
		}

		if(key == null || !tempThubmnailPoints.ContainsKey(key.GetHashCode()) ||tempThubmnailPoints[key.GetHashCode()] == null){
			return;
		}
		float x = key.transform.position.x;
		float y = key.transform.position.z;
		if(Mathf.Abs(x) > 120000 || Mathf.Abs(y) > 120000){
			return;
		}
		float pointX = 0.0f;
		float pointY = 0.0f;
		pointX = thubmnail.rectTransform.sizeDelta.x * (120000 + x)*1.0f / 240000;
		pointY =Screen.height - thubmnail.rectTransform.sizeDelta.y * (120000 - y)*1.0f / 240000;
		object[] objs = tempThubmnailPoints [key.GetHashCode ()];
		if(objs[0] == null || objs[1] == null){
			return;
		}
		Image tempImage = (Image) objs[1];
		tempImage.rectTransform.position = new Vector3(pointX, pointY, tempImage.rectTransform.position.z);

	}

	void clearNullThubmnail(){
		lock(GameInit.locker){
			int[] keys = new int[thubmnailPoints.Keys.Count];
			thubmnailPoints.Keys.CopyTo (keys, 0);
			for (int i = 0; i < keys.Length; i++) {
				object[] objs = thubmnailPoints[keys[i]];
				if(objs == null){
					thubmnailPoints.Remove (keys[i]);
					continue;
				}
				GameObject mGameObject = (GameObject)objs [0];
				if(mGameObject == null){
					Image mImage = (Image)objs [1];
					if(mImage != null){
						Destroy (mImage.gameObject);
					}
					thubmnailPoints.Remove (keys[i]);
				}
			}

			keys = new int[enemythubmnailPoints.Keys.Count];
			enemythubmnailPoints.Keys.CopyTo (keys, 0);
			for (int i = 0; i < keys.Length; i++) {
				object[] objs = enemythubmnailPoints[keys[i]];
				if(objs == null){
					enemythubmnailPoints.Remove (keys[i]);
					continue;
				}
				GameObject mGameObject = (GameObject)objs [0];
				if(mGameObject == null){
					Image mImage = (Image)objs [1];
					if(mImage != null){
						Destroy (mImage.gameObject);
					}
					enemythubmnailPoints.Remove (keys[i]);
				}
			}
		}
	}

	public void AcceleratePointerUp(BaseEventData data){
		if(planMove.player == null  && accelerate.enabled){
			//accelerate.value = accelerate.maxValue * 0.5f;
			isCamreaHeightMove = false;
			return;
		}
		PointerEventData data1 = data as PointerEventData;
		if(accelerate != null && accelerate.enabled){
			accelerate.value = accelerate.maxValue * 0.5f;
			planMove.accelerate = 0;
		}
		//Debug.Log (data1.selectedObject + "gqb------>AcceleratePointerUp: " + data1.position);
	}

	public void AcceleratePointerMove(BaseEventData data){
		if(planMove.player == null){
			//Debug.Log ("gqb------>AcceleratePointerMove dy: " + dy);
			//if(/*dy > 200 && */dy < 21800){
			isCamreaHeightMove = true;
			isOnLeave = false;
			if(!isCamreaMoveTo){
				currentCamreaMoveTo = new Vector3 (Camera.main.transform.position.x, cameraY, Camera.main.transform.position.z);
			}
			frontCameraY = cameraY;
			cameraY = accelerate.value * 120 + 100;

			return;
		}
		PointerEventData data1 = data as PointerEventData;
		if(accelerate != null && accelerate.enabled){
			planMove.accelerate = planMove.maxAccelerate * (accelerate.value - accelerate.maxValue * 0.5f)  /(accelerate.maxValue * 0.5f);
		}
		//Debug.Log (data1.selectedObject + "gqb------>AcceleratePointerUp: " + data1.position);
	}

	public void OnFireMissileClick(){
		//Debug.Log ("gqb------>OnFireMissileClick");
		if(isSelectMode || planMove.player == null || missileAimedTra == null || planMove.currentMountMissle <= 0){
			return;
		}
		PojulObject mPojulObject = planMove.player.gameObject.GetComponent<PojulObject> ();
		if(mPojulObject == null){
			return;
		}
		mPojulObject.fireMissileOfPlayer (missileAimedTra);
	}

	public void OnFireMissileDown(){
		if(isSelectMode || missileAimedTra == null || planMove.currentMountMissle <= 0){
			return;
		}
		fireMissile.sprite = fireMissileBg2;
	}

	public void OnFireMissileUp(){
		if(isSelectMode || missileAimedTra == null || planMove.currentMountMissle <= 0){
			return;
		}
		fireMissile.sprite = fireMissileBg1;
	}

	public void OnAimNextClick(){
		//Debug.Log ("gqb------>OnAimNextClick");
		if(isSelectMode || planMove.player == null || missileAimedTra == null){
			return;
		}
		int index = planMove.nearEnemy.IndexOf (missileAimedTra);
		if(index < 0 || index >= planMove.nearEnemy.Count ){
			return;
		}

		if((index + 1) < planMove.nearEnemy.Count ){
			for(int i = (index + 1); i<  planMove.nearEnemy.Count; i++){
				if(planMove.nearEnemy[i] != null && planMove.nearEnemy[i] != missileAimedTra){
					float[] rotations = getMissileAimRotation(planMove.nearEnemy[i]);
					if (rotations[0] <= 30 && rotations[1] <= 45) {
						missileAimedTra = planMove.nearEnemy [i];
						return;
					}
				}
			}
		}

		for(int i = 0; i< index; i++){
			if(planMove.nearEnemy[i] != null && planMove.nearEnemy[i] != missileAimedTra){
				float[] rotations = getMissileAimRotation(planMove.nearEnemy[i]);
				if (rotations[0] <= 30 && rotations[1] <= 45) {
					missileAimedTra = planMove.nearEnemy [i];
					return;
				}
			}
		}

	}

	public void OnAimNextDown(){
		if(isSelectMode){
			return;
		}
		aimNext.sprite = aimNextBg2;
	}

	public void OnAimNextUp(){
		if(isSelectMode){
			return;
		}
		aimNext.sprite = aimNextBg1;
	}

	public void OnFireClick(){
		if(isSelectMode){
			return;
		}
		if(planMove.player != null && "car3".Equals(playerType) && canFire){
			PojulObject mPojulObject = planMove.player.gameObject.GetComponent<PojulObject> ();
			if(mPojulObject != null){
				canFire = false;
				Invoke ("car3CanFire", 2);
				fire.sprite = fireBg3;
				mPojulObject.fireOfPlayer ();
			}
		}
		//Debug.Log ("gqb------>OnFireClick");
	}

	public void OnFireDown(){
		if(isSelectMode){
			return;
		}
		if(!canFire){
			return;
		}
		fire.sprite = fireBg2;
		isFireDown = true;
	}

	public void OnFireUp(){
		if(isSelectMode){
			return;
		}
		if(!canFire){
			return;
		}
		fire.sprite = fireBg1;
		isFireDown = false;
	}

	void car3CanFire(){
		fire.sprite = fireBg1;
		canFire = true;
	}

	public void OnMselectClick(){
		isSelectMode = !isSelectMode;
		if (isSelectMode) {
			mselect.sprite = mselect2;
		} else {
			mselect.sprite = mselect1;
			if(selectedTra!= null && selectedTra.GetComponent<PojulObject>()){
				selectedTra.GetComponent<PojulObject> ().isSelected = false;
				selectedTra = null;
			}
		}
	}

	public void OnMselectDown(){
		mselect.sprite = mselect2;
	}

	public void OnMselectUp(){
		if (isSelectMode) {
			mselect.sprite = mselect2;
		} else {
			mselect.sprite = mselect1;
		}
	}

	public void OnBuyClick(){
		if(isSelectMode){
			return;
		}
		showShopWin = true;
		armShopPanel.rectTransform.localScale = new Vector3 (1, 1, 1);
		RectTransform tempRectTransform = armShopContent.GetComponent<RectTransform> ();
		tempRectTransform.position = new Vector3 (tempRectTransform.position.x, scrollPos,tempRectTransform.position.z);
	}

	public void OnBuyDown(){
		if(isSelectMode){
			return;
		}
		buy.sprite = buy2;
	}

	public void OnBuyUp(){
		if(isSelectMode){
			return;
		}
		buy.sprite = buy1;
	}

	public void OnThubmnailDown(BaseEventData rawdata){
		MainCameraMoveTo (((PointerEventData)rawdata).position);
		Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, cameraY, Camera.main.transform.position.z);
		Camera.main.transform.rotation =  Quaternion.Euler(90 , 
			0,
			0);
		isCamreaMoveTo = true;
	}

	public void OnThubmnailMove(BaseEventData rawdata){
		isCamreaMoveTo = true;
		if((Time.time - lastCamreaMoveToTime) > 0.05){
			MainCameraMoveTo (((PointerEventData)rawdata).position);
			lastCamreaMoveToTime = Time.time;
		}
	}

	public void OnThubmnailUp(BaseEventData rawdata){
		isCamreaMoveTo = false;
	}

	void MainCameraMoveTo(Vector2 position){
		float x = position.x * 240000.0f / thubmnail.rectTransform.sizeDelta.x - 120000;
		float z = 120000 - (Screen.height - position.y) * 240000.0f / thubmnail.rectTransform.sizeDelta.y;
		if(z > 0){
			return;
		}
		float distance = (Camera.main.transform.position - new Vector3 (x, Camera.main.transform.position.y, z)).magnitude;
		if(distance > 200){
			//Debug.Log ("gqb------>y: " + Camera.main.transform.position.y);
			currentCamreaMoveTo = new Vector3 (x, cameraY, z);
		}
	}

	public void OnTouchMoveDown(BaseEventData rawdata){
		if(planMove.player != null){
			return;
		}
		touchMoveDown = ((PointerEventData)rawdata).position;
	}

	public void OnTouchMoveMove(BaseEventData rawdata){
		if(planMove.player != null){
			return;
		}
		float dx = ((PointerEventData)rawdata).position.x - touchMoveDown.x;
		float dz = ((PointerEventData)rawdata).position.y - touchMoveDown.y;
		Camera.main.transform.position = new Vector3 ((Camera.main.transform.position.x - dx*4.5f),
			cameraY, (Camera.main.transform.position.z - dz*4.5f));
		touchMoveDown = ((PointerEventData)rawdata).position;
	}

	public void OnTouchMoveUp(BaseEventData rawdata){
		if(planMove.player != null){
			return;
		}
	}


	void updateMissileAim(){

		updateBuyArmStatus ();
		updateArmInfoStatus ();

		if (planMove.player == null) {
			missileAimedTra = null;
			fireMissile.sprite = fireMissileBg3;
			return;
		}

		if(missileAimedTra != null){
			float[] rotations = getMissileAimRotation(missileAimedTra);
			if (rotations[0] > 30 || rotations[1] > 45) {
				missileAimedTra = null;
			}

			if(missileAimedTra != null && !planMove.nearEnemy.Contains(missileAimedTra)){
				missileAimedTra = null;
			}
		}

		if(missileAimedTra != null){
			return;
		}

		for(int i =0; i< planMove.nearEnemy.Count; i++){
			if(planMove.nearEnemy[i] != null){
				float[] rotations = getMissileAimRotation(planMove.nearEnemy[i]);
				if (rotations[0] <= 30 && rotations[1] <= 45) {
					missileAimedTra = planMove.nearEnemy [i];
					if(planMove.currentMountMissle > 0){
						fireMissile.sprite = fireMissileBg1;
					}
					break;
				}
			}
		}

		if(missileAimedTra == null){
			fireMissile.sprite = fireMissileBg3;
		}

	}

	void updateBuyArmStatus(){
		
		if(showShopWin){
			for(int i = 0; i < armShopImgs.Length; i++){
				string str = GameInit.prices[armNames[i]] + "/" + GameInit.MyMoney.ToString ();
				if ((int)GameInit.currentInstance [("0_" + armNames [i])] >= (int)GameInit.maxInstance [("0_" + armNames [i])]) {
					couldArmBuy [i] = false;
					str = str + " max number";
					armShopBuys [i].sprite = armShopBuy3;
				} else if ((int)GameInit.prices [armNames [i]] > GameInit.MyMoney) {
					couldArmBuy [i] = false;
					str = str + " not enough gold";
					armShopBuys [i].sprite = armShopBuy3;
				} else {
					couldArmBuy [i] = true;
					if(armShopBuys [i].sprite == armShopBuy3){
						armShopBuys [i].sprite = armShopBuy1;
					}
				}
				armShopPrices [i].text = str;
			}
		}
	}

	void updateArmInfoStatus(){
		int leftTime = 0;
		if(sellingTra != null && sellingTra.GetComponent<PojulObject>()){
			leftTime = 4 - (int)(Time.time - sellingInv);
			if(leftTime <= 0){
				PojulObject mPojulObject = sellingTra.GetComponent<PojulObject> ();
				int sellMoney = 0;
				if ("car2".Equals (mPojulObject.type)) {
					sellMoney = (int)(GameInit.prices ["car2"] * 0.5f * ((CarType2)mPojulObject).sliderHealth.value / ((CarType2)mPojulObject).sliderHealth.maxValue);
					((CarType2)mPojulObject).destoryData ();
					((CarType2)mPojulObject).destoryAll ();
				} else if ("car3".Equals (mPojulObject.type)) {
					sellMoney = (int)(GameInit.prices ["car3"] * 0.5f * ((CarType3)mPojulObject).sliderHealth.value / ((CarType3)mPojulObject).sliderHealth.maxValue);
					((CarType3)mPojulObject).destoryData ();
					((CarType3)mPojulObject).destoryAll ();
				} else if ("car5".Equals (mPojulObject.type)) {
					sellMoney = (int)(GameInit.prices ["car5"] * 0.5f * ((CarType5)mPojulObject).sliderHealth.value / ((CarType5)mPojulObject).sliderHealth.maxValue);
					((CarType5)mPojulObject).destoryData ();
					((CarType5)mPojulObject).destoryAll ();
				} else if ("a10".Equals (mPojulObject.type)) {
					sellMoney = (int)(GameInit.prices ["a10"] * 0.5f * ((A10aPlan)mPojulObject).sliderHealth.value / ((A10aPlan)mPojulObject).sliderHealth.maxValue);
					((A10aPlan)mPojulObject).destoryData ();
					((A10aPlan)mPojulObject).destoryAll ();
				}
				GameInit.MyMoney = GameInit.MyMoney + sellMoney;
				sellingTra = null;

				if(showArmInfo && selectedTra != null){
					armInfoSell.sprite = armInfoSell1;
				}

			}
		}

		if(selectedTra == null && showArmInfo){
			armInfoPanel.rectTransform.localScale = new Vector3 (0,0,0);
			showArmInfo = false;
		}

		if(showArmInfo && selectedTra != null && selectedTra.GetComponent<PojulObject>()){
			PojulObject mPojulObject = selectedTra.GetComponent<PojulObject> ();
			if(mPojulObject.isSelling){
				string text = "";
				if ("car2".Equals (mPojulObject.type)) {
					text = (int)(GameInit.prices["car2"]*0.5f*((CarType2)mPojulObject).sliderHealth.value/((CarType2)mPojulObject).sliderHealth.maxValue) + " golds";
				} else if ("car3".Equals (mPojulObject.type)) {
					text = (int)(GameInit.prices["car3"]*0.5f*((CarType3)mPojulObject).sliderHealth.value/((CarType3)mPojulObject).sliderHealth.maxValue) + " golds";
				}else if ("car5".Equals (mPojulObject.type)) {
					text = (int)(GameInit.prices["car5"]*0.5f*((CarType5)mPojulObject).sliderHealth.value/((CarType5)mPojulObject).sliderHealth.maxValue) + " golds";
				}else if ("a10".Equals (mPojulObject.type)) {
					text = (int)(GameInit.prices["a10"]*0.5f*((A10aPlan)mPojulObject).sliderHealth.value/((A10aPlan)mPojulObject).sliderHealth.maxValue) + " golds";
				}
				text = text + sellingNotes[sellingNoteIndex] + leftTime;
				armInfoSellNote.text = text;
				sellingNoteIndex = (sellingNoteIndex + 1) % 3;
			}
		}
	}

	float[] getMissileAimRotation(Transform target){
		if(planMove.player == null){
			return new float[]{ 1000, 1000 };
		}

		Quaternion angle = Quaternion.LookRotation (target.position - planMove.player.position);
		Quaternion angle1 = Quaternion.LookRotation (planMove.player.forward);
		float dy1 = Mathf.Abs (angle1.eulerAngles.y - angle.eulerAngles.y);
		float dy2 = 360 - dy1;
		float dx1 = Mathf.Abs (angle1.eulerAngles.x - angle.eulerAngles.x);
		float dx2 = 360 - dx1;

		float dx = Mathf.Min (dx1, dx2);
		float dy = Mathf.Min(dy1, dy2);

		float[] rotations = new float[]{ dx, dy };

		return rotations;
	}

	public void setPlayerUI(string type){
		playerType = type;
		if("a10".Equals(type)){
			fireMissile.rectTransform.localScale = new Vector3 (1,1,1);
			aimNext.rectTransform.localScale = new Vector3 (1, 1, 1);
		}else if("car3".Equals(type)){
			fireMissile.rectTransform.localScale = new Vector3 (0,0,0);
			aimNext.rectTransform.localScale = new Vector3 (0, 0, 0);
		}
	}

	public void OnArmShopCloseClick(){
		armShopPanel.rectTransform.localScale = new Vector3 (0, 0, 0);
		showShopWin = false;
	}

	public void OnArmShopCloseDown(){
		if(armShopClose.enabled){
			armShopClose.sprite = armShopClose2;
		}
	}

	public void OnArmShopCloseUp(){
		armShopClose.sprite = armShopClose1;
	}

	public void OnArmShopItemClick(GameObject obj){
		int index = armShopBuyObjs.IndexOf (obj);
		if(index < 0 || !couldArmBuy[index]){
			return;
		}
		GameInit.instanceGameobject ("0", armNames[index]);
		GameInit.MyMoney = GameInit.MyMoney - (int)GameInit.prices [armNames [index]];
		updateBuyArmStatus ();
	}

	public void OnArmShopItemDown(GameObject obj){
		int index = armShopBuyObjs.IndexOf (obj);
		if(index < 0 || !couldArmBuy[index]){
			return;
		}
		armShopBuys [index].sprite = armShopBuy2;
	}

	public void OnArmShopItemUp(GameObject obj){
		int index = armShopBuyObjs.IndexOf (obj);
		if(index < 0 || !couldArmBuy[index]){
			return;
		}
		armShopBuys [index].sprite = armShopBuy1;
	}

	public void OnArmInfoCloseClick(){
		//Debug.Log ("gqb------>OnArmInfoCloseClick");
		armInfoPanel.GetComponent<RectTransform> ().localScale = new Vector3 (0,0,0);
		showArmInfo = false;
		if(selectedTra != null && selectedTra.GetComponent<PojulObject>()){
			selectedTra.GetComponent<PojulObject> ().isSelected = false;
		}
	}

	public void OnArmInfoCloseDown(){
		armInfoClose.sprite = armInfoClose2;
	}

	public void OnArmInfoCloseUp(){
		armInfoClose.sprite = armInfoClose1;
	}

	public void OnArmInfoControlClick(){
		if(armInfoControl.sprite == armInfoControl3){
			return;
		}
		if(selectedTra == null || !selectedTra.GetComponent<PojulObject>() ||
			selectedTra.GetComponent<PojulObject>().isDestoryed){
			return;
		}
		PojulObject mPojulObject = selectedTra.GetComponent<PojulObject> ();
		if ("car3".Equals (mPojulObject.type)) {
			((CarType3)mPojulObject).setPlayType (0);
		}else if("a10".Equals (mPojulObject.type)){
			((A10aPlan)mPojulObject).setPlayType (0);
		}

	}

	public void OnArmInfoControlDown(){
		if(armInfoControl.sprite == armInfoControl3){
			return;
		}
		armInfoControl.sprite = armInfoControl2;
	}

	public void OnArmInfoControlUp(){
		if(armInfoControl.sprite == armInfoControl3){
			return;
		}
		armInfoControl.sprite = armInfoControl1;
	}

	public void OnArmInfoSellClick(){
		if(sellingTra != null || selectedTra == null || !selectedTra.GetComponent<PojulObject>() ||
			selectedTra.GetComponent<PojulObject>().isDestoryed || selectedTra.GetComponent<PojulObject>().isSelling){
			return;
		}
		armInfoSell.sprite = armInfoSell3;
		selectedTra.GetComponent<PojulObject> ().isSelling = true;
		sellingInv = Time.time;
		sellingNoteIndex = 0;
		sellingTra = selectedTra;
	}

	public void OnArmInfoSellDown(){
		if(sellingTra != null || selectedTra == null || !selectedTra.GetComponent<PojulObject>() ||
			selectedTra.GetComponent<PojulObject>().isDestoryed || selectedTra.GetComponent<PojulObject>().isSelling){
			return;
		}
		armInfoSell.sprite = armInfoSell2;
	}

	public void OnArmInfoSellUp(){
		if(sellingTra != null || selectedTra == null || !selectedTra.GetComponent<PojulObject>() ||
			selectedTra.GetComponent<PojulObject>().isDestoryed || selectedTra.GetComponent<PojulObject>().isSelling){
			return;
		}
		armInfoSell.sprite = armInfoSell1;
	}

	public void OnPatrolAreaChanged(){
		if(selectedTra == null || !selectedTra.GetComponent<PojulObject>() || selectedTra.GetComponent<PojulObject>().isDestoryed){
			return;
		}
		PojulObject mPojulObject = selectedTra.GetComponent<PojulObject> ();
		if("car3".Equals(mPojulObject.type) && (((CarType3)mPojulObject).mPatrolArea.areaId -1) != patrolAreaValue.value){
			((CarType3)mPojulObject).mPatrolArea = new RadiusArea((patrolAreaValue.value + 1));
			if(mPojulObject.behavior == 1){
				((CarType3)mPojulObject).startNav (((CarType3)mPojulObject).mPatrolArea.getRandomPoint());
			}
		}else if("car5".Equals (mPojulObject.type) && (((CarType5)mPojulObject).mPatrolArea.areaId -1) != patrolAreaValue.value){
			((CarType5)mPojulObject).mPatrolArea = new RadiusArea((patrolAreaValue.value + 1));
			((CarType5)mPojulObject).mPatrolArea.maxRange = 40000;
			((CarType5)mPojulObject).mPatrolArea.minRange = 20000;
			if(mPojulObject.behavior == 1){
				((CarType5)mPojulObject).startNav (((CarType5)mPojulObject).mPatrolArea.getRandomPoint());
			}
		}else if("a10".Equals (mPojulObject.type) && (((A10aPlan)mPojulObject).mPatrolArea.areaId -1) != patrolAreaValue.value){
			((A10aPlan)mPojulObject).mPatrolArea = new RadiusArea((patrolAreaValue.value + 1));
			if(mPojulObject.behavior == 1){
				((A10aPlan)mPojulObject).startNav (((A10aPlan)mPojulObject).mPatrolArea.getRandomPoint());
			}
		}

	}

	public void OnAttackArmyValueChanged(){
		if(selectedTra == null || !selectedTra.GetComponent<PojulObject>() || selectedTra.GetComponent<PojulObject>().isDestoryed){
			return;
		}
		PojulObject mPojulObject = selectedTra.GetComponent<PojulObject> ();
		if ("car3".Equals (mPojulObject.type) || "car5".Equals (mPojulObject.type) || "a10".Equals (mPojulObject.type)) {
			mPojulObject.isAttackArmy = attackArmyValue.isOn;
		}
	}

	public void OnMaxMountMissileChanged(){
		if(selectedTra == null || !selectedTra.GetComponent<PojulObject>() || selectedTra.GetComponent<PojulObject>().isDestoryed){
			return;
		}
		PojulObject mPojulObject = selectedTra.GetComponent<PojulObject> ();
		if ("car5".Equals (mPojulObject.type) && (((CarType5)mPojulObject).maxMountMissle) != maxMountMissileValue.value) {
			((CarType5)mPojulObject).maxMountMissle = maxMountMissileValue.value;
		}else if ("a10".Equals (mPojulObject.type) && (((A10aPlan)mPojulObject).maxMountMissle) != maxMountMissileValue.value) {
			((A10aPlan)mPojulObject).maxMountMissle = maxMountMissileValue.value;
		}
	}

	public void OnSupplyPriorityChanged(){
		if (selectedTra == null || !selectedTra.GetComponent<PojulObject> () || selectedTra.GetComponent<PojulObject> ().isDestoryed) {
			return;
		}
		PojulObject mPojulObject = selectedTra.GetComponent<PojulObject> ();
		if ("car5".Equals (mPojulObject.type) && (((CarType5)mPojulObject).priority) != supplyPriorityValue.value) {
			((CarType5)mPojulObject).priority = supplyPriorityValue.value;
		}
	}

	public void OnLeaveClick(){
		if(isSelectMode || planMove.player == null || !planMove.player.GetComponent<PojulObject>() || planMove.player.GetComponent<PojulObject>().isDestoryed){
			return;
		}
		PojulObject mPojulObject = planMove.player.GetComponent<PojulObject> ();
		if ("car3".Equals (mPojulObject.type)) {
			((CarType3)mPojulObject).setPlayType (1);
			isOnLeave = true;
		}else if("a10".Equals (mPojulObject.type)){
			((A10aPlan)mPojulObject).setPlayType (1);
			isOnLeave = true;
		}

	}

	public void OnLeaveDown(){
		if(isSelectMode){
			return;
		}
		leave.sprite = leave2;
	}

	public void OnLeaveUp(){
		if(isSelectMode){
			return;
		}
		leave.sprite = leave1;
	}



}
