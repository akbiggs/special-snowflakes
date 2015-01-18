using UnityEngine;
using System.Collections;

public class ParticleSystemDestroyer : MonoBehaviour {
	void LateUpdate() {
		if (!this.particleSystem.IsAlive()) {
			GameObject.Destroy(this.gameObject);
		}
	}
}
