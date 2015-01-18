using UnityEngine;
using System.Collections;

public class InputText : MonoBehaviour {
	public string JoypadText;
	public string KeyboardText;

	void Start() {
		string[] names = Input.GetJoystickNames();
		if (names.Length == 0) {
			this.GetComponent<TextMesh>().text = this.KeyboardText;
		} else {
			this.GetComponent<TextMesh>().text = this.JoypadText;
		}
	}
}
