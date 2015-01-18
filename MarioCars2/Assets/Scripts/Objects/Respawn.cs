using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		Damageable d = other.GetComponent<Damageable>();
		if (d != null) {
			d.Die();
		}
	}
}
