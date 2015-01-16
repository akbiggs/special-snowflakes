using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : Singleton<LevelManager> {

	private string currentLevel;
	private string[] remainingLevels = { "GoalScene", "RandomScene" };

	protected LevelManager() { }

	public void NextLevel() {
		Application.LoadLevel(remainingLevels.Pop());
	}
}
