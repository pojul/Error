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

}
