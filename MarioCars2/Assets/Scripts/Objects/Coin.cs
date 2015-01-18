using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {
	public float rotationSpeed;
	public int coinValue;
	public GameObject sparkle;

	// Use this for initialization
	void Start () {
		renderer.material.color = new Color(1f,1f,0);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(rotationSpeed, 0, 0) * Time.deltaTime);
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.GetComponent<Player>() != null) {
			GameObject obj = (GameObject)GameObject.Instantiate(this.sparkle);
			obj.transform.position = this.transform.position;
			LevelState.Instance.score += this.coinValue;
			GameObject.Destroy(this.gameObject);
		}
	}
}
