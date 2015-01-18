using UnityEngine;
using System.Collections;

public class BouncyBlock : MonoBehaviour {
	public float bouncyForce;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision c) {
		Player player = c.gameObject.GetComponent<Player>();
		if (player != null)
		{
			Debug.Log ("Hit the bouncy");

			GameObject hitObject;
			bool isGrounded = player.isGrounded(out hitObject);

			if (isGrounded && hitObject == this.gameObject) {
				Debug.Log("The hit was on top.");
				player.rigidbody.AddForce(new Vector3(0, this.bouncyForce, 0));
			}
		}
	}
}
