using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {
	public float rotationSpeed;
	public int coinValue;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up * Time.deltaTime, Space.Self);
	}

	void OnCollisionEnter(Collision collision) {
		if ( collision.gameObject.GetComponent<Player>() != null) {
			Destroy(this.gameObject);
			LevelManager.Instance.score += this.coinValue;
		}
	}
}
