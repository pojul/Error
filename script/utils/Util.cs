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
		return areaid = areaid;
	}

	public static string updateCoordinate(Transform mTransform, string currentCoordinate){
		if(GameInit.coordinateManager == null){
			return currentCoordinate;
		}
		int coordinateX = (int)mTransform.position.x / 4000;
		int coordinateZ = (int)mTransform.position.z / 4000;
		string tempCoordinate = coordinateX.ToString() + coordinateZ.ToString();
		//Debug.Log (mTransform.position.x + "gqb----->updateCoordinate: " + coordinateX);
		if(currentCoordinate.Equals(tempCoordinate)){
			return currentCoordinate;
		}

		if(GameInit.coordinateManager.ContainsKey(currentCoordinate)){
			GameInit.coordinateManager [currentCoordinate].Remove (mTransform);
		}

		if(!GameInit.coordinateManager.ContainsKey(tempCoordinate)){
			List<Transform> objs = new List<Transform> ();
			objs.Add (mTransform);
			GameInit.coordinateManager.Add (tempCoordinate, objs);
			currentCoordinate = tempCoordinate;
		}else {
			GameInit.coordinateManager [tempCoordinate].Add (mTransform);
			currentCoordinate = tempCoordinate;
		}
		return currentCoordinate;
	}

}
