using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PojulObject : MonoBehaviour {

	public bool isDestoryed = false;

	public string playerId = "";
	public string enemyId = "";
	public string type = "";
	public int behavior = 1;//1: patrol;2: ready to attack;3: attack

	public virtual void isFired (Collision collision, int type){}

}
