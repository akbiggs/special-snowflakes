using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraParams : MonoBehaviour {

	public GameObject playerDummy;

	void Update() {
		Transform other = this.playerDummy != null ? this.playerDummy.transform : this.transform.parent;
		if (!Application.isPlaying && Input.GetKey(KeyCode.Q)) {
			this.transform.LookAt(other);
		}
	}

	void OnDrawGizmos() {
		Vector3 other = this.playerDummy != null ? this.playerDummy.transform.position : this.transform.parent.position;
		Gizmos.color = Color.white;
		Gizmos.DrawLine(this.transform.position, other);
		Gizmos.DrawWireSphere(this.transform.position, 1);
		Gizmos.DrawWireSphere(other, 1);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 3f);
	}
}