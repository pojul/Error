using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PojulObject : MonoBehaviour {

	public bool isSelling = false;
	public bool isSelected = false;
	public bool isAttackArmy = false;

	public bool isDestoryed = false;
	public bool isPanDestoryed = false;

	public bool isMissileAimed = false;
	public Transform MissileAimedTra = null;
	public bool isAvoidMissile = false;

	public bool missTarget = false;//for missile

	public int playerType = 1; //0: player; 1:  prefab
	public string playerId = "";
	public string enemyId = "";
	public string type = "";
	public int behavior = 1;//1: patrol;2: ready to attack;3: attack

	public Vector3 myCenter;
	public Vector3 enemyCenter;

	public virtual void isFired (RaycastHit hit, Collision collision, int type){}

	public virtual void fireMissileOfPlayer (Transform target){}

	public virtual void fireOfPlayer (){}

	public virtual void setPlayType (int playerType){}

}
