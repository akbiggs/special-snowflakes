using UnityEngine;
using System.Collections;

public class Shooter : Enemy {

	public float fireInterval = 1f;
	public float visionDistance = 1f;
	public float minShootDistance = 0f;

	public GameObject bulletPrefab;

	private float fireTimer;

	// Use this for initialization
	void Start () {
		this.fireTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 playerPos = LevelState.Instance.player.transform.position;
		Vector3 visionDirection = playerPos - this.transform.position;

		Debug.DrawRay(this.transform.position, visionDirection);

		RaycastHit hit;
		bool hitObject = false;

		if (Physics.Raycast(this.transform.position, visionDirection, out hit, this.visionDistance)) {
			if (hit.distance > this.minShootDistance && hit.collider.gameObject.GetComponent<Damageable>() != null) {
				hitObject = true;

				this.fireTimer += Time.deltaTime;
				if (this.fireTimer > this.fireInterval) {
					this.FireBullet();
					this.fireTimer = 0;
				}
			}
		}

		if (!hitObject) {
			this.fireTimer = 0;
		}
	}

	void FireBullet() {
		GameObject bulletObject = GameObject.Instantiate(this.bulletPrefab) as GameObject;

		Vector3 direction = LevelState.Instance.player.transform.position - this.transform.position;
		direction.Normalize();

		bulletObject.transform.parent = this.transform.parent;
		bulletObject.transform.position = this.transform.position + direction * 1f;

		Bullet bullet = bulletObject.GetComponent<Bullet>();
		bullet.owner = this.GetComponent<Damageable>();
		bullet.direction = direction;
	}
}