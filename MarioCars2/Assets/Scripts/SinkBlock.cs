using UnityEngine;
using System.Collections;

public class SinkBlock : MonoBehaviour {
	private bool playerIsOnMe;

	public float resetForce;

	private float originalY;

	// Use this for initialization
	void Start () {
		this.playerIsOnMe = false;
		this.originalY = this.transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		if (this.playerIsOnMe) {
			//Vector3 pos = this.transform.position;
			//pos.y += (-this.fallRate * Time.deltaTime);

			//this.transform.position = pos;
			//Debug.Log("HERE");
		}
	}


	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.GetComponent<Player>() != null) {
			Debug.Log("I HIT THE PLAYER");
			this.playerIsOnMe = true;
			this.rigidbody.isKinematic = false;
		}
	}

	void OnCollisionExit(Collision collision) {
		if (collision.gameObject.GetComponent<Player>() != null) {
			Debug.Log("NO LONGER HITTING THE PLAYER");
			this.playerIsOnMe = false;
			this.rigidbody.AddForce(new Vector3(0, resetForce, 0));
		}
	}
}
