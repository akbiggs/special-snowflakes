using UnityEngine;
using System.Collections;

public class MovingBlock : MonoBehaviour {
	//Note, you can only move in one direction at once (either horizontal or vertical)
	public Vector3 moveDirection;

	// Use this for initialization
	void Start () {
		if ( this.isHorizontalMovement() ) {
			this.rigidbody.constraints |= RigidbodyConstraints.FreezePositionY |
										 RigidbodyConstraints.FreezeRotationX | 
										 RigidbodyConstraints.FreezeRotationY |
										 RigidbodyConstraints.FreezeRotationZ;
		}
		else if ( this.isVerticalMovement() ) {
			this.rigidbody.isKinematic = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (this.isHorizontalMovement()) {
			this.rigidbody.velocity = moveDirection;
		}
	}

	void FixedUpdate() {
		if (this.isVerticalMovement()) {
			Vector3 newPos = this.transform.position + (this.moveDirection * Time.deltaTime);
			this.transform.position = newPos;
		}
	}

	public void ChangeDirection() {
		this.moveDirection = -this.moveDirection;

		if (this.isHorizontalMovement()) {
			this.rigidbody.velocity = this.moveDirection;
		}
	}

	private bool isVerticalMovement() {
		return this.moveDirection.y != 0;
	}

	private bool isHorizontalMovement() {
		return this.moveDirection.x != 0 || this.moveDirection.z != 0;
	}
}
