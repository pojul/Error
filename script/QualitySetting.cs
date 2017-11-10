using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualitySetting : MonoBehaviour {

	// Use this for initialization
	void Start () {
		QualitySettings.SetQualityLevel(0, true);
	}


	/*void OnGUI() {
		string[] names = QualitySettings.names;
		GUILayout.BeginArea(new Rect(10, 30, 200, 400));
		GUILayout.BeginVertical();
		int i = 0;
		while (i < names.Length) {
			if (GUILayout.Button(names[i]))
				QualitySettings.SetQualityLevel(i, true);
			i++;
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();

		if (GUI.Button(new Rect(10, 200, 80, 60), " - ")){
			QualitySettings.lodBias = QualitySettings.lodBias - 0.1f;
		}
		if (GUI.Button(new Rect(90, 200, 240, 60), "lodBias: " + QualitySettings.lodBias)){
			//QualitySettings.lodBias = QualitySettings.lodBias + 0.1f;
		}
		if (GUI.Button(new Rect(330, 200, 80, 60), " + ")){
			QualitySettings.lodBias = QualitySettings.lodBias + 0.1f;
		}

	}*/

}
