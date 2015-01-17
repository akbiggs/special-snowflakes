using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	void OnCollisionEnter(Collision collision) {
		Player player = collision.gameObject.GetComponent<Player>();
		if (player != null) {
			Damageable damageable = player.GetComponent<Damageable>();
			damageable.Damage(1);
		}
	}
}
