using UnityEngine;
using System.Collections;

public class PropelPlayer : MonoBehaviour {

	public Vector3 amount;

	public void OnTriggerEnter(Collider other) {
		Player player = other.gameObject.GetComponent<Player>();
		if (player != null) {
			player.gameObject.rigidbody.velocity = amount;
		}
	}
}
