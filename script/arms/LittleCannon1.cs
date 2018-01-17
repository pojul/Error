using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LittleCannon1 : PojulObject {

	private Transform fireTransform;
	private Transform aimTransform;
	private Transform target;
	private Transform panTransform;
	private Transform paoTransform;
	private Transform playerView;

	private float fireInterval = 12f;
	private float lastFileTime = 0.0f;
	private float aimSpeed = 20f;
	private float rayCastEnemyDis = 6000;
	private float height = -190;

	public float dzMainCamera = 200;
	public float dyMainCamera = 150;

	private GameObject preShell;
	//血量条
	public Slider sliderHealth;

	// Use this for initialization
	void Start () {

		RectTransform mRectTransform = sliderHealth.GetComponent<RectTransform> ();
		if (transform.position.z < 0) {
			tag = "0_littlecannon1";
			enemyId = "1";
			aimSpeed = 20f;
			rayCastEnemyDis = 6000;
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.251f, 0.647f, 0.78f);
		} else {
			tag = "1_littlecannon1";
			enemyId = "0";
			aimSpeed = 40f;
			rayCastEnemyDis = 16000;
			sliderHealth.fillRect.GetComponent<Image> ().color = new Color(0.698f, 0.255f, 0.157f);
		}

		if(playerType == 1){
			mRectTransform.sizeDelta = new Vector2 (Screen.width / 18, Screen.width / 60);
		}
		sliderHealth.value = sliderHealth.maxValue;

		panTransform = transform.FindChild ("little_cannon1").FindChild ("pan");
		paoTransform = panTransform.FindChild ("pao");
		fireTransform = paoTransform.FindChild("fire");
		aimTransform = transform.FindChild ("aim");
		playerView = paoTransform.FindChild("playerView");

		string[] strs = transform.tag.Split ('_');
		playerId = strs [0];
		type = strs [1];

		createShell ();
		InvokeRepeating ("findEnemy", 1.5f, 1.5f);

		InvokeRepeating ("behaviorListener", 0.5f, 0.5f);

		//InvokeRepeating ("fire", 5f, 5f);
	}
	
	// Update is called once per frame
	void Update () {

		if(isDestoryed){
			return;
		}

		sliderHealth.transform.rotation = Quaternion.Euler(paoTransform.rotation.eulerAngles.x , 
			Camera.main.transform.rotation.eulerAngles.y,
			paoTransform.rotation.eulerAngles.z);

		if(MissileAimedTra != null){
			target = MissileAimedTra;
		}

		if(target != null){
			aimEnemy (target);
		}

	}

	void createShell(){
		GameObject shell1 = (GameObject)Instantiate(Resources.Load("Prefabs/arms/pao"), 
			(fireTransform.position + fireTransform.forward*2), fireTransform.rotation) as GameObject;
		shell1.tag = "shell2";
		shell1.GetComponent<Renderer> ().enabled = false;
		shell1.transform.parent = fireTransform;
		preShell = shell1;
	}

	void behaviorListener(){
		if (MissileAimedTra == null) {
			isMissileAimed = false;
		} else {
			PojulObject p = MissileAimedTra.gameObject.GetComponent<PojulObject> ();
			if (p.missTarget) {
				isMissileAimed = false;
			} else {
				isMissileAimed = true;
			}
		}

		rayCastEnemy ();
	}

	void findEnemy(){
		if(isDestoryed){
			return;
		}
		if(MissileAimedTra != null){
			target = MissileAimedTra;
			return;
		}

		float dangeroustDis = 16000.0f;
		Transform dangeroustEnemy = null;

		if(playerId.Equals("1") && planMove.player != null){
			PojulObject mPojulObject = planMove.player.GetComponent<PojulObject> ();
			float tempDis = (transform.position - planMove.player.position).magnitude;
			if(mPojulObject != null && !mPojulObject.isDestoryed && tempDis < dangeroustDis && mPojulObject.type.Equals("a10") ){
				dangeroustEnemy = planMove.player;
				dangeroustDis = tempDis;
			}
		}

		List<Transform> allNearEnemys = null;
		if("0".Equals(playerId)){
			allNearEnemys = GameInit.attackArms_1;
		}else if("1".Equals(playerId)){
			allNearEnemys = GameInit.attackArms_0;
		}

		for (int i = 0; i < allNearEnemys.Count; i++) {
			Transform tempDangeroustEnemy = allNearEnemys[i];
			if(tempDangeroustEnemy == null || !Util.isOnMyNavArea1(tempDangeroustEnemy.position, playerId) || 
				!tempDangeroustEnemy.GetComponent<PojulObject>() || tempDangeroustEnemy.GetComponent<PojulObject>().isDestoryed){
				continue;
			}
			string[] strs = tempDangeroustEnemy.tag.Split ('_');
			if(strs.Length != 2){
				continue;
			}
			float tempDis = (transform.position - tempDangeroustEnemy.position).magnitude;
			if(tempDis < dangeroustDis && ("car3".Equals (strs[1]) || "a10".Equals (strs[1])) ){
				//Debug.Log (transform.tag +  "; gqb------>findDangeroust: " +  tempDangeroustEnemy.tag );
				dangeroustEnemy = tempDangeroustEnemy;
				dangeroustDis = tempDis;
			}
			if(tempDis < 16000){
				Util.AddNearEnemys (tempDangeroustEnemy, playerId);
			}
		}

		if(dangeroustEnemy != null && dangeroustEnemy.FindChild("aim") != null){
			target = dangeroustEnemy.FindChild("aim");
			//Debug.Log ("find Enemy: " + target.root.name);
		}

	}

	void aimEnemy(Transform enemyTransform){
		if(target == null || isPanDestoryed || isDestoryed){
			return;
		}

		Quaternion lookFireRotation = Quaternion.LookRotation(enemyTransform.position - fireTransform.position);
		 
		float dRolY = Util.getDirectDRol(fireTransform.rotation.eulerAngles.y, lookFireRotation.eulerAngles.y, aimSpeed);
		panTransform.rotation = Quaternion.Euler (new Vector3(0, (panTransform.eulerAngles.y + dRolY), 0));

		float dRolX = Util.getDirectDRol(fireTransform.rotation.eulerAngles.x, lookFireRotation.eulerAngles.x, aimSpeed);
		paoTransform.rotation = Quaternion.Euler (new Vector3((paoTransform.eulerAngles.x + dRolX), panTransform.eulerAngles.y, 0));
	}

	void rayCastEnemy(){
		
		if(target == null || isDestoryed){
			return;
		}

		//Debug.DrawRay(fireTransform.position, fireTransform.forward*16100, Color.white);

		RaycastHit hit;
		if(Physics.Raycast (fireTransform.position, fireTransform.forward, out hit, rayCastEnemyDis)){
			//Debug.Log (transform.tag +  "; gqb------>rayCastEnemy111111: " +  hit.transform.name );
			if(hit.transform != null && (Time.time - lastFileTime) > fireInterval){
				//Debug.Log ("; gqb------>rayCastEnemy00000: " +  hit.transform.name );
				Transform root = null;
				PojulObject mPojulObject = null;

				if(hit.transform.root.childCount > 0){
					root = hit.transform.root.GetChild(0);
					mPojulObject = root.GetComponent<PojulObject> ();
				}

				if(mPojulObject == null){
					root = hit.transform.root;
					mPojulObject = root.gameObject.GetComponent<PojulObject> ();
				}
				//Debug.Log ("; gqb------>rayCastEnemy111111: " +  hit.transform.name );
				if(mPojulObject != null && !mPojulObject.isDestoryed && enemyId.Equals(mPojulObject.playerId) && 
					( "a10".Equals (mPojulObject.type) || "car3".Equals (mPojulObject.type) ) ){
					lastFileTime = Time.time;
					fire ();
				}
			}
		}
	}

	void fire(){
		GameObject fire = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/paoFire"), 
			fireTransform.position, fireTransform.rotation) as GameObject;
		fire.tag = "paoFire";
		fire.transform.parent = transform;
		shootShell ();
	}

	void shootShell(){
		if(preShell == null){
			createShell ();
		}
		((ShellType1)preShell.GetComponent<ShellType1> ()).shoot(51000, 0);
		createShell ();
	}

	public override void isFired(RaycastHit hit, Collision collision, int type){
		if (isDestoryed) {
			return;
		}

		if (type == 2) {
			Vector3 hitPoint;
			if (collision != null) {
				hitPoint = collision.contacts[0].point;
			} else {
				hitPoint = hit.point;
			}
			sliderHealth.value = sliderHealth.value - 28;
			if(sliderHealth.value <= 0){
				isDestoryed = true;
				isPanDestoryed = true;
				DestoryAll (hitPoint, 30000.0f);
				return;
			}
		}else if(type == 3){
			sliderHealth.value = sliderHealth.value - 37;
			if (sliderHealth.value <= 0) {
				isDestoryed = true;
				isPanDestoryed = true;
				DestoryAll (collision.contacts[0].point, 30000.0f);
				return;
			}
		}
	}

	void DestoryAll(Vector3 point, float force){
		if(playerType == 0){
			UImanager.isOnLeave = true;
			Camera.main.gameObject.AddComponent<Rigidbody> ();
			Camera.main.gameObject.GetComponent<Rigidbody> ().drag = 1;
			Camera.main.gameObject.GetComponent<Rigidbody> ().angularDrag = 1;
			Camera.main.gameObject.GetComponent<Rigidbody> ().useGravity = false;
		}

		panTransform.gameObject.AddComponent<Rigidbody> ();
		paoTransform.gameObject.AddComponent<Rigidbody> ();
		panTransform.gameObject.GetComponent<MeshCollider> ().convex = true;
		paoTransform.gameObject.GetComponent<MeshCollider> ().convex = true;
		panTransform.parent = null;
		paoTransform.parent = null;

		Vector3 explosionPos = new Vector3 (point.x, 
			(point.y - 200), 
			point.z);
		Collider[] colliders = Physics.OverlapSphere(explosionPos, 200.0f);
		foreach (Collider hit in colliders){
			if (hit.GetComponent<Rigidbody>() && hit.transform != Camera.main.transform){
				hit.GetComponent<Rigidbody>().AddExplosionForce(force, explosionPos, 200.0f);
			}  
		}
		Invoke ("destoryAll", 30);
		if(aimTransform != null){
			Destroy (aimTransform.gameObject);
		}
		if(preShell != null){
			Destroy (preShell.gameObject);
		}
	}

	public void destoryAll(){
		if(panTransform != null){
			Destroy (panTransform.gameObject);
		}
		if(paoTransform != null){
			Destroy (paoTransform.gameObject);
		}
		Destroy(transform.root.gameObject);
	}

	public override void setPlayType(int playerType){
		this.playerType = playerType;
		if (playerType == 0) {
			//mCanvas.enabled = false;
			sliderHealth.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, 0);
			PlanControls.newPoint1Rolation = transform.rotation.eulerAngles.y;

			if(!transform.gameObject.GetComponent<planMove> ()){
				transform.gameObject.AddComponent<planMove> ();
			}

			planMove.player = transform;
			planMove.speed = 0;
			planMove.maxSpeed = 0;
			planMove.maxAccelerate = 0f;
			PlanControls.rorateSpeed = 35f;
			planMove.rolSpeed = 7.0f;
			if(fireTransform == null){
				fireTransform = transform.FindChild ("car_type3_lod0").FindChild ("pan").FindChild("pao").FindChild("fire");
			}
			WorldUIManager.fireAimTra = fireTransform;
			WorldUIManager.fireAimDistance = 12000.0f;
			UImanager.fireInterval = fireInterval;
			GameObject.FindGameObjectWithTag ("mainUI").GetComponent<UImanager> ().setPlayerUI ("car3");
			if(Camera.main.gameObject.GetComponent<Rigidbody>()){
				Destroy(Camera.main.gameObject.GetComponent<Rigidbody>());
			}
		} else if (playerType == 1) {
			/*
			*need judge is in nav area
			*/
			sliderHealth.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Screen.width / 18, Screen.width / 60);

			if(transform.gameObject.GetComponent<planMove> ()){
				Destroy (transform.gameObject.GetComponent<planMove> ());
			}

			planMove.player = null;
			WorldUIManager.fireAimTra = null;
			WorldUIManager.fireAimDistance = 0.0f;
			Camera.main.gameObject.AddComponent<Rigidbody> ();
			Camera.main.gameObject.GetComponent<Rigidbody> ().drag = 1;
			Camera.main.gameObject.GetComponent<Rigidbody> ().angularDrag = 1;
			Camera.main.gameObject.GetComponent<Rigidbody> ().useGravity = false;
			//GameObject.FindGameObjectWithTag ("mainUI").GetComponent<UImanager> ().setPlayerUI ("car3");
		}
	}

	public void rotatePan(float rotationPan, float rotationPao){
		if(isDestoryed){
			return;
		}
		panTransform.rotation = Quaternion.Slerp(panTransform.rotation,
			Quaternion.Euler (new Vector3(0, PlanControls.newPoint1Rolation, 0)),
			3.5f * Time.deltaTime
		);
		float paoRol = paoTransform.rotation.eulerAngles.x - rotationPao * 5.0f;
		//Debug.Log ("gqb------>paoRol: " + paoRol);
		if(paoRol > 89 && paoRol < 271){
			if ((paoRol - 89) < (271 - paoRol)) {
				paoRol = 89;
			} else {
				paoRol = 271;
			}
		}
		paoTransform.rotation =  Quaternion.Slerp(paoTransform.rotation,
			Quaternion.Euler (new Vector3(paoRol, panTransform.rotation.eulerAngles.y, 0)),
			3.5f * Time.deltaTime
		);
	}

	public void rotateCamera(){
		if(isDestoryed){
			return;
		}
		Camera.main.transform.position = playerView.position;
		Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, 
			playerView.rotation, 3.4f * Time.deltaTime);
		//98.2 -54.1
	}

	public override void fireOfPlayer(){
		if(isDestoryed){
			return;	
		}
		fire ();
	}

}
