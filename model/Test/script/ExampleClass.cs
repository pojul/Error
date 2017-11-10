using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleClass : MonoBehaviour {

	//public GameObject target;

	private Animator ani;

	// Use this for initialization
	void Start () {
		
		//---------------- 先获取材质 -------------------------  
        //获取自身和所有子物体中所有MeshRenderer组件  
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();    
        //新建材质球数组  
        Material[] mats = new Material[meshRenderers.Length];    
        for (int j = 0; j < meshRenderers.Length; j++) {
            //生成材质球数组   
            mats[j] = meshRenderers[j].sharedMaterial;  
        }  
		
		//---------------- 合并 Mesh -------------------------  
        //获取自身和所有子物体中所有MeshFilter组件  
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();    
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];     
        for (int i = 0; i < meshFilters.Length; i++) {  
            combine[i].mesh = meshFilters[i].sharedMesh;  
            //矩阵(Matrix)自身空间坐标的点转换成世界空间坐标的点   
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;  
            meshFilters[i].gameObject.SetActive(false);  
        }   
        //为新的整体新建一个mesh  
        transform.GetComponent<MeshFilter>().mesh = new Mesh();   
        //合并Mesh. 第二个false参数, 表示并不合并为一个网格, 而是一个子网格列表  
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, false);  
        transform.gameObject.SetActive(true);  
		
		//为合并后的新Mesh指定材质 ------------------------------  
        transform.GetComponent<MeshRenderer>().sharedMaterials = mats;   
		
		/*MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length) {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.active = false;
            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.active = true;*/
		

		/*Vector3 target_pos = target.transform.position;
		Vector3 my_pos = transform.position;

		Vector3 from = Vector3.right;  
		Vector3 to = target_pos - my_pos;  
		//transform.rotation = Quaternion.FromToRotation(from, to);
		//transform.rotation = Quaternion.LookRotation(target.transform.position);

		//paoTransform = transform.FindChild ("pao");

		//LODGroup l = (LODGroup)transform.GetComponent<LODGroup>();
		//l.transform*/
		
		ani = (Animator)transform.GetComponent<Animator> ();
		//ani.
		ani.SetBool ("test", true);
		
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
		//transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), 1 * Time.deltaTime);

	}
}
