using UnityEngine;
using System.Collections;

// TODO: make this a unity singleton
public class MusicManager : MonoBehaviour {
	private static MusicManager instance = null;
	
	private AudioSource audioLevel;
	private AudioSource audioMenu;

	public AudioSource startingMusic;

	public AudioClip deathSound;

	public static MusicManager Instance {
		get { return instance; }
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			if (startingMusic != null) {
				startingMusic.Play();
			}
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () {
		AudioSource[] audioSources = GetComponents<AudioSource>();
		this.audioLevel = audioSources[0];
		this.audioMenu = audioSources[1];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void playMenuMusic() {
		if (!this.audioMenu.isPlaying) {
			this.audioLevel.Stop();
			this.audioMenu.Play();
		}
	}

	public void playLevelMusic() {
		if (!this.audioLevel.isPlaying) {
			this.audioMenu.Stop();
			this.audioLevel.Play();
		}
	}

	public void PlayDeathSound() {
		AudioSource.PlayClipAtPoint(this.deathSound, Vector3.zero);
	}
}


