using UnityEngine;
using System.Collections;

public class DetonatorTest : MonoBehaviour
{
	public GameObject[] detonatorPrefabs;
	public float explosionLife = 10;
	public float timeScale = 1.0f;
	public float detailLevel = 1.0f;

    private void Start()
    {
		InvokeRepeating ("fire", 5, 5);
    }

	void fire(/*int typeId, Vector3 hitPoint*/){

		fire (((int)Time.time)%2);

	}

	void fire(int detonatorId){
		Detonator dTemp = (Detonator)detonatorPrefabs[detonatorId].GetComponent("Detonator");

		float offsetSize = dTemp.size/4;//+ new Vector3(offsetSize, offsetSize, offsetSize);
		Vector3 hitPoint = new Vector3(0,0,0);
		GameObject exp = (GameObject) Instantiate(detonatorPrefabs[detonatorId], hitPoint, Quaternion.identity);
		dTemp = (Detonator)exp.GetComponent("Detonator");
		dTemp.detail = detailLevel;


		Destroy(exp, explosionLife);
	}

}
