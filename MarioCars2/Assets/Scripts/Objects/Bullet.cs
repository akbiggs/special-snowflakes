using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	public Vector3 direction;
	public float moveSpeed = 0.075f;

	public Damageable owner;
	public int damage = 1;

	void OnCollisionEnter(Collision other) {
		Damageable damageableObject = other.gameObject.GetComponent<Damageable>();

		if (damageableObject != null) {
			if (damageableObject.gameObject == this.owner.gameObject) {
				return;
			}

			damageableObject.Damage(this.damage);
		}

		Debug.Log("Killed " + other.gameObject.name);

		GameObject.Destroy(this.gameObject);
	}

	public void Update() {
//		this.rigidbody.velocity = this.direction * this.moveSpeed;
		this.transform.position += this.direction * this.moveSpeed;
	}
}
