using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Damageable))]
public class Player : MonoBehaviour {

	const float FADE_IN_TIME = 0.4f;
	const float DEATH_TIME = 0.8f;
	const float SHAKE_TIME = 0.15f;
	const float SHAKE_POWER = 0.2f;
	const float FLASH_TIME = 0.03f;
	const bool SHAKE_FLASH = false;

	public float maxVelocity = 5;
	public float movementForce = 10;
	public float midairForce = 5;
	public float turnSpeed = 4;
	public float minTurnForce = 0.1f;

	public float jumpPower = 10;
	public float jumpRayLength = 1;
	public float jumpReleaseMax = 0;

	public Animator animator;
	public GameObject rendererObject;
	public GameObject deathParticlesPrefab;
	public Texture whiteTexture;

	public AudioClip jumpSound;

	private Damageable damagable;
	private float prevFireAxis = 0;

	private bool flash = false;
	private bool deathShakeFinished = false;
	private Vector3 deathCameraPosition;
	private float deathTime;

	void Start () {
		this.damagable = this.GetComponent<Damageable>();
		this.damagable.OnDeath += OnDeath;
	}
	
	void Update () {
		if (this.damagable.IsDead) {
			this.DuringDeath();
		}

		// Get the movement directon from the axis.
		Vector3 forward = Camera.main.transform.forward;
		Vector3 sideways = Camera.main.transform.right;
		forward.y = 0;
		sideways.y = 0;
		forward = forward.normalized;
		sideways = sideways.normalized;
		Vector3 direction = Input.GetAxis("Horizontal") * sideways + Input.GetAxis("Vertical") * forward;

		// Find the force that we need to add the the player.
		Vector3 desiredVelocity = direction * maxVelocity;
		Vector3 currentVelocity = this.rigidbody.velocity.SetY(0);
		Vector3 diff = desiredVelocity - currentVelocity;
		float force = this.IsGrounded() ? this.movementForce : this.midairForce; 
		this.rigidbody.AddForce(diff * force);

		// Allow jumping.
		if (Input.GetAxis("Fire1") > 0) {
			if (this.IsGrounded()) {
				//Start the jump.
				AudioSource.PlayClipAtPoint(this.jumpSound, this.transform.position);

				this.rigidbody.velocity = this.rigidbody.velocity.SetY(this.jumpPower);
			}
		} else if (this.prevFireAxis > 0 && this.rigidbody.velocity.y > this.jumpReleaseMax) {
			this.rigidbody.velocity = this.rigidbody.velocity.SetY(this.jumpReleaseMax);
		}

		this.prevFireAxis = Input.GetAxis("Fire1");

		// Rotate the player to face the direction it is moving.
		this.animator.SetFloat("Speed", direction.magnitude);
		if (direction.magnitude > this.minTurnForce) {
			this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(direction), this.turnSpeed * Time.deltaTime);
		}

		// Edge case to ensure the player dies.
		if (this.transform.position.y < -200) {
			this.GetComponent<Damageable>().Die();
		}
	}

	void OnDeath() {
		Time.timeScale = 0.00f;
		this.deathTime = Time.realtimeSinceStartup;
		this.deathCameraPosition = Camera.main.transform.position;
	}

	private void DuringDeath() {
		Camera.main.transform.position = this.deathCameraPosition;
		if (Time.realtimeSinceStartup - this.deathTime < SHAKE_TIME) {
			Camera.main.transform.position += Random.onUnitSphere * SHAKE_POWER;
		} else if (!this.deathShakeFinished) {
			this.deathShakeFinished = true;

			Time.timeScale = 1;
			this.rendererObject.SetActive(false);

			GameObject obj = (GameObject)GameObject.Instantiate(this.deathParticlesPrefab);
			obj.transform.position = this.transform.position;
		}
		if (Time.realtimeSinceStartup - this.deathTime > DEATH_TIME) {
			this.OnDeathFinish();
		}
	}

	void OnDeathFinish() {
		LevelManager.Instance.ReloadCurrentLevel();
	}

	void OnGUI() {
		// Death flash.
		float timeSinceDeath = Time.realtimeSinceStartup - this.deathTime;
		if (SHAKE_FLASH && this.deathShakeFinished && timeSinceDeath > SHAKE_TIME && timeSinceDeath < FLASH_TIME + SHAKE_TIME) {
			GUI.color = Color.white * 0.85f;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.whiteTexture);
		}

		// Level fade in.
		if (Time.timeSinceLevelLoad < FADE_IN_TIME) {
			float fade = 1 - Time.timeSinceLevelLoad / FADE_IN_TIME;
			GUI.color = Color.white * fade;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.whiteTexture);
		}
	}

	public bool IsGrounded() {
		GameObject thingBeingStoodOn;
		return this.IsGrounded(out thingBeingStoodOn);
	}

	public bool IsGrounded(out GameObject thingBeingStoodOn) {
		RaycastHit hit;
		bool result = Physics.Raycast(new Ray(this.transform.position, Vector3.down), out hit, this.jumpRayLength);
		if (result) {
			thingBeingStoodOn = hit.collider.gameObject;
		} else {
			thingBeingStoodOn = null;
		}

		return result;
	}
}
