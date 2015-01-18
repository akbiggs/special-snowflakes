using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

	public GameObject following;
	public float moveSpeed;
	public Vector3 difference { get; set; }

	public float rotationSpeed;
	public Quaternion targetRotation;

	void Start() {
		this.difference = this.transform.position - this.following.transform.position;
		this.targetRotation = this.transform.localRotation;
	}

	void FixedUpdate () {
		Vector3 desired = this.following.transform.position + this.difference;
		this.transform.position += (desired - this.transform.position) * Time.fixedDeltaTime * this.moveSpeed;

		if (Quaternion.Angle(this.targetRotation, this.transform.localRotation) > 0.001f) {
			this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation,
			                                                        this.targetRotation,
			                                                        this.rotationSpeed * Time.fixedDeltaTime);
		}
	}
	
}
