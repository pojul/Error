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
		if ("0".Equals (playId)) {
			bool matchArea = false;
			while (!matchArea) {
				int areaId = Random.Range (1, 4);
				if(!GameInit.Car5Area0.ContainsKey(areaId)){
					matchArea = true;
					areaid = areaId;
				}

			}
		}else if("1".Equals(playId)){
			bool matchArea = false;
			while (!matchArea) {
				int areaId = Random.Range(5, 8);
				if(!GameInit.Car5Area1.ContainsKey(areaId)){
					matchArea = true;
					areaid = areaId;
				}
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
		Dictionary<int, List<Transform>> nearEnemys = null;
		List<Transform> allNearEnemys = null;
		if("0".Equals(playerId)){
			nearEnemys = GameInit.nearEnemys_0;
			allNearEnemys = GameInit.allNearEnemys_0;
		}else if("1".Equals(playerId)){
			nearEnemys = GameInit.nearEnemys_1;
			allNearEnemys = GameInit.allNearEnemys_1;
		}
		if(allNearEnemys.Contains(mTransform)){
			return;
		}
		allNearEnemys.Add (mTransform);
		int radioAreaId = getRadioAreaId (mTransform);
		if (nearEnemys.ContainsKey (radioAreaId)) {
			nearEnemys [radioAreaId].Add (mTransform);

		} else {
			List<Transform> transforms = new List<Transform>();
			transforms.Add (mTransform);
			nearEnemys.Add (radioAreaId, transforms);
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

}
