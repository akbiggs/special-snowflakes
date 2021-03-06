﻿using UnityEngine;
using System.Collections;

public class CameraTrigger : MonoBehaviour {

	private Vector3 newCameraOffset;
	private Quaternion newCameraRotation;

	void Start() {
		Transform parameters = this.transform.FindChild("CameraParams");

		this.newCameraOffset = parameters.position - this.transform.position;
		this.newCameraRotation = parameters.rotation;
	}

	void OnTriggerExit(Collider collider) {
		Player player = collider.gameObject.GetComponent<Player>();
		if (player != null) {
			PlayerCamera pCamera = Camera.main.GetComponent<PlayerCamera>();

			Vector3 oldOffset = pCamera.difference;
			pCamera.difference = this.newCameraOffset;
			this.newCameraOffset = oldOffset;

			Quaternion oldRotation = pCamera.targetRotation;
			pCamera.targetRotation = this.newCameraRotation;
			Debug.Log(pCamera.targetRotation.eulerAngles.ToString());
			this.newCameraRotation = oldRotation;
		}
	}
}
