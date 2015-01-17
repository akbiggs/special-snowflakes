using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {

	void OnCollisionEnter(Collision collision) {
		Player player = collision.gameObject.GetComponent<Player>();
		if (player != null) {
			LevelManager.Instance.NextLevel();
		}
	}
}
