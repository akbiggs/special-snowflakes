using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : Singleton<LevelManager> {
	
	private string currentLevel;
	
	public int score;

	protected LevelManager() { }

	public void ReloadCurrentLevel() {
		Application.LoadLevel(Application.loadedLevelName);
	}

	public void GoToLevel(string level) {
		if (level.Contains("Hub"))
			MusicManager.Instance.playMenuMusic();
		else
		    MusicManager.Instance.playLevelMusic();

		Application.LoadLevel(level);
	}
}
