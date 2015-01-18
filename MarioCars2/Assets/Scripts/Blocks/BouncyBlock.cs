using UnityEngine;
using System.Collections;

public class BouncyBlock : MonoBehaviour {
	public float bouncyForce;

	void OnCollisionEnter(Collision c) {
		Player player = c.gameObject.GetComponent<Player>();
		if (player != null) {
			GameObject hitObject;
			bool isGrounded = player.IsGrounded(out hitObject);

			if (isGrounded && hitObject == this.gameObject) {
				player.transform.position += Vector3.up * 0.1f;
				player.rigidbody.velocity = player.rigidbody.velocity.SetY(this.bouncyForce);
			}
		}
	}
}
