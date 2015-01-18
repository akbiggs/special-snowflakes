using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {

	public Animator animator;
	bool activated = false;

	public string level;

	public bool showAnimation = true;

	void OnCollisionEnter(Collision collision) {
		Player player = collision.gameObject.GetComponent<Player>();
		if (player != null && !this.activated) {
			if (!this.showAnimation) {
				OnAnimationFinish();
				return;
			}

			this.GetComponent<AudioSource>().Play();

			this.activated = true;
			this.animator.SetBool("On", true);

			player.gameObject.SetActive(false);
			Camera.main.gameObject.SetActive(false);	
		}
	}

	void OnAnimationFinish() {
		LevelManager.Instance.GoToLevel(this.level);
	}
}
