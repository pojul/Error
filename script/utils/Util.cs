using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour {

	public static Vector3 getIdlePart(string playId){
		Hashtable tempPark = null;
		if("0".Equals(playId)){
			tempPark = GameInit.park0;
		}else if("1".Equals(playId)){
			tempPark = GameInit.park1;
		}
		//Debug.Log ("gqb------>playId: " + playId );
		foreach(Vector3 key in tempPark.Keys)
		{
			if((int)tempPark[key] == 0){
				return key;
			}
		}
		return new Vector3(0,0,0);
	}

	public static int getCar5AreaId(string playId){
		int areaid = -1;
		int times = 0;
		if ("0".Equals (playId)) {
			bool matchArea = false;
			while (!matchArea) {
				if(times > 50){
					areaid = Random.Range (1, 4);
					break;
				}
				int areaId = Random.Range (1, 4);
				if(!GameInit.Car5Area0.ContainsKey(areaId)){
					matchArea = true;
					areaid = areaId;
				}
				times = times + 1;
			}
		}else if("1".Equals(playId)){
			bool matchArea = false;
			while (!matchArea) {
				if(times > 50){
					areaid = Random.Range(5, 8);
					break;
				}
				int areaId = Random.Range(5, 8);
				if(!GameInit.Car5Area1.ContainsKey(areaId)){
					matchArea = true;
					areaid = areaId;
				}
				times = times + 1;
			}
		}
		return areaid;
	}

	public static object[] updateCoordinate(Transform mTransform, string currentCoordinate, string playerId, 
		bool fndEnemys, int findArea){
		object[] infors = new object[2];
		Dictionary<string, List<Transform>> coordinateManager = null;
		Dictionary<string, List<Transform>> enemyCoordinateManager = null;
		if("0".Equals(playerId)){
			coordinateManager = GameInit.coordinateManager0;
			enemyCoordinateManager = GameInit.coordinateManager1;
		}else if("1".Equals(playerId)){
			coordinateManager = GameInit.coordinateManager1;
			enemyCoordinateManager = GameInit.coordinateManager0;
		}

		if(coordinateManager == null){
			return infors;
		}
		int coordinateX = (int)mTransform.position.x / 10000;
		int coordinateZ = (int)mTransform.position.z / 10000;
		string tempCoordinate = coordinateX.ToString() + coordinateZ.ToString();
		//Debug.Log (mTransform.position.x + "gqb----->updateCoordinate: " + coordinateX);
		if(currentCoordinate.Equals(tempCoordinate)){
			return infors;
		}

		if(coordinateManager.ContainsKey(currentCoordinate)){
			coordinateManager [currentCoordinate].Remove (mTransform);
		}

		if(!coordinateManager.ContainsKey(tempCoordinate)){
			List<Transform> objs = new List<Transform> ();
			objs.Add (mTransform);
			coordinateManager.Add (tempCoordinate, objs);
			currentCoordinate = tempCoordinate;
		}else {
			coordinateManager [tempCoordinate].Add (mTransform);
			currentCoordinate = tempCoordinate;
		}
		infors [0] = (object)currentCoordinate;
		if(!fndEnemys || enemyCoordinateManager == null){
			return infors;
		}
		List<Transform> nearEnemys = new List<Transform> ();
		for(int i = (coordinateX-findArea); i <= (coordinateX+findArea); i++){
			for(int j = (coordinateZ-findArea); j <= (coordinateZ+findArea); j++){
				string key = i.ToString() + j.ToString();
				Debug.Log (key + "gqb------>util: " + enemyCoordinateManager.ContainsKey(key));
				if(enemyCoordinateManager.ContainsKey(key) && enemyCoordinateManager[key] != null){
					if("0".Equals(playerId)){
						Debug.Log (key + "gqb------>util: " + enemyCoordinateManager[key].Count);
					}
					nearEnemys.AddRange (enemyCoordinateManager[key]);
				}
			}
		}	
		infors [1] = (object)nearEnemys;

		return infors;
	}

	public static void AddNearEnemys(Transform mTransform, string playerId){
		lock(GameInit.locker){
			if(mTransform == null){
				return;
			}
			Dictionary<int, Dictionary<Transform, float>> nearEnemys = null;
			Dictionary<int, object[]> allNearEnemys = null;
			if("0".Equals(playerId)){
				nearEnemys = GameInit.nearEnemys_0;
				allNearEnemys = GameInit.allNearEnemys_0;
			}else if("1".Equals(playerId)){
				nearEnemys = GameInit.nearEnemys_1;
				allNearEnemys = GameInit.allNearEnemys_1;
			}
			if(allNearEnemys.ContainsKey(mTransform.GetHashCode())){
				allNearEnemys [mTransform.GetHashCode()][1] = Time.time;
				int[] keys = new int[nearEnemys.Keys.Count];
				nearEnemys.Keys.CopyTo (keys, 0);
				for (int i = 0; i < keys.Length; i++) {
					if(nearEnemys[keys[i]].ContainsKey(mTransform)){
						nearEnemys[keys[i]][mTransform] = Time.time;
						break;
					}
				}
				return;
			}
			object[] objs = new object[2];
			objs [0] = mTransform;
			objs [1] = Time.time;
			allNearEnemys.Add (mTransform.GetHashCode(), objs);
			int radioAreaId = getRadioAreaId (mTransform);
			if (nearEnemys.ContainsKey (radioAreaId)) {
				nearEnemys [radioAreaId].Add (mTransform, Time.time);
				//nearEnemys [radioAreaId].Add (mTransform);
			} else {
				Dictionary<Transform, float> transforms = new  Dictionary<Transform, float>();
				transforms.Add (mTransform, Time.time);
				nearEnemys.Add (radioAreaId, transforms);
			}
		}
	}

	public static int getRadioAreaId(Transform mTransform){
		if(mTransform.position.x > -55000 && mTransform.position.x < 0){
			if(mTransform.position.z < -60000 && mTransform.position.z > -115000){
				return 4;
			}else if(mTransform.position.z > -60000 && mTransform.position.z < -5000){
				return 1;
			}else if(mTransform.position.z > 5000 && mTransform.position.z < 60000){
				return 6;
			}else if(mTransform.position.z > 60000 && mTransform.position.z < 115000){
				return 8;
			}
		}else if(mTransform.position.x > 0 && mTransform.position.x < 55000){
			if(mTransform.position.z < -60000 && mTransform.position.z > -115000){
				return 3;
			}else if(mTransform.position.z > -60000 && mTransform.position.z < -5000){
				return 2;
			}else if(mTransform.position.z > 5000 && mTransform.position.z < 60000){
				return 5;
			}else if(mTransform.position.z > 60000 && mTransform.position.z < 115000){
				return 7;
			}
		}
		return 0;
	}

	public static bool isOnEnemyNavArea1(Vector3 point, string playerId){
		if ("0".Equals (playerId)) {
			return isOnNavArea2(point);
		} else {
			return isOnNavArea1(point);
		}
	}

	public static bool isOnMyNavArea1(Vector3 point, string playerId){
		if ("0".Equals (playerId)) {
			return isOnNavArea1(point);
		} else {
			return isOnNavArea2(point);
		}
	}

	public static bool isOnNavArea1(Vector3 point){
		if((new Vector3(point.x, 0, point.z) - GameInit.home1Pos).magnitude > 52000){
			return false;
		}else{
			return true;
		}
	}

	public static bool isOnNavArea2(Vector3 point){
		if((new Vector3(point.x, 0, point.z) - GameInit.home2Pos).magnitude > 52000 ){
			return false;
		}else{
			return true;
		}
	}


	public static float getDirectDRol(float fromRol, float toRol, float limit, float moreScale){
		float dRol = 0;
		float dRol1 = 0;
		float dRol2 = 0;
		int direct = 1;
		if(fromRol > toRol){
			dRol1 = fromRol - toRol;
			dRol2 = 360 - fromRol + toRol;
			if(dRol1 < dRol2){
				direct = -1;
				dRol = dRol1;
			}else{
				direct = 1;
				dRol = dRol2;
			}
		}else if(fromRol < toRol){
			dRol1 = toRol - fromRol;
			dRol2 = 360 - toRol + fromRol;
			if (dRol1 < dRol2) {
				direct = 1;
				dRol = dRol1;
			} else {
				direct = -1;
				dRol = dRol2;
			}
		}
		float minDRol = limit * Time.deltaTime;
		if(dRol > minDRol){
			dRol = minDRol + (dRol - minDRol) * moreScale;
		}
		return dRol*direct;
	}

}
