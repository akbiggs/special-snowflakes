using UnityEngine;
using System.Collections;

public class CameraTriggerArea : MonoBehaviour {

	private Vector3 newCameraOffset;
	private Quaternion newCameraRotation;

	void Start() {
		Transform parameters = this.transform.FindChild("CameraParams");

		this.newCameraOffset = parameters.position - this.transform.position;
		this.newCameraRotation = parameters.rotation;
	}

	void OnTriggerEnter(Collider collider) {
		Player player = collider.gameObject.GetComponent<Player>();
		if (player != null) {
			PlayerCamera pCamera = Camera.main.GetComponent<PlayerCamera>();
			pCamera.difference = this.newCameraOffset;
			pCamera.targetRotation = this.newCameraRotation;
		}
	}
}
