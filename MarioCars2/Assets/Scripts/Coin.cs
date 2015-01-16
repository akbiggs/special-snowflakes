using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {
	public float rotationSpeed;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		this.rigidbody.MoveRotation(new Quaternion(0, 10, 0, 0));
	}

	void OnCollisionEnter(Collision collision) {
		/*if (collision == Player) {

		}*/
	}
}
