using UnityEngine;
using System.Collections;

public class LevelState : MonoBehaviour {

	public static LevelState Instance;

	public GameObject respawnPoint;
	public Player player;
	public int score;

	void Awake() {
		Instance = this;
	}

	void OnDestroy() {
		Instance = null;
	}

	void Start () {
	
	}
	
	void Update () {
	
	}
}
