using System.Collections;
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

	private float armShopItemImgHeight = Screen.height * 0.16f;
	private float armShopItemImgWidth = Screen.height * 0.3f;
	private float armShopItemPriceHeight = Screen.height * 0.16f;
	private float armShopItemPriceWidth = Screen.height * 0.2f;

	private float armShopItemBuyheight = Screen.height* 0.1f;
	private float armShopItemSpace = Screen.height * 0.01f;

	public Image armShopPanel;
	public Image armShopTitle;
	public Image armShopClose;
	public GameObject armShopScrollView;
	public GameObject armShopContent;

	public Image[] armShopImgs = new Image[10];
	public Text[] armShopPrices = new Text[10];
	public Image[] armShopBuys = new Image[10];
	public Sprite[] armShopBgs = new Sprite[10];


	//public ScrollView

	public Sprite armShopClose1;
	public Sprite armShopClose2;

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
			fire.rectTransform.sizeDelta .y * 2.2f,fire.rectTransform.position.z);

		aimNext.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.12f, Screen.height * 0.12f);
		aimNext.rectTransform.position = new Vector3 ((Screen.width - aimNext.rectTransform.sizeDelta .x * 2.2f), 
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
			Screen.height * 0.6f, mRectTransform.position.z);
		accelerate.value = accelerate.maxValue * 0.5f;
		accelerate.handleRect.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.height * 0.06f, Screen.height * 0.05f);;

		buy.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.11f, Screen.height * 0.11f);
		buy.rectTransform.position = new Vector3 (buy.rectTransform.sizeDelta .x * 0.5f, 
			buy.rectTransform.sizeDelta .y * 5f,buy.rectTransform.position.z);

		mselect.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.11f, Screen.height * 0.11f);
		mselect.rectTransform.position = new Vector3 (mselect.rectTransform.sizeDelta .x * 0.5f, 
			mselect.rectTransform.sizeDelta .y * 4f,mselect.rectTransform.position.z);

		for (int i = 0; i < armImgs.Length; i++) {
			itemImgStys [i] = new GUIStyle ();
			itemImgStys [i].normal.background = armImgs [i];
		}

		touchMove.rectTransform.sizeDelta = new Vector2 ( (Screen.width - Screen.height * 0.64f), Screen.height);
		touchMove.rectTransform.position = new Vector3 ((Screen.height * 0.32f + touchMove.rectTransform.sizeDelta .x * 0.5f), 
			touchMove.rectTransform.sizeDelta .y * 0.5f ,touchMove.rectTransform.position.z);

		initArmShop ();

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
			string str = GameInit.prices[armNames[i]] + "/" + GameInit.MyMoney.ToString ();
			armShopPrices [i].text = str;


		}

		//armShopScrollBarRect.sizeDelta.y = 100;


		//armShopScrollViewRect.sizeDelta = new Vector2 (armShopScrollViewRect.sizeDelta.x, 1000);
		//armShopScrollViewRect.sizeDelta = new Vector2 (100, 1000);

	}

	void Update(){

		if(isSelectMode){
			listenerScreenTap ();
		}

		if(planMove.player == null && cameraThubmnailPoint.enabled){
			updatyXZ (Camera.main.transform, cameraThubmnailPoint);
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
				if(selectedTra != null && selectedTra.GetComponent<PojulObject>()){
					selectedTra.GetComponent<PojulObject> ().isSelected = false;
				}
				selectedTra = root;
				mPojulObject.isSelected = true;
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
					if (tempDistance < minDistance) {
						selected = tempTransform;
						minDistance = tempDistance;
					}
				}
			}
			if(selected != null){
				if(selectedTra != null && selectedTra.GetComponent<PojulObject>()){
					selectedTra.GetComponent<PojulObject> ().isSelected = false;
				}
				if(selected.GetComponent<PojulObject>()){
					selectedTra = selected;
					selected.GetComponent<PojulObject> ().isSelected = true;
					//Debug.Log (selected.name + "gqb------>onScreenHit2");
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

			//enemys
			//foreach(Transform mTransform in GameInit.allNearEnemys_1.Keys){

			//}
			int[] keys = new int[GameInit.allNearEnemys_0.Keys.Count];
			GameInit.allNearEnemys_0.Keys.CopyTo (keys, 0);
			for (int i = 0; i < keys.Length; i++) {
				//Debug.Log (GameInit.allNearEnemys_0.Count + "; gqb------>allNearEnemys_0: " + enemythubmnailPoints.Count);
				//Debug.Log (GameInit.allNearEnemys_0.Count + "; gqb------>allNearEnemys_0: " + keys[i] + 
				//"; dt: " + (Time.time - GameInit.allNearEnemys_0[keys[i]]));
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
		Camera.main.transform.position = new Vector3 ((Camera.main.transform.position.x - dx*1.5f),
			cameraY, (Camera.main.transform.position.z - dz*1.5f));
		touchMoveDown = ((PointerEventData)rawdata).position;
	}

	public void OnTouchMoveUp(BaseEventData rawdata){
		if(planMove.player != null){
			return;
		}
	}


	void updateMissileAim(){
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

	void OnGUI(){
		draw ();
	}

	void draw(){
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
			if((int)GameInit.currentInstance[("0_"+armNames[i])] >= (int)GameInit.maxInstance[("0_"+armNames[i])]){
				buyStr = buyStatus [2];
			}

			if(GUI.Button(new Rect(itemImgX, itemSpace*i, itemImgWidth, itemImgHeight), "", itemImgStys[i])){
			}
			if(GUI.Button(new Rect(itemPricex, itemSpace*i, itemPricewidth, itemPriceheight), str)){
			}
			if(GUI.Button(new Rect(itemBuyX, (itemSpace*i + Screen.height * 0.3f / 10), itemBuyWidth, itemBuyheight), buyStr)){
				if(buyStatus [0].Equals(buyStr)){
					GameInit.instanceGameobject ("0", armNames[i]);
					GameInit.MyMoney = GameInit.MyMoney - (int)GameInit.prices [armNames [i]];
				}
			}
		}
		GUI.EndScrollView();

	}

	public void OnArmShopCloseClick(){
		
	}

	public void OnArmShopCloseDown(){
		if(armShopClose.enabled){
			armShopClose.sprite = armShopClose2;
		}
	}

	public void OnArmShopCloseUp(){
		armShopClose.sprite = armShopClose1;
	}

	void onArmShopShow(){
		
	}

}
