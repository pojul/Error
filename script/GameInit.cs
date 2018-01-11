using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInit  : MonoBehaviour {

	public static Object locker = new Object();

	public static float mach = 1000;

	public static string goldgatePrefabPath = "Prefabs/goldgate_lod";
	public static string homePrefabPath = "Prefabs/Home_type1";
	//public static string homePrefabPath = "Prefabs/Home_lod";
	public static Hashtable modelpaths = new Hashtable();
	public static Hashtable modelParams = new Hashtable();

	public static Dictionary<string, float> prices = new Dictionary<string, float>();
	public static List<GameObject> gameObjectInstance = new List <GameObject>();
	public static List<GameObject> myThumbnailObjs = new List <GameObject>();
	public static Dictionary<string, int> maxInstance = new  Dictionary<string, int>();
	public static Dictionary<string, int> currentInstance = new Dictionary<string, int>();
	public static Hashtable park0 = new Hashtable(); //我方停车位
	public static Hashtable park1 = new Hashtable(); //敌方停车位
	public static Dictionary<string, List<Transform>> coordinateManager0 = new Dictionary<string, List<Transform>>();
	public static Dictionary<string, List<Transform>> coordinateManager1 = new Dictionary<string, List<Transform>>();

	public static Dictionary<int, object[]>  allNearEnemys_0 = new Dictionary<int, object[]>();//Transform, float
	public static Dictionary<int, object[]>  allNearEnemys_1 = new Dictionary<int, object[]>();
	public static Dictionary<int, Dictionary<Transform, float>>  nearEnemys_0 = new Dictionary<int, Dictionary<Transform, float>>();
	public static Dictionary<int, Dictionary<Transform, float>>  nearEnemys_1 = new Dictionary<int, Dictionary<Transform, float>>();

	public static Dictionary<int, GameObject> Car5Area0 = new Dictionary<int, GameObject>();
	public static Dictionary<int, GameObject> Car5Area1 = new Dictionary<int, GameObject>();

	public static Dictionary<string, int> remainMissile = new Dictionary<string, int>();

	public static List<Transform> MyCar2 = new List<Transform> ();
	public static List<Transform> EnemyCar2 = new List<Transform> ();

	public static List<Transform> attackArms_0 = new List<Transform>();
	public static List<Transform> attackArms_1 = new List<Transform>();
	public static List<Vector3> attackMasses_0 = new List<Vector3> ();
	public static List<Vector3> attackMasses_1 = new List<Vector3> ();
	public static List<Vector3> attackAreas_0 = new List<Vector3> ();
	public static List<Vector3> attackAreas_1 = new List<Vector3> ();

	public static Vector3 home1Pos = new Vector3(0, 0, -60000);
	public static Vector3 home2Pos = new Vector3(0, 0, 60000);//19003//17750

	public static List<Transform> invades_0 = new List<Transform> ();
	public static List<Transform> invades_1 = new List<Transform> ();

	public static Texture2D backgroundProgress;
	public static Texture2D progress1;
	public static Texture2D progress2;
	
	public static GameObject player;

	public static float MyMoney = 0;
	public static float EnemyMoney = 0;
	
	public UnityEngine.AI.NavMeshAgent nav;

	public RadiusArea mRadiusArea1= new RadiusArea(5);

	public GameObject test;
	public float enemyGoldScale = 0.5f;
	public int a10AttackNum = 0;
	public int car3AttackNum = 0;
	public int transport1AttackNum = 0;
	public int attackMountMissile = 0;
	public Transform attackTra;
	public int enemyAttackBehavor;

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
		//Invoke("InstancePlayer", 2.0f);

		InvokeRepeating("addMoney", 2.0f, 2.0f);

		InvokeRepeating("gc", 60.0f, 60.0f);

		InvokeRepeating("enemyManager", 5f, 5f);

		InvokeRepeating("instanceEnemy", 2f, 2f);
	}

	void gc(){
		if(ShowFPS.currFPS < 20.0f){
			System.GC.Collect ();
		}

	}

	void addMoney(){
		MyMoney = MyMoney + 1;
		EnemyMoney = EnemyMoney + 1;
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
		modelpaths.Add ("bullet", "Prefabs/arms/bullet");
		modelpaths.Add ("transport1", "Prefabs/arms/transport_type1");

		backgroundProgress = (Texture2D)Resources.Load ("icon/progress1/progress1a");
		progress1 = (Texture2D)Resources.Load ("icon/progress1/progress1c");
		progress2 = (Texture2D)Resources.Load ("icon/progress1/progress1d");

		prices.Add ("a10", 8);
		prices.Add ("car2", 4);//6
		prices.Add ("car3", 6);//21
		prices.Add ("car4", 1);//23
		prices.Add ("car5", 6);//28
		prices.Add ("car6", 20);
		prices.Add ("missile1", 1);//33
		prices.Add ("missile2", 1);//29
		prices.Add ("missile3", 1);
		prices.Add ("shell1", 1);
		prices.Add ("transport1", 4);//62

		maxInstance.Add ("0_a10", 2);
		maxInstance.Add ("1_a10", 2);
		maxInstance.Add ("0_car2", 1);//12
		maxInstance.Add ("1_car2", 1);
		maxInstance.Add ("0_car3", 4);//16
		maxInstance.Add ("1_car3", 4);//16
		maxInstance.Add ("0_car4", 100);//16
		maxInstance.Add ("1_car4", 100);//16
		maxInstance.Add ("0_car5", 1);
		maxInstance.Add ("1_car5", 1);
		maxInstance.Add ("0_car6", 8);
		maxInstance.Add ("1_car6", 8);
		maxInstance.Add ("0_missile1",3);
		maxInstance.Add ("1_missile1",3);
		maxInstance.Add ("0_missile2", 2);
		maxInstance.Add ("1_missile2", 2);
		maxInstance.Add ("0_missile3", 6);
		maxInstance.Add ("1_missile3", 6);
		maxInstance.Add ("0_shell1", 100000);
		maxInstance.Add ("1_shell1", 100000);
		maxInstance.Add ("0_transport1", 1);
		maxInstance.Add ("1_transport1", 1);

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
		currentInstance.Add ("1_missile2", 0);
		currentInstance.Add ("0_missile3", 0);
		currentInstance.Add ("1_missile3", 0);
		currentInstance.Add ("0_shell1", 0);
		currentInstance.Add ("1_shell1", 0);
		currentInstance.Add ("0_transport1", 0);
		currentInstance.Add ("1_transport1", 0);

		remainMissile.Add ("0_missile1", 0);
		remainMissile.Add ("1_missile1", 0);
		remainMissile.Add ("0_missile2", 0);
		remainMissile.Add ("1_missile2", 0);
		remainMissile.Add ("0_missile3", 0);
		remainMissile.Add ("1_missile3", 0);

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

		park1.Add (new Vector3 (-16000, 0, 70000), 0);
		park1.Add (new Vector3 (-16000, 0, 67000), 0);
		park1.Add (new Vector3 (-16000, 0, 64000), 0);
		park1.Add (new Vector3 (-16000, 0, 61000), 0);

		/*park1.Add (new Vector3 (-3000, 0, 40000), 0);
		park1.Add (new Vector3 (-5000, 0, 40000), 0);
		park1.Add (new Vector3 (-7000, 0, 40000), 0);
		park1.Add (new Vector3 (-9000, 0, 40000), 0);
		park1.Add (new Vector3 (-11000, 0, 40000), 0);
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
		park1.Add (new Vector3 (18000, 0, 82500), 0);*/

		attackMasses_0.Add (new Vector3(-32000, 0, -60000));
		//attackMasses_0.Add (new Vector3(18000, 0, 20000));
		attackMasses_0.Add (new Vector3(32000, 0, -60000));

		attackMasses_1.Add (new Vector3(-32000, 0, 60000));
		attackMasses_1.Add (new Vector3(32000, 0, 60000));

		attackAreas_0.Add (new Vector3(18000, 0, 20000));
		attackAreas_0.Add (new Vector3(-41000, 0, 40000));
		attackAreas_0.Add (new Vector3(44000, 0, 82000));
		attackAreas_0.Add (new Vector3(-24000, 0, 102000));

		attackAreas_1.Add (new Vector3(-15000, 0, -18000));
		attackAreas_1.Add (new Vector3(43000, 0, -40000));
		attackAreas_1.Add (new Vector3(18000, 0, -106000));
		attackAreas_1.Add (new Vector3(-42000, 0, -80000));

	}

	public static void instanceGameobject(string playerId, string type){
		string tag = (playerId + "_" + type);
		if ("missile1".Equals (type) || "missile2".Equals (type) || "missile3".Equals (type)) {
			currentInstance [tag] = (int)currentInstance [tag] + 1;
			remainMissile[tag] = remainMissile[tag] + 1;
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
		if("0".Equals(playerId)){
			myThumbnailObjs.Add (prefab);
			if(!"car2".Equals (type) && !"car5".Equals (type)){
				attackArms_0.Add (prefab.transform);
			}
		}
		if("1".Equals(playerId)){
			if(!"car2".Equals (type) && !"car5".Equals (type)){
				attackArms_1.Add (prefab.transform);
			}
		}
	}



	void InstancePlayer(){
		/*player = (GameObject)Instantiate(Resources.Load((string)modelpaths["a10"]), 
			new Vector3(0, 125, -60000), Quaternion.Euler(0, 0, 0)) as GameObject;
		player.tag = "0_a10";
		player.GetComponent<PojulObject>().setPlayType (0); 
		//((A10aPlan)player.GetComponent<A10aPlan>()).setPlayType (0);
		player.AddComponent<planMove> ();

		gameObjectInstance.Add (player);
		myThumbnailObjs.Add (player);*/

		player = (GameObject)Instantiate(Resources.Load((string)modelpaths["car3"]), 
			new Vector3(0, 125, -60000), Quaternion.Euler(0, 0, 0)) as GameObject;
		player.tag = "0_car3";
		player.GetComponent<PojulObject>().setPlayType (0); 
		//((A10aPlan)player.GetComponent<A10aPlan>()).setPlayType (0);
		//player.AddComponent<planMove> ();

		gameObjectInstance.Add (player);
		myThumbnailObjs.Add (player);
	}

	// Update is called once per frame
	void Update () {

		//enemyBehavor ();

	}

	void instanceEnemy(){
		if(EnemyMoney > prices["a10"]*enemyGoldScale && currentInstance["1_a10"] < maxInstance["1_a10"]){
			instanceGameobject ("1", "a10");
			EnemyMoney = EnemyMoney - prices ["a10"]*enemyGoldScale;
		}

		if(EnemyMoney > prices["missile3"]*enemyGoldScale){
			if(currentInstance["1_a10"] >= maxInstance["1_a10"] && currentInstance["1_car5"] >= maxInstance["1_car5"] 
				&& currentInstance["1_missile3"] < maxInstance["1_missile3"]){
				instanceGameobject ("1", "missile3");
				EnemyMoney = EnemyMoney - prices ["missile3"]*enemyGoldScale;
			}else if(currentInstance["1_missile3"] < (maxInstance["1_missile3"]/2) ){
				instanceGameobject ("1", "missile3");
				EnemyMoney = EnemyMoney - prices ["missile3"]*enemyGoldScale;
			}
		}

		if(EnemyMoney > prices["car5"]*enemyGoldScale && currentInstance["1_car5"] < maxInstance["1_car5"]){
			instanceGameobject ("1", "car5");
			EnemyMoney = EnemyMoney - prices["car5"]*enemyGoldScale;
		}

		if(EnemyMoney > prices["missile1"]*enemyGoldScale){
			if(currentInstance["1_a10"] >= maxInstance["1_a10"] && currentInstance["1_car5"] >= maxInstance["1_car5"] 
				&& currentInstance["1_missile1"] < maxInstance["1_missile1"]){
				instanceGameobject ("1", "missile1");
				EnemyMoney = EnemyMoney - prices ["missile1"]*enemyGoldScale;
			}else if(currentInstance["1_missile1"] < (maxInstance["1_missile1"]/2) ){
				instanceGameobject ("1", "missile1");
				EnemyMoney = EnemyMoney - prices ["missile1"]*enemyGoldScale;
			}
		}

		if(EnemyMoney > prices["car2"]*enemyGoldScale && currentInstance["1_car2"] < maxInstance["1_car2"]){
			instanceGameobject ("1", "car2");
			EnemyMoney = EnemyMoney - prices ["car2"]*enemyGoldScale;
		}

		if (currentInstance ["1_a10"] >= maxInstance ["1_a10"] && currentInstance ["1_car5"] >= maxInstance ["1_car5"]) {
			if (EnemyMoney > prices ["car3"]*enemyGoldScale && currentInstance["1_car3"] < maxInstance["1_car3"]) {
				instanceGameobject ("1", "car3");
				EnemyMoney = EnemyMoney - prices ["car3"]*enemyGoldScale;
			}
			/*if(EnemyMoney > prices ["missile2"]*enemyGoldScale && currentInstance["1_missile2"] < maxInstance["1_missile2"]){
				instanceGameobject ("1", "missile2");
				EnemyMoney = EnemyMoney - prices ["missile2"]*enemyGoldScale;
			}*/
		}

		if (currentInstance ["1_a10"] >= maxInstance ["1_a10"] && currentInstance ["1_car5"] >= maxInstance ["1_car5"] 
			&& currentInstance["1_car3"] >= maxInstance["1_car3"]) {
			if(EnemyMoney > prices ["transport1"]*enemyGoldScale && currentInstance["1_transport1"] < maxInstance["1_transport1"]){
				instanceGameobject ("1", "transport1");
				EnemyMoney = EnemyMoney - prices ["transport1"]*enemyGoldScale;
			}
		}
	}

	void updateEnemyAttacksCounts(){
		UImanager.attacks_1.Remove (null);
		a10AttackNum = 0;
		car3AttackNum = 0;
		transport1AttackNum = 0;
		attackMountMissile = 0;
		for(int i = 0; i< UImanager.attacks_1.Count; i++){
			if(UImanager.attacks_1[i] == null){
				continue;
			}
			PojulObject mPojulObject = UImanager.attacks_1 [i].GetComponent<PojulObject> ();
			if(mPojulObject == null || mPojulObject.isDestoryed){
				continue;
			}
			if ("a10".Equals (mPojulObject.type)) {
				a10AttackNum = a10AttackNum + 1;
				attackMountMissile = ((A10aPlan)mPojulObject).currentMountMissle;
			} else if("car3".Equals (mPojulObject.type)){
				car3AttackNum = car3AttackNum + 1;
			}else if("transport1".Equals (mPojulObject.type)){
				attackTra = UImanager.attacks_1 [i];
				transport1AttackNum = transport1AttackNum + 1;
				((TransportType1)mPojulObject).attackTransports.Remove (null);
			}
		}
	}

	void addEnemyAttacks(){
		bool needAddA10 = false;
		bool needAddCar3 = false;
		bool needAddTransport1 = false;
		if( (currentInstance ["1_a10"] >= maxInstance ["1_a10"] && a10AttackNum < 1) ){
			needAddA10 = true;
		}
		if( (currentInstance ["1_car3"] >= maxInstance ["1_car3"] && car3AttackNum < 3) ){
			needAddCar3 = true;
		}
		if( (currentInstance ["1_transport1"] >= maxInstance ["1_transport1"] && transport1AttackNum < 1) ){
			needAddTransport1 = true;
		}
		for(int i = 0; i< attackArms_1.Count; i++){
			if(attackArms_1[i] == null){
				continue;
			}
			PojulObject mPojulObject = attackArms_1[i].GetComponent<PojulObject> ();
			if(mPojulObject == null || mPojulObject.isDestoryed){
				continue;
			}
			if(mPojulObject.type.Equals("a10") && needAddA10 && !mPojulObject.isAttackArmy){
				mPojulObject.isAttackArmy = true;
				mPojulObject.behavior = UImanager.attackBehavorId_1;
				((A10aPlan)mPojulObject).onBehavorChanged ();
				UImanager.attacks_1.Add (attackArms_1[i]);
				a10AttackNum = a10AttackNum + 1;
				needAddA10 = false;
			}
			if (mPojulObject.type.Equals ("car3") && needAddCar3 && !mPojulObject.isAttackArmy) {
				mPojulObject.isAttackArmy = true;
				mPojulObject.behavior = UImanager.attackBehavorId_1;
				UImanager.attacks_1.Add (attackArms_1[i]);
				car3AttackNum = car3AttackNum + 1;
				if(car3AttackNum >= 3){
					needAddCar3 = false;
				}
			}
			if (mPojulObject.type.Equals ("transport1") && needAddTransport1 && !mPojulObject.isAttackArmy) {
				mPojulObject.isAttackArmy = true;
				mPojulObject.behavior = UImanager.attackBehavorId_1;
				UImanager.attacks_1.Add (attackArms_1[i]);
				transport1AttackNum = transport1AttackNum + 1;
				needAddTransport1 = false;
			}

		}

	}

	void enemyManager(){
		updateEnemyAttacksCounts ();
		enemyAttackBehavor = UImanager.attackBehavorId_1;
		//Debug.Log ("gqb------>enemyManager111 attackBehavorId_1: " + UImanager.attackBehavorId_1);
		if (UImanager.attackBehavorId_1 == 1) {
			for (int i = 0; i < UImanager.attacks_0.Count; i++) {
				if (UImanager.attacks_0 [i] != null && Util.isOnNavArea2 (UImanager.attacks_0 [i].position)) {
					return;
				}
			}
			//Debug.Log ("gqb------>enemyManager222");
			if (a10AttackNum >= 1 && car3AttackNum >= 3 && transport1AttackNum >= 1) {
				UImanager.attackBehavorId_1 = 2;
				onEnemyBehavorChanged ();
			} else {
				addEnemyAttacks ();
			}
		} else if (UImanager.attackBehavorId_1 == 2) {
			if (a10AttackNum < 1 || car3AttackNum < 3 || transport1AttackNum < 1) {
				//addEnemyAttacks ();
				UImanager.attackBehavorId_1 = 1;
				onEnemyBehavorChanged ();
				return;
			}
			if (isEnemyAttackReady()) {
				UImanager.attackBehavorId_1 = 3;
				onEnemyBehavorChanged ();
			}
		} else if (UImanager.attackBehavorId_1 == 3) {
			if ( car3AttackNum < 1 ) {
				UImanager.attackBehavorId_1 = 1;
				UImanager.massId_1 = Random.Range (0, 1);
				int attackListId = Random.Range (0, 1);
				if(UImanager.massId_1 == 0){
					UImanager.attackAreaId_1 = new int[]{1,4}[attackListId];
				}else if(UImanager.massId_1 == 1){
					UImanager.attackAreaId_1 = new int[]{2,3}[attackListId];
				}
				onEnemyBehavorChanged ();
			}
		}
	}

	bool isEnemyAttackReady(){
		//Debug.Log ("gqb------>isEnemyAttackReady00000");
		if(attackMountMissile < 2){
			return false;
		}
		//Debug.Log ("gqb------>isEnemyAttackReady11111");
		if(attackTra == null || (new Vector3(attackTra.position.x, 0, attackTra.position.z) - attackMasses_1[UImanager.massId_1]).magnitude > 10 ){
			return false;
		}
		//Debug.Log ("gqb------>isEnemyAttackReady22222");
		List<Vector3> enemyMasses = new List<Vector3> ();
		enemyMasses.Add (new Vector3 ((attackMasses_1[UImanager.massId_1].x - 600), 
			300, 
			attackMasses_1[UImanager.massId_1].z
		));
		enemyMasses.Add (new Vector3 ((attackMasses_1[UImanager.massId_1].x - 600), 
			300, 
			(attackMasses_1[UImanager.massId_1].z - 1*600)
		));
		enemyMasses.Add (new Vector3 ((attackMasses_1[UImanager.massId_1].x - 600), 
			300, 
			(attackMasses_1[UImanager.massId_1].z - 2*600)
		));
		RaycastHit hit;
		for(int i = 0; i < enemyMasses.Count; i++){
			bool isHit = Physics.Raycast (enemyMasses[i], Vector3.down, out hit, 500.0f);
			if(!isHit){
				return false;
			}
			if(hit.transform.root.childCount <= 0 || hit.transform.root.GetChild(0).tag.Equals("Untagged")){
				return false;
			}
			PojulObject mPojulObject = hit.transform.root.GetChild (0).GetComponent<PojulObject> ();
			if(mPojulObject == null){
				return false;
			}
			if (mPojulObject.playerType == 0 || mPojulObject.isDestoryed) {
				return false;
			}
		}
		Debug.Log ("gqb------>isEnemyAttackReady33333");
		return true;
	}

	void onEnemyBehavorChanged(){
		for (int i = 0; i < UImanager.attacks_1.Count; i++) {
			if(UImanager.attacks_1[i] == null){
				continue;
			}
			PojulObject mPojulObject = UImanager.attacks_1 [i].GetComponent<PojulObject> ();
			if(mPojulObject == null || mPojulObject.isDestoryed){
				continue;
			}
			mPojulObject.behavior = UImanager.attackBehavorId_1;
			if ("a10".Equals (mPojulObject.type)) {
				((A10aPlan)mPojulObject).onBehavorChanged ();
			}
		}
	}

}
