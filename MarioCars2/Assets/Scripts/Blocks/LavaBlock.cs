using UnityEngine;
using System.Collections;

public class LavaBlock : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision c) {
		Damageable d  = c.gameObject.GetComponent<Damageable>();
		if (d != null) {
			d.Die();
		}
	}
}
