using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Damageable))]
public class Player : MonoBehaviour {

	public float maxVelocity = 5;
	public float movementForce = 10;
	public float midairForce = 5;

	public float jumpPower = 10;
	public float jumpRayLength = 1;
	public float jumpReleaseMax = 0;

	public GameObject forwardReference;

	// TODO: This is hacky - plz fix.
	public bool isGrounded {
		get { return Physics.Raycast(new Ray(this.transform.position, Vector3.down), this.jumpRayLength); }
	}

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
		forward = forward.normalized;
		sideways = sideways.normalized;
		Vector3 direction = Input.GetAxis("Horizontal") * sideways + Input.GetAxis("Vertical") * forward;

		Vector3 desiredVelocity = direction * maxVelocity;
		Vector3 currentVelocity = this.rigidbody.velocity;
		currentVelocity.y = 0;

		Vector3 diff = desiredVelocity - currentVelocity;
		float force = this.isGrounded ? this.movementForce : this.midairForce; 
		this.rigidbody.AddForce(diff * force);

		if (Input.GetAxis("Fire1") > 0) {
			if (this.isGrounded) {
				this.rigidbody.velocity = this.rigidbody.velocity.SetY(this.jumpPower);
			}
		} else if (this.rigidbody.velocity.y > this.jumpReleaseMax) {
			this.rigidbody.velocity = this.rigidbody.velocity.SetY(this.jumpReleaseMax);
		}
	}
}
