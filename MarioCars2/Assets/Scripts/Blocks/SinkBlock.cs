using UnityEngine;
using System.Collections;

public class SinkBlock : MonoBehaviour {
	public float downForce;
	public float upForce;

	private float originalY;

	// Use this for initialization
	void Start () {
		this.originalY = this.transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		if (this.transform.position.y > this.originalY) {
			this.rigidbody.velocity = Vector3.zero;

			Vector3 pos = this.transform.position;
			pos.y = this.originalY;

			this.transform.position = pos;
		}
	}


	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.GetComponent<Player>() != null) {
			this.rigidbody.AddForce(new Vector3(0, downForce, 0));
		}
	}

	void OnCollisionExit(Collision collision) {
		if (collision.gameObject.GetComponent<Player>() != null) {
			this.rigidbody.velocity = Vector3.zero;
			this.rigidbody.AddForce(new Vector3(0, upForce, 0));
		}
	}
}
