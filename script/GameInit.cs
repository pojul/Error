using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInit  : MonoBehaviour {

	public static string goldgatePrefabPath = "Prefabs/goldgate_lod";
	public static string homePrefabPath = "Prefabs/Home_type1";
	//public static string homePrefabPath = "Prefabs/Home_lod";
	public static Hashtable modelpaths = new Hashtable();
	public static Hashtable modelParams = new Hashtable();
	
	public static Vector3 home1Pos = new Vector3(0, 0, 0);
	public static Vector3 home2Pos = new Vector3(0, 0, 117750);//19003//17750
	
	private GameObject player;
	
	public UnityEngine.AI.NavMeshAgent nav;


	public static Vector3[] myNavPoint1as = new Vector3[]{new Vector3(-21049, 100, -62745), 
		new Vector3 (-1818, 100, -80192), 
		new Vector3 (2951, 100, -44451),//15549), 
		new Vector3 (19707, 100, -56891)//3109)
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
		initEnvironment();
		InstancePlayer();




		//tempStrategy ();
	}

	void initData(){
		modelpaths.Add ("A10_lod", "Prefabs/A10a_lod");
		modelpaths.Add ("A10", "Prefabs/A-10a");
		modelpaths.Add ("air_ray_A10", "Prefabs/Particle/air_ray_A101");
	}

	void initEnvironment(){
		/*GameObject goldgate = (GameObject)Instantiate(Resources.Load(goldgatePrefabPath), 
			new Vector3(0, 50, 59003), Quaternion.Euler(0, 90, 0)) as GameObject;
			
		GameObject home1 = (GameObject)Instantiate(Resources.Load(homePrefabPath), 
			home1Pos, Quaternion.Euler(0, 0, 0)) as GameObject;
			
		GameObject home2 = (GameObject)Instantiate(Resources.Load(homePrefabPath), 
			home2Pos, Quaternion.Euler(0, 0, 0)) as GameObject;*/
			
		/*GameObject goldgate2 = (GameObject)Instantiate(Resources.Load(goldgatePrefabPath), 
			new Vector3(-5000, 50, 19003), Quaternion.Euler(0, 90, 0)) as GameObject;*/
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
