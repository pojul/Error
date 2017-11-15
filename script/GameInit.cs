using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInit  : MonoBehaviour {

	public static float mach = 1000;

	public static string goldgatePrefabPath = "Prefabs/goldgate_lod";
	public static string homePrefabPath = "Prefabs/Home_type1";
	//public static string homePrefabPath = "Prefabs/Home_lod";
	public static Hashtable modelpaths = new Hashtable();
	public static Hashtable modelParams = new Hashtable();

	public static Hashtable prices = new Hashtable();
	public static List<GameObject> gameObjectInstance = new List <GameObject>();
	public static Hashtable maxInstance = new Hashtable();
	public static Hashtable currentInstance = new Hashtable();

	public static Vector3 home1Pos = new Vector3(0, 0, -60000);
	public static Vector3 home2Pos = new Vector3(0, 0, 60000);//19003//17750
	
	private GameObject player;

	public static float MyMoney = 0;
	
	public UnityEngine.AI.NavMeshAgent nav;

	public RadiusArea mRadiusArea1= new RadiusArea(5);

	public GameObject test;

	public static Vector3[] myNavPoint1as = new Vector3[]{new Vector3(3340, 100, -34925), 
		new Vector3 (32331, 100, -61230), 
		new Vector3 (-3853, 100, -92787),//15549), 
		new Vector3 (-42224, 100, -61568)//3109)
		//new Vector3 (11630, 125, 235)
		} ;

	private int airShipNum = 10;
	private float airShipInterval = 20;
	private float eachAirShipInterval = 1.0f;
	private float airShapArriveTime =0.0f;
	private float lastAirBuildTime = 0;
	private bool airShapArrive = false;
	private int hasBuildAirNum = 0;

	/*[RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
		
        Debug.Log("After scene is loaded and game is running");
    }

    [RuntimeInitializeOnLoadMethod]
    static void OnSecondRuntimeMethodLoad()
    {
        Debug.Log("SecondMethod After scene is loaded and game is running.");
    }*/


	void Start () {

		initData ();
		InstancePlayer();

		InvokeRepeating("addMoney", 2.0f, 2.0f);
	}

	void addMoney(){
		MyMoney = MyMoney + 1;
	}

	void initData(){
		modelpaths.Add ("A10_lod", "Prefabs/A10a_lod");
		modelpaths.Add ("A10", "Prefabs/A-10a");
		modelpaths.Add ("air_ray_A10", "Prefabs/Particle/air_ray_A101");
		modelpaths.Add ("cannon1", "Prefabs/arms/cannon_type1");
		modelpaths.Add ("car2", "Prefabs/arms/car_type2");
		modelpaths.Add ("car3", "Prefabs/arms/car_type3");
		modelpaths.Add ("car4", "Prefabs/arms/car_type4");
		modelpaths.Add ("car5", "Prefabs/arms/car_type5");
		modelpaths.Add ("car6", "Prefabs/arms/car_type6");
		modelpaths.Add ("missile1", "Prefabs/arms/missile_type1");
		modelpaths.Add ("missile2", "Prefabs/arms/missile_type2");
		modelpaths.Add ("shell1", "Prefabs/arms/shell_type1");
		modelpaths.Add ("transport1", "Prefabs/arms/transport_type1");

		prices.Add ("cannon1", 8);
		prices.Add ("car2", 6);
		prices.Add ("car3", 21);
		prices.Add ("car4", 25);
		prices.Add ("car5", 28);
		prices.Add ("car6", 20);
		prices.Add ("missile1", 33);
		prices.Add ("missile2", 29);
		prices.Add ("shell1", 1);
		prices.Add ("transport1", 62);

		maxInstance.Add ("cannon1", 20);
		maxInstance.Add ("car2", 10);
		maxInstance.Add ("car3", 16);
		maxInstance.Add ("car4", 16);
		maxInstance.Add ("car5", 6);
		maxInstance.Add ("car6", 8);
		maxInstance.Add ("missile1", 20);
		maxInstance.Add ("missile2", 10);
		maxInstance.Add ("shell1", 100000);
		maxInstance.Add ("transport1", 6);

		currentInstance.Add ("0_cannon1", 0);
		currentInstance.Add ("1_cannon1", 0);
		currentInstance.Add ("0_car2", 0);
		currentInstance.Add ("1_car2", 0);
		currentInstance.Add ("0_car3", 0);
		currentInstance.Add ("1_car3", 0);
		currentInstance.Add ("0_car4", 0);
		currentInstance.Add ("1_car4", 0);
		currentInstance.Add ("0_car5", 0);
		currentInstance.Add ("1_car5", 0);
		currentInstance.Add ("0_car6", 0);
		currentInstance.Add ("1_car6", 0);
		currentInstance.Add ("0_missile1", 0);
		currentInstance.Add ("1_missile1", 0);
		currentInstance.Add ("0_missile2", 0);
		currentInstance.Add ("1_missile2", 0);
		currentInstance.Add ("0_shell1", 0);
		currentInstance.Add ("1_shell1", 0);
		currentInstance.Add ("0_transport1", 0);
		currentInstance.Add ("1_transport1", 0);
	}

	public static void instanceGameobject(int playerId, string type){
		float initZ = -60000;
		if(playerId  == 1){
			initZ = 60000;
		}
		GameObject prefab = (GameObject)Instantiate(Resources.Load((string)modelpaths[type]), 
			new Vector3(0, 200, initZ), Quaternion.Euler(0, 0, 0)) as GameObject;
		prefab.tag = (playerId + "_" + type);
		gameObjectInstance.Add (prefab);
	}



	void InstancePlayer(){
		player = (GameObject)Instantiate(Resources.Load((string)modelpaths["A10"]), 
			new Vector3(0, 125, -60000), Quaternion.Euler(0, 0, 0)) as GameObject;
		player.tag = "player";
		((A10aPlan)player.GetComponent<A10aPlan>()).setPlayType (0);
		player.AddComponent<planMove> ();
	}
	

	void buildAir (Vector3 navPoint){
		GameObject prefabA10 = (GameObject)Instantiate(Resources.Load((string)modelpaths["A10_lod"]), 
			new Vector3(0, 200, -60000), Quaternion.Euler(0, 0, 0)) as GameObject;
		prefabA10.tag = "A10";
		((A10aPlan)prefabA10.GetComponent<A10aPlan>()).setPlayType (1);
		prefabA10.AddComponent<AirAction>();
		AirAction ai = (AirAction)prefabA10.GetComponent<AirAction> ();
		modelParams.Add (ai, prefabA10);
		ai.startNav (navPoint);
	}

	// Update is called once per frame
	void Update () {
		minoAirShip ();
	}

	void minoAirShip (){
		int shipTimes = (int)(Time.time/airShipInterval);
		if (!airShapArrive && shipTimes % 2 == 0) {
			airShapArrive = true;
			hasBuildAirNum = 0;
			airShapArriveTime = Time.time;
		} else if(shipTimes % 2 == 1){
			airShapArrive = false;
			return;
		}
		if(airShapArrive){
			float dTime = Time.time - airShapArriveTime;
			int airBuildNum = (int)(dTime / eachAirShipInterval);
			if(hasBuildAirNum < airShipNum && hasBuildAirNum < airBuildNum){
				buildAir (myNavPoint1as[hasBuildAirNum%4]);
				hasBuildAirNum = hasBuildAirNum + 1;
			}
		}
	}

}
