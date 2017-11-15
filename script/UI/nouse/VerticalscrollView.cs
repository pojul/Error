using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalscrollView : View{

	public ArrayList datas;
	public View itemView;

	public VerticalscrollView(Vector2 position, float width, float height){
		this.position = position;
		this.width = width;
		this.height = height;
	}

	void OnGUI(){
		if(isDrawing && isParentShow && isShow){
			Draw ();
		}
	}

	void Draw(){
		
	}

}
