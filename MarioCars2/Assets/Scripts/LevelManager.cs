using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : Singleton<LevelManager> {

	private string currentLevel;
	private string[] remainingLevels = { "GoalScene", "RandomScene", "CoinScene" };

	public int score;

	public Player player;

	protected LevelManager() { }

	public void NextLevel() {
		Application.LoadLevel(remainingLevels.Pop());
	}	
}
