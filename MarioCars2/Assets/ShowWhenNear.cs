using UnityEngine;
using System.Collections;

public class ShowWhenNear : MonoBehaviour {
	public float distance;
	public GameObject checkObject;
	public float showSpeed = 0.5f;

	private Vector3 showPosition;
	private Vector3 hidePosition;
	private bool showing = false;

	void Start() {
		this.showPosition = this.transform.position;
		this.transform.position = this.transform.FindChild("HideAnchor").position;
		this.hidePosition = this.transform.position;
	}

	void Update() {
		if (this.checkObject == null) {
			return;
		}

		if (!showing && Vector3.Distance(this.showPosition, checkObject.transform.position) <= this.distance) {
			this.showing = true;
		} else if (showing) {
			this.transform.position = Vector3.MoveTowards(this.transform.position, this.showPosition,
			                                              Vector3.Distance(this.transform.position, this.showPosition) / Vector3.Distance(this.hidePosition, this.showPosition));
		}
	}
}
