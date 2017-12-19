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

	public Image thubmnailPoint;

	public Sprite redThubmnailPoint;
	public Sprite blueThubmnailPoint;
	public Sprite yellowThubmnailPoint;

	public Sprite fireMissileBg1;
	public Sprite fireMissileBg2;
	public Sprite fireMissileBg3;
	public Sprite aimNextBg1;
	public Sprite aimNextBg2;
	public Sprite fireBg1;
	public Sprite fireBg2;
	public Sprite fireBg3;


	private Dictionary<int, object[]> thubmnailPoints = new Dictionary<int, object[]> ();//GameObject, Image
	private Dictionary<int, object[]> enemythubmnailPoints = new Dictionary<int, object[]> ();

	public Transform missileAimedTra;

	public Image fireAimPre;
	public static Image fireAim;

	private string playerType = "";
	private bool canFire = true;

	// Use this for initialization
	void Start () {

		fireAim = fireAimPre;

		thubmnail.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.32f, Screen.height * 0.32f);
		thubmnail.rectTransform.position = new Vector3 (thubmnail.rectTransform.sizeDelta .x * 0.5f, 
			(Screen.height - thubmnail.rectTransform.sizeDelta .y * 0.5f),thubmnail.rectTransform.position.z);

		fire.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.12f, Screen.height * 0.12f);
		fire.rectTransform.position = new Vector3 ((Screen.width - fire.rectTransform.sizeDelta .x * 0.56f), 
			fire.rectTransform.sizeDelta .y * 2.4f,fire.rectTransform.position.z);

		aimNext.rectTransform.sizeDelta = new Vector2 (Screen.height * 0.12f, Screen.height * 0.12f);
		aimNext.rectTransform.position = new Vector3 ((Screen.width - aimNext.rectTransform.sizeDelta .x * 2.4f), 
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

		InvokeRepeating("updateThubmnail", 0.5f, 0.5f);

		InvokeRepeating("updateMissileAim", 0.2f, 0.2f);
		InvokeRepeating("clearNullThubmnail", 2f, 2f);
	}

	void Update(){

		if(missileAimedTra != null){
			missileAim.enabled = true;
			Vector2 screenPos = Camera.main.WorldToScreenPoint (missileAimedTra.position);
			missileAim.rectTransform.position = new Vector3 (screenPos.x, 
				screenPos.y, missileAim.rectTransform.position.z);
		} else {
			missileAim.enabled = false;
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
					tempImage.sprite = blueThubmnailPoint;
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
		PointerEventData data1 = data as PointerEventData;
		if(accelerate != null && accelerate.enabled){
			accelerate.value = accelerate.maxValue * 0.5f;
			planMove.accelerate = 0;
		}
		//Debug.Log (data1.selectedObject + "gqb------>AcceleratePointerUp: " + data1.position);
	}

	public void AcceleratePointerMove(BaseEventData data){
		PointerEventData data1 = data as PointerEventData;
		if(accelerate != null && accelerate.enabled){
			planMove.accelerate = planMove.maxAccelerate * (accelerate.value - accelerate.maxValue * 0.5f)  /(accelerate.maxValue * 0.5f);
		}
		//Debug.Log (data1.selectedObject + "gqb------>AcceleratePointerUp: " + data1.position);
	}

	public void OnFireMissileClick(){
		//Debug.Log ("gqb------>OnFireMissileClick");
		if(planMove.player == null || missileAimedTra == null || planMove.currentMountMissle <= 0){
			return;
		}
		PojulObject mPojulObject = planMove.player.gameObject.GetComponent<PojulObject> ();
		if(mPojulObject == null){
			return;
		}
		mPojulObject.fireMissileOfPlayer (missileAimedTra);
	}

	public void OnFireMissileDown(){
		if(missileAimedTra == null || planMove.currentMountMissle <= 0){
			return;
		}
		fireMissile.sprite = fireMissileBg2;
	}

	public void OnFireMissileUp(){
		if(missileAimedTra == null || planMove.currentMountMissle <= 0){
			return;
		}
		fireMissile.sprite = fireMissileBg1;
	}

	public void OnAimNextClick(){
		//Debug.Log ("gqb------>OnAimNextClick");
		if(planMove.player == null || missileAimedTra == null){
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
		aimNext.sprite = aimNextBg2;
	}

	public void OnAimNextUp(){
		aimNext.sprite = aimNextBg1;
	}

	public void OnFireClick(){
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
		if(!canFire){
			return;
		}
		fire.sprite = fireBg2;
	}

	public void OnFireUp(){
		if(!canFire){
			return;
		}
		fire.sprite = fireBg1;
	}

	void car3CanFire(){
		fire.sprite = fireBg1;
		canFire = true;
	}

	void updateMissileAim(){

		if(planMove.player == null){
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

}
