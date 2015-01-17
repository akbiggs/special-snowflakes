using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {
	public float rotationSpeed;
	public int coinValue;

	// Use this for initialization
	void Start () {
		renderer.material.color = new Color(0.5f,0,0);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(rotationSpeed, 0, 0) * Time.deltaTime);
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.GetComponent<Player>() != null) {
			Destroy(this.gameObject);
			LevelState.Instance.score += this.coinValue;
		}
	}
}
