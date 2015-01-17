using UnityEngine;
using System.Collections;

public class CameraTrigger : MonoBehaviour {

	public Vector3 newCameraOffset;
	public Vector3 newCameraRotation;

	void OnTriggerExit(Collider collider) {
		Player player = collider.gameObject.GetComponent<Player>();
		if (player != null) {
			PlayerCamera pCamera = Camera.main.GetComponent<PlayerCamera>();

			Vector3 oldOffset = pCamera.difference;
			pCamera.difference = this.newCameraOffset;
			this.newCameraOffset = oldOffset;

			Quaternion oldRotation = pCamera.targetRotation;
			pCamera.targetRotation = Quaternion.Euler(this.newCameraRotation);
			this.newCameraRotation = oldRotation.eulerAngles;
		}
	}
}
