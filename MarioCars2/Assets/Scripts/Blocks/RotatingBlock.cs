using UnityEngine;
using System.Collections;

public class RotatingBlock : MonoBehaviour {

	public Vector3 rotateSpeed;
	
	void Start () {
		this.rigidbody.angularVelocity = rotateSpeed;
		//this.rigidbody.AddTorque(rotateSpeed);
	}

	void Update() {

		//this.rigidbody.ad
	}
}
