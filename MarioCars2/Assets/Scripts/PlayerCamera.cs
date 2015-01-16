using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

	public GameObject following;

	void Update () {
		this.transform.LookAt(this.following.transform);
	}
}
