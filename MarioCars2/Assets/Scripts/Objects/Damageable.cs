using UnityEngine;
using System.Collections;
using System;

public class Damageable : MonoBehaviour {

	public int health = 1;

	public event Action OnDeath;
	public bool IsDead { get { return this.health <= 0; } }

	public void Damage(int damage) {
		this.health -= damage;
		if (this.IsDead) {
			this.Die();
		}
	}

	public void Die() {
		this.health = 0;
		if (this.OnDeath != null) {
			this.OnDeath();
		} else {
			GameObject.Destroy(this.gameObject);
		}
	}
}
