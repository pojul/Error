using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation1 : MonoBehaviour {

	public GameObject target;
	bool starNav =false;

	void Start () {
		//m_agent = GetComponent<NavMeshAgent>(); 
		//GetComponent<NavMeshAgent>().destination = new Vector3(-0.08f, 0.5f, -1.43f/*28897*/);
		//target = GameObject.FindGameObjectWithTag("NavTarget");
		//GetComponent<UnityEngine.AI.NavMeshAgent> ().baseOffset = 100;
		//this.gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();

		//GetComponent<UnityEngine.AI.NavMeshAgent> ().baseOffset = 300;
		//GetComponent<UnityEngine.AI.NavMeshAgent>().destination = new Vector3(0, 2, 200/*28897*/);
		//193.5
	}

	void Update () {
		if(Time.time > 5 && !starNav){
			this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = target.transform.position;
			Debug.Log ("gqb---->"+Time.time);
			starNav = true;
		}

	}
}
