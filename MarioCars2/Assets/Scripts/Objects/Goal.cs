using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {

	public Animator animator;
	bool activated = false;
	
	void OnCollisionEnter(Collision collision) {
		Player player = collision.gameObject.GetComponent<Player>();
		if (player != null && !this.activated) {
			this.GetComponent<AudioSource>().Play();

			this.activated = true;
			this.animator.SetBool("On", true);

			player.gameObject.SetActive(false);
			Camera.main.gameObject.SetActive(false);	
		}
	}

	void OnAnimationFinish() {
		LevelManager.Instance.NextLevel();
	}
}
