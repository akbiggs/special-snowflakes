using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	public Vector3 direction;
	public float moveSpeed = 0.075f;

	public GameObject particleEffect;
	public GameObject renderer;
	public Damageable owner;
	public int damage = 1;
	public float time = 2;

	void Start() {
		this.transform.rotation = Quaternion.LookRotation(this.direction);

		GameObject obj = (GameObject)GameObject.Instantiate(particleEffect);
		obj.transform.position = this.transform.position;
	}

	void OnCollisionEnter(Collision other) {
		Damageable damageableObject = other.gameObject.GetComponent<Damageable>();

		if (damageableObject != null) {
			if (damageableObject.gameObject == this.owner.gameObject) {
				return;
			}

			damageableObject.Damage(this.damage);
		}

		this.OnDeath();
	}

	void OnDeath() {
		GameObject obj = (GameObject)GameObject.Instantiate(particleEffect);
		obj.transform.position = this.transform.position;
		GameObject.Destroy(this.gameObject);
	}

	void FixedUpdate() {
		this.renderer.transform.Rotate(new Vector3(1, 0, 0));
		this.transform.position += this.direction * this.moveSpeed;

		this.time -= Time.fixedDeltaTime;
		if (this.time <= 0) {
			this.OnDeath();
		}
	}
}
