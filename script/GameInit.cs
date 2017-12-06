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
	public static Hashtable park0 = new Hashtable(); //我方停车位
	public static Hashtable park1 = new Hashtable(); //敌方停车位
	public static Dictionary<string, List<Transform>> coordinateManager0 = new Dictionary<string, List<Transform>>();
	public static Dictionary<string, List<Transform>> coordinateManager1 = new Dictionary<string, List<Transform>>();

	public static List<Transform>  allNearEnemys_0 = new List<Transform>();
	public static List<Transform>  allNearEnemys_1 = new List<Transform>();
	public static Dictionary<int, List<Transform>>  nearEnemys_0 = new Dictionary<int, List<Transform>>();
	public static Dictionary<int, List<Transform>>  nearEnemys_1 = new Dictionary<int, List<Transform>>();

	public static Hashtable Car5Area0 = new Hashtable();
	public static Hashtable Car5Area1 = new Hashtable();

	public static Vector3 home1Pos = new Vector3(0, 0, -60000);
	public static Vector3 home2Pos = new Vector3(0, 0, 60000);//19003//17750

	public static List<Transform> invades_0 = new List<Transform> ();
	public static List<Transform> invades_1 = new List<Transform> ();

	public static Texture2D backgroundProgress;
	public static Texture2D progress1;
	public static Texture2D progress2;
	
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

		InvokeRepeating("gc", 60.0f, 60.0f);
	}

	void gc(){
		if(ShowFPS.currFPS < 20.0f){
			System.GC.Collect ();
		}

	}

	void addMoney(){
		MyMoney = MyMoney + 1;
	}

	void initData(){
		modelpaths.Add ("a10", "Prefabs/A10a_lod");
		modelpaths.Add ("A10", "Prefabs/A-10a");
		modelpaths.Add ("air_ray_A10", "Prefabs/Particle/air_ray_A101");
		modelpaths.Add ("tankFire", "Prefabs/Particle/tankFire");
		modelpaths.Add ("bomb1", "Prefabs/Particle/bomb1");
		modelpaths.Add ("bomb2", "Prefabs/Particle/bomb2");
		modelpaths.Add ("missile_blaze1", "Prefabs/Particle/missile_blaze1");
		modelpaths.Add ("missile_blaze2", "Prefabs/Particle/missile_blaze2");
		modelpaths.Add ("inflame", "Prefabs/Particle/inflame");
		modelpaths.Add ("cannon1", "Prefabs/arms/cannon_type1");
		modelpaths.Add ("car2", "Prefabs/arms/car_type2");
		modelpaths.Add ("car3", "Prefabs/arms/car_type3");
		modelpaths.Add ("car4", "Prefabs/arms/car_type4");
		modelpaths.Add ("car5", "Prefabs/arms/car_type5");
		modelpaths.Add ("car6", "Prefabs/arms/car_type6");
		modelpaths.Add ("missile1", "Prefabs/arms/missile_type1");
		modelpaths.Add ("missile2", "Prefabs/arms/missile_type2");
		modelpaths.Add ("missile3", "Prefabs/arms/missile_type3");
		modelpaths.Add ("missile4", "Prefabs/arms/missile_type4");
		modelpaths.Add ("shell1", "Prefabs/arms/shell_type1");
		modelpaths.Add ("transport1", "Prefabs/arms/transport_type1");

		backgroundProgress = (Texture2D)Resources.Load ("icon/progress1/progress1a");
		progress1 = (Texture2D)Resources.Load ("icon/progress1/progress1c");
		progress2 = (Texture2D)Resources.Load ("icon/progress1/progress1d");

		prices.Add ("a10", 1);
		prices.Add ("car2", 1);//6
		prices.Add ("car3", 1);//21
		prices.Add ("car4", 1);//23
		prices.Add ("car5", 1);//28
		prices.Add ("car6", 20);
		prices.Add ("missile1", 33);
		prices.Add ("missile2", 29);
		prices.Add ("missile3", 1);
		prices.Add ("shell1", 1);
		prices.Add ("transport1", 1);//62

		maxInstance.Add ("0_a10", 20);
		maxInstance.Add ("1_a10", 20);
		maxInstance.Add ("0_car2", 20);//12
		maxInstance.Add ("1_car2", 12);
		maxInstance.Add ("0_car3", 40);//16
		maxInstance.Add ("1_car3", 40);//16
		maxInstance.Add ("0_car4", 100);//16
		maxInstance.Add ("1_car4", 100);//16
		maxInstance.Add ("0_car5", 3);
		maxInstance.Add ("1_car5", 3);
		maxInstance.Add ("0_car6", 8);
		maxInstance.Add ("1_car6", 8);
		maxInstance.Add ("0_missile1",10);
		maxInstance.Add ("1_missile1",10);
		maxInstance.Add ("0_missile2", 2);
		maxInstance.Add ("1_missile2", 2);
		maxInstance.Add ("0_missile3", 2);
		maxInstance.Add ("1_missile3", 2);
		maxInstance.Add ("0_shell1", 100000);
		maxInstance.Add ("1_shell1", 100000);
		maxInstance.Add ("0_transport1", 6);
		maxInstance.Add ("1_transport1", 6);

		currentInstance.Add ("0_a10", 0);
		currentInstance.Add ("1_a10", 0);
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
		currentInstance.Add ("1_missile3", 0);
		currentInstance.Add ("0_missile3", 0);
		currentInstance.Add ("1_missile2", 0);
		currentInstance.Add ("0_shell1", 0);
		currentInstance.Add ("1_shell1", 0);
		currentInstance.Add ("0_transport1", 0);
		currentInstance.Add ("1_transport1", 0);

		park0.Add (new Vector3 (3000, 0, -40000), 0);//0:空闲；1:被占用
		park0.Add (new Vector3 (5000, 0, -40000), 0);
		park0.Add (new Vector3 (7000, 0, -40000), 0);
		park0.Add (new Vector3 (9000, 0, -40000), 0);
		park0.Add (new Vector3 (11000, 0, -40000), 0);
		park0.Add (new Vector3 (20000, 0, -54500), 0);
		park0.Add (new Vector3 (20000, 0, -56500), 0);
		park0.Add (new Vector3 (20000, 0, -58500), 0);
		park0.Add (new Vector3 (20000, 0, -60500), 0);
		park0.Add (new Vector3 (20000, 0, -62500), 0);
		park0.Add (new Vector3 (-16000, 0, -49000), 0);
		park0.Add (new Vector3 (-16000, 0, -51000), 0);
		park0.Add (new Vector3 (-16000, 0, -53000), 0);
		park0.Add (new Vector3 (-16000, 0, -55000), 0);
		park0.Add (new Vector3 (-16000, 0, -57000), 0);
		park0.Add (new Vector3 (-18000, 0, -78000), 0);
		park0.Add (new Vector3 (-14000, 0, -78000), 0);
		park0.Add (new Vector3 (-18000, 0, -82000), 0);
		park0.Add (new Vector3 (-14000, 0, -82000), 0);

		park1.Add (new Vector3 (-3000, 0, -40000), 0);
		park1.Add (new Vector3 (-5000, 0, -40000), 0);
		park1.Add (new Vector3 (-7000, 0, -40000), 0);
		park1.Add (new Vector3 (-9000, 0, -40000), 0);
		park1.Add (new Vector3 (-11000, 0, -40000), 0);
		park1.Add (new Vector3 (16000, 0, 52000), 0);
		park1.Add (new Vector3 (16000, 0, 54000), 0);
		park1.Add (new Vector3 (16000, 0, 56000), 0);
		park1.Add (new Vector3 (16000, 0, 58000), 0);
		park1.Add (new Vector3 (16000, 0, 60000), 0);
		park1.Add (new Vector3 (-18000, 0, 74500), 0);
		park1.Add (new Vector3 (-18000, 0, 72500), 0);
		park1.Add (new Vector3 (-18000, 0, 70500), 0);
		park1.Add (new Vector3 (-18000, 0, 68500), 0);
		park1.Add (new Vector3 (-18000, 0, 66500), 0);
		park1.Add (new Vector3 (14000, 0, 78500), 0);
		park1.Add (new Vector3 (14000, 0, 82500), 0);
		park1.Add (new Vector3 (18000, 0, 78500), 0);
		park1.Add (new Vector3 (18000, 0, 82500), 0);

	}

	public static void instanceGameobject(string playerId, string type){
		string tag = (playerId + "_" + type);
		if ("missile1".Equals (type) || "missile2".Equals (type)) {
			currentInstance [tag] = (int)currentInstance [tag] + 1;
			return;
		}

		float initZ = -60000;
		if("1".Equals(playerId)){
			initZ = 60000;
		}
		GameObject prefab = (GameObject)Instantiate(Resources.Load((string)modelpaths[type]), 
			new Vector3(0, 200, initZ), Quaternion.Euler(0, 0, 0)) as GameObject;
		prefab.tag = tag;
		gameObjectInstance.Add (prefab);
		currentInstance [prefab.tag] = (int)currentInstance [prefab.tag] + 1;

	}



	void InstancePlayer(){
		player = (GameObject)Instantiate(Resources.Load((string)modelpaths["a10"]), 
			new Vector3(0, 125, -60000), Quaternion.Euler(0, 0, 0)) as GameObject;
		player.tag = "0_player";
		((A10aPlan)player.GetComponent<A10aPlan>()).setPlayType (0);
		player.AddComponent<planMove> ();
	}

	// Update is called once per frame
	void Update () {
		
	}

}
