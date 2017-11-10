using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanControls : MonoBehaviour {

    public GUIStyle controlPressStyle;
	public GUIStyle controlNormalStyle;

	private float Controls1RadioPow = Mathf.Pow(Screen.height*1.65f/10, 2);
	private float Controls2RadioPow = Mathf.Pow(Screen.height/12, 2);
	private float Controls3RadioPow = Mathf.Pow(Screen.height*1.65f/10, 2);

    //control1 : 水平方向控制
	private float Controls1CenterX = Screen.width*0.6f/10 + Screen.height*1.6f/10;
	private float Controls1CenterY = Screen.height*8.15f/10;
	public GUIStyle controlCircle1astyle;
	private float controlCircle1aSize = Screen.height*3.2f/10;
	private float controlCircle1aX = Screen.width*0.6f/10;
	private float controlCircle1aY = Screen.height*6.55f/10;
	/*public GUIStyle controlpoint1bstyle;
	private float controlCircle1bSize = Screen.height*1.25f/10;
	private float controlCircle1bX = Screen.width*0.8f/10 + Screen.height*0.625f/10;
	private float controlCircle1bY = Screen.height*7.425f/10;*/
	//public GUIStyle controlpoint1style;
	public static float oldPoint1Rolation = -1.0f;
	public static float newPoint1Rolation = 0.0f;
	/*private float centerControlpoint1X = 0.0f;
	private float centerControlpoint1Y = 0.0f;
	//private float controlpoint1Size = Screen.height*1.25f/30;
	private float controlpoint1Size = Screen.height*0.635f/10;
	private float controlpoint1X;
	private float controlpoint1Y;*/

	//control2 ： 翻转方向控制
	/*public GUIStyle control2Circlestyle;
	private float control2CircleSize = Screen.height*5f/30;
	private float control2CircleCenterX = Screen.width*7f/10 - Screen.height*2.5f/30;
	private float control2CircleCenterY = Screen.height*8.05f/10;
	private float control2CircleX = Screen.width*7f/10 - Screen.height*5f/30;
	private float control2CircleY = Screen.height*8.05f/10 - Screen.height*2.5f/30;
	public GUIStyle control2Pointstyle;
	private float control2PointSize = Screen.height*5.2f/90;
	private float oldPoint2Rolation = -1.0f;*/
	public static int newPoint2Rolation = 0;
	/*private float centerControlpoint2X = 0.0f;
	private float centerControlpoint2Y = 0.0f;
	private float controlpoint2X = 0.0f;
	private float controlpoint2Y = 0.0f;
	public GUIStyle controlpoint2bstyle;
	private float controlCircle2bSize = Screen.height*2.5f/30;
	private float controlCircle2bX = Screen.width*7f/10 - Screen.height*3.75f/30;
	private float controlCircle2bY = Screen.height*8.05f/10 - Screen.height*1.25f/30;*/

	//control3 ： 垂直方向控制
	public GUIStyle control3Circlestyle;
	private float control3CircleSize = Screen.height*3.2f/10;//Screen.height*5f/30;
	private float control3CircleCenterX = Screen.width*9.4f/10 - Screen.height*1.6f/10;//Screen.width*8.15f/10 - Screen.height*2.5f/30;
	private float control3CircleCenterY = Screen.height*8.15f/10;
	private float control3CircleX = Screen.width*9.4f/10 - Screen.height*3.2f/10;
	private float control3CircleY = Screen.height*6.55f/10;
	
	private float oldPoint3Rolation = -1.0f;
	public static float newPoint3Rolation = 0.0f;
	/*public GUIStyle control3Pointstyle;
	private float control3PointSize = Screen.height*0.635f/10;
	private float centerControlpoint3X = 0.0f;
	private float centerControlpoint3Y = 0.0f;
	private float controlpoint3X = 0.0f;
	private float controlpoint3Y = 0.0f;*/
	/*public GUIStyle controlpoint3bstyle;
	private float controlCircle3bSize = Screen.height*1.25f/10;
	private float controlCircle3bX = Screen.width*9.2f/10 - Screen.height*1.875f/10;
	private float controlCircle3bY = Screen.height*7.425f/10;*/ 
	
	Gyroscope gyro;
	
	// Use this for initialization
	void Start () {
		Input.multiTouchEnabled = true;
		controlCircle1astyle = controlNormalStyle;
		control3Circlestyle = controlNormalStyle;
		
		gyro = Input.gyro;
		gyro.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI(){

		listenIput ();
		
		//绘制水平方向控制盘
		drawControls1();
		//绘制飞机水平翻转方向控制盘
		//drawControls2();
		//绘制飞机垂直方向控制盘
		drawControls3();


	}
	
	void drawControls1(){
		//外层盘
		if(GUI.Button(new Rect(controlCircle1aX, controlCircle1aY, controlCircle1aSize, controlCircle1aSize), 
				"", controlCircle1astyle)){
		}
		//内层盘
		/*if(GUI.Button(new Rect(controlCircle1bX, controlCircle1bY, controlCircle1bSize, controlCircle1bSize), 
				"", controlpoint1bstyle)){
					
		}*/

		//指示更新方向点位置
		if(oldPoint1Rolation == -1){
			//updatePoint1ByRolate(2);
			oldPoint1Rolation = newPoint1Rolation;
		}
		/*if(GUI.Button(new Rect(controlpoint1X, controlpoint1Y, controlpoint1Size, controlpoint1Size), 
			"", controlpoint1style)){

		}*/
		
	}
	
	/*void drawControls2(){
		//圆盘
		if(GUI.Button(new Rect(control2CircleX, control2CircleY, control2CircleSize, control2CircleSize), 
			"", control2Circlestyle)){
					
		}

		if(GUI.Button(new Rect(controlCircle2bX, controlCircle2bY, controlCircle2bSize, controlCircle2bSize), 
			"", controlpoint2bstyle)){

		}

		//指示点位置
		if(oldPoint2Rolation == -1){
			updatePoint2ByRolate(2);
			oldPoint2Rolation = newPoint2Rolation;
		}

		if(GUI.Button(new Rect(controlpoint2X, controlpoint2Y, control2PointSize, control2PointSize), 
			"", control2Pointstyle)){

		}
	}*/
	
	void drawControls3(){
		//圆盘
		if(GUI.Button(new Rect(control3CircleX, control3CircleY, control3CircleSize, control3CircleSize), 
			"", control3Circlestyle)){

		}

		/*if(GUI.Button(new Rect(controlCircle3bX, controlCircle3bY, controlCircle3bSize, controlCircle3bSize), 
			"", controlpoint3bstyle)){

		}*/

		//指示点位置
		if(oldPoint3Rolation == -1){
			//updatePoint3ByRolate(2);
			oldPoint3Rolation = newPoint3Rolation;
		}

		/*if(GUI.Button(new Rect(controlpoint3X, controlpoint3Y, control3PointSize, control3PointSize), 
			"", control3Pointstyle)){

		}*/
	}

	private bool isLeftMouseDown = false;
	private bool isControToucher1 = false;
	private bool isControToucher2 = false;
	private int controToucher1View = -1;
	private int controToucher2View = -1;
	private float ControToucher1PreRolate = -1000;
	private float ControToucher2PreRolate = -1000;
	private bool touchEnded1 = false;
	private bool touchEnded2 = false;
	private bool touchCountChanged = false;
 
	void listenIput(){

		if(Application.isMobilePlatform){
			
			//Debug.Log("gqb------>acceleration x: " + Input.acceleration.x + " ;y: " 
				//+ Input.acceleration.y + " ;z: " + Input.acceleration.z + " ;newPoint2Rolation: " + newPoint2Rolation + " ;dRol: " + dRol);
			
			accelerateListener();
			
			
			mobileInputLister ();
		}else {
			pcInputLister ();
		}
	}

	void accelerateListener(){
		int phoneRolationX = (int)(Input.acceleration.x * 90);
	    int dRolX = Mathf.Abs(newPoint2Rolation - phoneRolationX);
		if(dRolX > 1){
			newPoint2Rolation = phoneRolationX;
		}
		
		//int phoneRolationZ = (int)(Input.acceleration.z * 90);
		//int dRolZ = Mathf.Abs( - phoneRolationZ);
		//float dRolZ = phoneRolationZ*1.0f/10
		/*Debug.Log("gqb------>y: " + Input.acceleration.y + " ;z: " + Input.acceleration.z 
			+ " ;Ratey: " + gyro.rotationRate.y + " ;Ratez: " + gyro.rotationRate.z
			+ " ;attX: " + gyro.attitude.x + " ;attY: " + gyro.attitude.y
			+ " ;attZ: " + gyro.attitude.z  + " ;attW: " + gyro.attitude.w );
		if(Mathf.Abs(Input.acceleration.y) > 0.3f){
			newPoint1Rolation = newPoint1Rolation + Input.acceleration.y;
		}*/
		
	}
	
	void mobileInputLister(){
		/*if(GUI.Button(new Rect(100, 300, 800, 100), "ControToucher1PreRolate:" + ControToucher1PreRolate + 
						";ControToucher2PreRolate:" + ControToucher2PreRolate + ";touchCount:" + Input.touchCount)){
		}*/
		if(Input.touchCount == 0){
			isControToucher1 = false;
			isControToucher2 = false;
			controToucher1View = -1;
			controToucher2View = -1;
			ControToucher1PreRolate = -1000;
			ControToucher2PreRolate = -1000;
		}else if(Input.touchCount == 1){
			touchCountChanged = false;
			float touch1X = Input.touches [0].position.x;
			float touch1Y = Screen.height - Input.touches [0].position.y;
			if (Input.touches [0].phase == TouchPhase.Began) {
				checkIsTouched (touch1X, touch1Y, 1);
				touchEnded1 = false;
			}else if(Input.touches [0].phase == TouchPhase.Moved){
				touchEnded1 = false;
			}else if (Input.touches [0].phase == TouchPhase.Ended && !touchEnded1) {
				touchEnded1 = true;
				dealLeaveTouch (1);
			}

			if(isControToucher1){
				dealControToucher(touch1X, touch1Y, 1);
			}
		}else if(Input.touchCount > 1 && !touchCountChanged){
			float touch1X = Input.touches [0].position.x;
			float touch1Y = Screen.height - Input.touches [0].position.y;
			float touch2X = Input.touches [1].position.x;
			float touch2Y = Screen.height - Input.touches [1].position.y;
			if (Input.touches [0].phase == TouchPhase.Began) {
				checkIsTouched (touch1X, touch1Y, 1);
				touchEnded1 = false;
			}else if(Input.touches [0].phase == TouchPhase.Moved){
				if(!isControToucher1){
					checkIsTouched (touch1X, touch1Y, 1);
				}
				touchEnded1 = false;
			}else if (Input.touches [0].phase == TouchPhase.Ended && !touchEnded1) {
				touchEnded1 = true;
				if(dealLeaveTouch (1) == -1){
					touchCountChanged = true;
					return;
				}
			}
			
			if(isControToucher1){
				dealControToucher(touch1X, touch1Y, 1);
			}
			
			if(Input.touchCount < 2){
				return;
			}
			
			if (Input.touches [1].phase == TouchPhase.Began) {
				checkIsTouched (touch2X, touch2Y, 2);
				touchEnded2 = false;
			} else if(Input.touches [1].phase == TouchPhase.Moved){
				if(!isControToucher2){
					checkIsTouched (touch2X, touch2Y, 2);
				}
				touchEnded2 = false;
			}else if (Input.touches [1].phase == TouchPhase.Ended && !touchEnded2) {
				//Debug.Log("gqb------>dealLeaveTouch2;" + touchCountChanged);
				dealLeaveTouch (2);
				touchEnded2 = true;
			}
			if(isControToucher2){
				dealControToucher(touch2X, touch2Y, 2);
			}
			
		}
	}

	void pcInputLister(){

		//鼠标左键
		if(Input.GetMouseButtonDown(0)){
			isLeftMouseDown = true;
		}else if(Input.GetMouseButtonUp(0)){
			dealLeaveTouch (1);
			isLeftMouseDown = false;
		}

		float mouseDownX = Input.mousePosition.x;
		float mouseDownY = Screen.height - Input.mousePosition.y;
		if(isLeftMouseDown && !isControToucher1){
			checkIsTouched (mouseDownX, mouseDownY, 1);
		}

		if(isControToucher1){
			dealControToucher(mouseDownX, mouseDownY, 1);
		}
		//获得单个手指触摸屏幕坐标
		//Input.GetTouch(0).position
	}

	void checkIsTouched(float touchX, float touchY , int touchId){
		if(Mathf.Pow(touchX - Controls1CenterX, 2) + Mathf.Pow(touchY - Controls1CenterY, 2) <= Controls1RadioPow){
			setTouched (true, touchId, 1);
			controlCircle1astyle = controlPressStyle;
		}
		/*if(Mathf.Pow(touchX - control2CircleCenterX, 2) + Mathf.Pow(touchY - control2CircleCenterY, 2) <= Controls2RadioPow){
			setTouched (true, touchId, 2);
		}*/
		if(Mathf.Pow(touchX - control3CircleCenterX, 2) + Mathf.Pow(touchY - control3CircleCenterY, 2) <= Controls3RadioPow){
			setTouched (true, touchId, 3);
			control3Circlestyle = controlPressStyle;
		}
	}

	void setTouched(bool isTouched, int touchId, int whichView){
		if (1 == touchId) {
			isControToucher1 = isTouched;
			if(controToucher1View != -1 && controToucher1View != whichView && Input.touchCount > 1){
				controToucher2View = controToucher1View;
				controToucher1View = whichView;
				isControToucher2 = true;
				ControToucher2PreRolate = ControToucher1PreRolate;
				//ControToucher2PreRolate = -1000;
				ControToucher1PreRolate = -1000;
			}else{
				controToucher1View = whichView;
			}
		} else {
			isControToucher2 = isTouched;
			controToucher2View = whichView;
		}
	}

	void dealControToucher(float touchX, float touchY, int touchId){
		if(touchId == 1){
			if(controToucher1View == 1){
				onControl1Toucher (touchX, touchY, 1);
			}/*else if(controToucher1View == 2){
				onControl2Toucher (touchX, touchY, 1);
			}*/else if(controToucher1View == 3){
				onControl3Toucher (touchX, touchY, 1);
			}
		}else if(touchId == 2){
			if(controToucher2View == 1){
				onControl1Toucher (touchX, touchY, 2);
			}/*else if(controToucher2View == 2){
				onControl2Toucher (touchX, touchY, 2);
			}*/else if(controToucher2View == 3){
				onControl3Toucher (touchX, touchY, 2);
			}
		}
	}
		
	int dealLeaveTouch(int touchId){
		if(touchId == 1 && isControToucher1){
				if(controToucher1View == 1){
					controlCircle1astyle = controlNormalStyle;
					//updatePoint1ByRolate (2);
				}/*else if(controToucher1View == 2){
					updatePoint2ByRolate (2);
				}*/else if(controToucher1View == 3){
					//newPoint3Rolation = 0.0f;
					//updatePoint3ByRolate (2);
					control3Circlestyle = controlNormalStyle;
				}
			if(isControToucher2){
				isControToucher1 = isControToucher2;
				isControToucher2 = false;
				
				ControToucher1PreRolate = ControToucher2PreRolate;
				ControToucher2PreRolate = -1000;
				
				controToucher1View = controToucher2View;
				controToucher2View = -1;
				return -1;
			}else{
				isControToucher1 = false;
				controToucher1View = -1;
				ControToucher1PreRolate = -1000; 
			}
		}else if(touchId == 2 && isControToucher2){
			if(controToucher2View == 1){
				controlCircle1astyle = controlNormalStyle;
				//updatePoint1ByRolate (2);
			}/*else if(controToucher2View == 2){
				updatePoint2ByRolate (2);
			}*/else if(controToucher2View == 3){
				//newPoint3Rolation = 0.0f;
				//updatePoint3ByRolate (2);
				control3Circlestyle = controlNormalStyle;
			}
			isControToucher2 = false;
			controToucher2View = -1;
			ControToucher2PreRolate = -1000;
		}
		return 1;
	}

	void onControl1Toucher(float x, float y, int touchedId){
		float dRol = 0.0f;
		if(touchedId == 1){
			/*if(Input.touchCount > 1){
				float x2 = Input.touches [1].position.x;
				float y2 = Input.touches [1].position.y;
				if( (Mathf.Pow(x - Controls1CenterX, 2) + Mathf.Pow(y - Controls1CenterY, 2)) >
					 (Mathf.Pow(x2 - Controls1CenterX, 2) + Mathf.Pow(y2 - Controls1CenterY, 2))){
					x = x2;
					y = y2;
				}
			}*/
			if(ControToucher1PreRolate == -1000){
				ControToucher1PreRolate = getRolation(x, y, Controls1CenterX, Controls1CenterY);
				return;
			}
			float touchRol = getRolation (x, y, Controls1CenterX, Controls1CenterY);
			dRol = touchRol - ControToucher1PreRolate;
			dRol = limitMaxRlo(dRol);
			newPoint1Rolation = newPoint1Rolation + dRol;
			//updatePoint1ByRolate (2);
			ControToucher1PreRolate = touchRol;
		}else if(touchedId == 2){
			if(ControToucher2PreRolate == -1000){
				ControToucher2PreRolate = getRolation(x, y, Controls1CenterX, Controls1CenterY);
				return;
			}
			float touchRol = getRolation (x, y, Controls1CenterX, Controls1CenterY);
			dRol = touchRol - ControToucher2PreRolate;
			dRol = limitMaxRlo(dRol);
			newPoint1Rolation = newPoint1Rolation + dRol;
			//updatePoint1ByRolate (2);
			ControToucher2PreRolate = touchRol;
		}
	}

	/*void onControl2Toucher(float x, float y, int touchedId){
		float dRol = 0.0f;
		if(touchedId == 1){
			if(ControToucher1PreRolate == -1000){
				ControToucher1PreRolate = getRolation(x, y, control2CircleCenterX, control2CircleCenterY);
				return;
			}
			float touchRol = getRolation (x, y, control2CircleCenterX, control2CircleCenterY);
			dRol = touchRol - ControToucher1PreRolate;
			newPoint2Rolation = newPoint2Rolation + dRol;
			updatePoint2ByRolate (2);
			ControToucher1PreRolate = touchRol;
		}else if(touchedId == 2){
			if(ControToucher2PreRolate == -1000){
				ControToucher2PreRolate = getRolation(x, y,  control2CircleCenterX, control2CircleCenterY);
				return;
			}
			float touchRol = getRolation (x, y, control2CircleCenterX, control2CircleCenterY);
			dRol = touchRol - ControToucher2PreRolate;
			newPoint2Rolation = newPoint2Rolation + dRol;
			updatePoint2ByRolate (2);
			ControToucher2PreRolate = touchRol;
		}
	}*/

	void onControl3Toucher(float x, float y, int touchedId){
		float dRol = 0.0f;
		if(touchedId == 1){
			if(ControToucher1PreRolate == -1000){
				ControToucher1PreRolate = getRolation(x, y, control3CircleCenterX, control3CircleCenterY);
				return;
			}
			float touchRol = getRolation (x, y, control3CircleCenterX, control3CircleCenterY);
			dRol = touchRol - ControToucher1PreRolate;
			dRol = limitMaxRlo(dRol);
			newPoint3Rolation = newPoint3Rolation + dRol;
			//updatePoint3ByRolate (2);
			ControToucher1PreRolate = touchRol;
		}else if(touchedId == 2){
			if(ControToucher2PreRolate == -1000){
				ControToucher2PreRolate = getRolation(x, y, control3CircleCenterX, control3CircleCenterY);
				return;
			}
			float touchRol = getRolation (x, y, control3CircleCenterX, control3CircleCenterY);
			dRol = touchRol - ControToucher2PreRolate;
			dRol = limitMaxRlo(dRol);
			newPoint3Rolation = newPoint3Rolation + dRol;
			//updatePoint3ByRolate (2);
			ControToucher2PreRolate = touchRol;
		}
	}

	float limitMaxRlo(float dRol){
		float maxdRol = 5;
		if(dRol > maxdRol){
			dRol = maxdRol;
		}else if(dRol < -maxdRol){
			dRol = -maxdRol;
		}
		return dRol;
	}
	
	/*
	 * int type: 1 for out circle position, 2 for inside circle position
	 */
	/*void updatePoint1ByRolate (int type){
		float circleradio = 0.0f;
		if(1 == type){
			circleradio = controlCircle1aSize / 2;
		}else{
			circleradio = controlCircle1aSize/4;
		}
		centerControlpoint1X = circleradio * Mathf.Sin (Mathf.PI*2*newPoint1Rolation/360) + Controls1CenterX;
		centerControlpoint1Y = -circleradio * Mathf.Cos (Mathf.PI*2*newPoint1Rolation/360) + Controls1CenterY;
		controlpoint1X = centerControlpoint1X - controlpoint1Size/2;
		if(centerControlpoint1Y < Controls1CenterY){
			controlpoint1Y = centerControlpoint1Y - controlpoint1Size/2+3;
		}else{
			controlpoint1Y = centerControlpoint1Y - controlpoint1Size / 2;
		}
	}*/

	/*
	 * int type: 1 for out circle position, 2 for inside circle position
	 */
	/*void updatePoint2ByRolate(int type){
		float circleradio = 0.0f;
		if(1 == type){
			circleradio = control2CircleSize / 2;
		}else{
			circleradio = controlCircle2bSize/2;
		}
		centerControlpoint2X = circleradio * Mathf.Sin (Mathf.PI*2*newPoint2Rolation/360) + control2CircleCenterX;
		centerControlpoint2Y = -circleradio * Mathf.Cos (Mathf.PI*2*newPoint2Rolation/360) + control2CircleCenterY;
		controlpoint2X = centerControlpoint2X - control2PointSize/2;
		if (centerControlpoint2Y < control2CircleCenterY) {
			controlpoint2Y = centerControlpoint2Y - control2PointSize/2+3;
		} else {
			controlpoint2Y = centerControlpoint2Y - control2PointSize/2;
		}
	}*/

	/*
	 * int type: 1 for out circle position, 2 for inside circle position
	 */
	/*void updatePoint3ByRolate(int type){
		float circleradio = 0.0f;
		if(1 == type){
			circleradio = control3CircleSize / 2;
		}else{
			circleradio = control3CircleSize/4;
		}
		centerControlpoint3X = circleradio * Mathf.Sin (Mathf.PI*2*newPoint3Rolation/360) + control3CircleCenterX;
		centerControlpoint3Y = -circleradio * Mathf.Cos (Mathf.PI*2*newPoint3Rolation/360) + control3CircleCenterY;
		controlpoint3X = centerControlpoint3X - control3PointSize/2;
		if (centerControlpoint3Y < control3CircleCenterY) {
			controlpoint3Y = centerControlpoint3Y - control3PointSize/2+3;
		} else {
			controlpoint3Y = centerControlpoint3Y - control3PointSize/2;
		}
	}*/

	float getRolation(float pointCenterX, float pointCenterY, float ControlsCenterX, float ControlsCenterY){
		float dx = pointCenterX - ControlsCenterX;
		float dz = ControlsCenterY - pointCenterY;
		if(dz == 0 && dx > 0){
			return 90;
		}else if(dz == 0 && dx < 0){
			return 270;
		}else if(dz == 0 && dx == 0){
			return oldPoint1Rolation;
		}else if(dz > 0){
			float rawRolation = Mathf.Atan ((dx / dz)) * 360 / (2 * Mathf.PI) + 360;
			return rawRolation%360;
		}else if(dz < 0){
			float rawRolation = Mathf.Atan ((dx / dz)) * 360 / (2 * Mathf.PI) + 180;
			return rawRolation%360;
		}
		return oldPoint1Rolation;
	}
}
