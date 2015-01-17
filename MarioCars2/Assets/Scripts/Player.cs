using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Damageable))]
public class Player : MonoBehaviour {

	public float movementForce = 10;
	public float jumpForce = 10;
	public float jumpRayLength = 1;

	void Start () {
		this.GetComponent<Damageable>().OnDeath += OnDeath;
	}

	void OnDeath() {
		this.transform.position = LevelState.Instance.respawnPoint.transform.position;
		this.rigidbody.velocity = Vector3.zero;
	}
	
	void Update () {
		Vector3 forward = Camera.main.transform.forward;
		Vector3 sideways = Camera.main.transform.right;
		forward.y = 0;
		sideways.y = 0;
		Vector3 force = Input.GetAxis("Horizontal") * sideways + Input.GetAxis("Vertical") * forward;
		force *= this.movementForce;
		//force.y = this.rigidbody.velocity.y;
		this.rigidbody.AddForce(force);

		// This is hacky, try something better soon!
		if (Input.GetAxis("Fire1") > 0) {
			if (Physics.Raycast(new Ray(this.transform.position, Vector3.down), this.jumpRayLength)) {
				this.rigidbody.AddForce(Vector3.up * this.jumpForce);
			}
		}
	}
}
