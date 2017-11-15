using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour{

	public Vector2 position;
	public Vector2 parentPosition;

	public float width;
	public float height;

	protected bool isDrawing = false;

	public GUIStyle background;

	public View parentView;
	public List<View> childViews;

	public bool isShow = true;
	public bool isParentShow = true;

	public void setContentView (){
		if(parentView != null && !parentView.isDrawing){
			isDrawing = false;
			return;
		}
		isDrawing = true;

		setChildDrawing (this);
	}

	void setChildDrawing(View v){
		if(v.childViews != null && v.childViews.Count > 0){
			for(int i = 0;i< childViews.Count; i++){
				childViews [i].isDrawing = true;
				setChildDrawing(childViews [i]);
			}
		}

	}

}
