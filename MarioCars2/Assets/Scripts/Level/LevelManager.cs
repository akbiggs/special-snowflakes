using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : Singleton<LevelManager> {

	private string currentLevel;

	private List<string> remainingLevels = new List<string>() { "GoalScene", "GoalScene" };
	public int score;

	protected LevelManager() { }

	public void ReloadCurrentLevel() {
		Application.LoadLevel(Application.loadedLevelName);
	}

	public bool NextLevel() {
		if (this.remainingLevels.Count > 0) {
			Application.LoadLevel(remainingLevels.Pop());
			return true;
		}

		return false;
	}
}
