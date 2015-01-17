using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

	public GameObject following;
	public float moveSpeed;
	public Vector3 difference { get; set; }

	void Start() {
		this.difference = this.transform.position - this.following.transform.position;
	}

	void FixedUpdate () {
		Vector3 desired = this.following.transform.position + this.difference;
		this.transform.position += (desired - this.transform.position) * Time.fixedDeltaTime * this.moveSpeed;
		this.transform.rotation = Quaternion.LookRotation(this.following.transform.position - this.transform.position);
	}
}
