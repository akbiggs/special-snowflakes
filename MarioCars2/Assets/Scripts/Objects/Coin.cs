using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {
	public float rotationSpeed;
	public int coinValue;
	public GameObject sparkle;
	
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
