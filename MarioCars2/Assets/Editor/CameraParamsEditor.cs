using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CameraParams))]
public class CameraParamsEditor : Editor {
	void OnSceneGUI() {
		Event e = Event.current;
		switch (e.type) {
			case EventType.keyDown:
				if (e.keyCode == KeyCode.Q) {
					CameraParams p = (CameraParams)this.target;
					p.gameObject.transform.LookAt(p.gameObject.transform.parent);
				}
				break;
		}
	}
}