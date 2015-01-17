using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Damageable))]
public class Player : MonoBehaviour {

	public float maxVelocity = 5;
	public float movementForce = 10;
	public float midairForce = 5;
	public float turnSpeed = 4;
	public float minTurnForce = 0.1f;

	public float jumpPower = 10;
	public float jumpRayLength = 1;
	public float jumpReleaseMax = 0;

	public Animator animator;


	void Start () {
		this.GetComponent<Damageable>().OnDeath += OnDeath;
	}

	void OnDeath() {
		this.transform.position = LevelState.Instance.respawnPoint.transform.position;
		this.rigidbody.velocity = Vector3.zero;
	}
	
	void Update () {
		// Get the movement directon from the axis.
		Vector3 forward = Camera.main.transform.forward;
		Vector3 sideways = Camera.main.transform.right;
		forward.y = 0;
		sideways.y = 0;
		forward = forward.normalized;
		sideways = sideways.normalized;
		Vector3 direction = Input.GetAxis("Horizontal") * sideways + Input.GetAxis("Vertical") * forward;

		// Find the force that we need to add the the player.
		Vector3 desiredVelocity = direction * maxVelocity;
		Vector3 currentVelocity = this.rigidbody.velocity.SetY(0);
		Vector3 diff = desiredVelocity - currentVelocity;
		float force = this.isGrounded() ? this.movementForce : this.midairForce; 
		this.rigidbody.AddForce(diff * force);

		// Allow jumping.
		if (Input.GetAxis("Fire1") > 0) {
			if (this.isGrounded()) {
				this.rigidbody.velocity = this.rigidbody.velocity.SetY(this.jumpPower);
			}
		} else if (this.rigidbody.velocity.y > this.jumpReleaseMax) {
			this.rigidbody.velocity = this.rigidbody.velocity.SetY(this.jumpReleaseMax);
		}

		// Rotate the player to face the direction it is moving.
		if (direction.magnitude > this.minTurnForce) {
			this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(direction), this.turnSpeed * Time.deltaTime);
		}

		this.animator.SetFloat("Speed", direction.magnitude);
		//if (this.animator.GetCurrentAnimatorStateInfo(0).nameHash == "Run") {

		//}
	}

	public bool isGrounded() {
		GameObject thingBeingStoodOn;
		return this.isGrounded(out thingBeingStoodOn);
	}

	public bool isGrounded(out GameObject thingBeingStoodOn) {
		RaycastHit hit;
		bool result = Physics.Raycast(new Ray(this.transform.position, Vector3.down), out hit, this.jumpRayLength);
		if (result) {
			thingBeingStoodOn = hit.collider.gameObject;
		} else {
			thingBeingStoodOn = null;
		}

		return result;
	}
}
