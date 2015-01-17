using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraParams : MonoBehaviour {

	void Update() {
		if (!Application.isPlaying && Input.GetKey(KeyCode.Q)) {
			this.transform.LookAt(this.transform.parent);
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.white;
		Gizmos.DrawLine(this.transform.position, this.transform.parent.position);
		Gizmos.DrawWireSphere(this.transform.position, 1);
		Gizmos.DrawWireSphere(this.transform.parent.position, 1);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 3f);
	}
}