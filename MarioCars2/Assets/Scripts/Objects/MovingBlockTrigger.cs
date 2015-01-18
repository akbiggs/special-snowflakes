using UnityEngine;
using System.Collections;

public class MovingBlockTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerExit(Collider c) {
		MovingBlock collidedBlock = c.gameObject.GetComponent<MovingBlock>();
		if (collidedBlock != null) {
			collidedBlock.ChangeDirection();
		}
	}

	void OnCollisionEnter(Collision c) {
		MovingBlock collidedBlock = c.gameObject.GetComponent<MovingBlock>();
		if (collidedBlock != null) {
			collidedBlock.ChangeDirection();
		}
	}


}
