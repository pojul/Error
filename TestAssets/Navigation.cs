using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {

	//NavMeshAgent m_agent;

	// Use this for initialization
	public GameObject target;
	
	void Start () {
		//m_agent = GetComponent<NavMeshAgent>(); 
		//GetComponent<NavMeshAgent>().destination = new Vector3(-0.08f, 0.5f, -1.43f/*28897*/);
		//target = GameObject.FindGameObjectWithTag("NavTarget");
		//GetComponent<UnityEngine.AI.NavMeshAgent> ().baseOffset = 100;
		MeshRenderer m = GetComponent<MeshRenderer>();
		m.enabled = false;
		GetComponent<UnityEngine.AI.NavMeshAgent>().destination = target.transform.position;
		GetComponent<UnityEngine.AI.NavMeshAgent> ().baseOffset = 1;
		GetComponent<UnityEngine.AI.NavMeshAgent> ().speed = 600;
		//GetComponent<UnityEngine.AI.NavMeshAgent>().destination = new Vector3(0, 2, 200/*28897*/);
		//193.5
	}
	
	// Update is called once per frame
	void Update () {
		/*float speed = 10 * Time.delta * 0.1f;
		m_agent.Move(m_transform.TransformDirection(new Vector3(0,0,speed)));*/
	}
}
