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

	public Animation charAnimation;


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

		if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 ) {
			//If the player is attempting to move in any direction.
			this.charAnimation.Play("Run");
		} else {
			//If the player is not pressing any move keys.
			this.charAnimation.Play("Idle_1");
		}

		Vector3 desiredVelocity = direction * maxVelocity;
		Vector3 currentVelocity = this.rigidbody.velocity;
		currentVelocity.y = 0;

		Vector3 diff = desiredVelocity - currentVelocity;
		float force = this.isGrounded() ? this.movementForce : this.midairForce; 
		this.rigidbody.AddForce(diff * force);

		if (Input.GetAxis("Fire1") > 0) {
			if (this.isGrounded()) {
				this.rigidbody.velocity = this.rigidbody.velocity.SetY(this.jumpPower);
			}
		} else if (this.rigidbody.velocity.y > this.jumpReleaseMax) {
			this.rigidbody.velocity = this.rigidbody.velocity.SetY(this.jumpReleaseMax);
		}

		Vector3 lookDir = new Vector3(direction.x, 0, direction.z);
		//this.transform.Rotate(lookDir);
		this.transform.LookAt(this.transform.position + (lookDir * 5));
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
