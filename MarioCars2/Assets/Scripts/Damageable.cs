using UnityEngine;
using System.Collections;
using System;

public class Damageable : MonoBehaviour {

	int health = 1;

	public event Action OnDeath;

	public void Damage(int damage) {
		this.health -= damage;
		if (this.health <= 0) {
			this.Die();
		}
	}

	public void Die() {
		if (this.OnDeath != null) {
			this.OnDeath();
		}
	}
}
