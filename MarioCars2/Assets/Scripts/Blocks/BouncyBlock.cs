using UnityEngine;
using System.Collections;

public class BouncyBlock : MonoBehaviour {
	public float bouncyForce;

	void OnCollisionEnter(Collision c) {
		Debug.Log ("Collision");
		Player player = c.gameObject.GetComponent<Player>();
		if (player != null) {
			if ((player.transform.position.y - player.rigidbody.collider.bounds.extents.y / 2) 
			    - (this.transform.position.y + 1f) > 0) {
				
//			}

//			if (isGrounded && hitObject == this.gameObject) {
				player.transform.position += Vector3.up * 0.1f;
				player.rigidbody.velocity = player.rigidbody.velocity.SetY(this.bouncyForce);
				player.bounceTime = Time.time;
			}
		}
	}
}
