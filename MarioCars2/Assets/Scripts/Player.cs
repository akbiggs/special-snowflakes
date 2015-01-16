using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float movementForce = 10;
	public float jumpForce = 10;

	void Start () {
	
	}
	
	void Update () {
		Vector3 forward = Camera.main.transform.forward;
		Vector3 sideways = Camera.main.transform.right;
		forward.y = 0;
		sideways.y = 0;
		Vector3 force = Input.GetAxis("Horizontal") * sideways + Input.GetAxis("Vertical") * forward;
		force *= this.movementForce;
		this.rigidbody.AddForce(force);

		// This is hacky, try something better soon!
		if (Input.GetAxis("Fire1") > 0) {
			if (Physics.Raycast(new Ray(this.transform.position, Vector3.down), 1f)) {
				this.rigidbody.AddForce(Vector3.up * this.jumpForce);
			}
		}
	}
}
