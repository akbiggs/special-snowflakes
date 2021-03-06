﻿using UnityEngine;
using System.Collections;
using System;

public class Damageable : MonoBehaviour {

	public int health = 1;
	public bool deadalrady = false;

	public event Action OnDeath;
	public bool IsDead { get { return this.health <= 0; } }

	private bool hasDied = false;

	public void Damage(int damage) {
		this.health -= damage;
		if (this.IsDead && !this.hasDied) {
			this.hasDied = true;
			this.Die();
		}
	}

	public void Die() {
		if (this.deadalrady)
			return;

		this.deadalrady = true;
		this.health = 0;
		if (this.OnDeath != null) {
			this.OnDeath();
		} else {
			GameObject.Destroy(this.gameObject);
		}
	}
}
