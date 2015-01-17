using UnityEngine;
using System.Collections;

public class Swiper : Enemy {

	public GameObject attachedBlock;
	public Vector3 movementAxis;
	public float movementSpeed;

	private float timeAtEdge = 0;

	// Use this for initialization
	void Start () {
		Vector3 topOfBlock = attachedBlock.transform.position + (Vector3.up * attachedBlock.collider.bounds.extents.y);

		this.transform.position = new Vector3(this.transform.position.x, 
		                                      topOfBlock.y + 0.5f,
		                                      this.transform.position.z);
	}

	private bool IsAtEdge() {
		Vector3 edge = (attachedBlock.collider.bounds.extents).ComponentMultiply(movementAxis);
		Vector3 distanceFromCenter = (this.transform.position - attachedBlock.transform.position).ComponentMultiply(movementAxis);

		return distanceFromCenter.magnitude > edge.magnitude;
	}

	// Update is called once per frame
	void Update () {
		if (this.IsAtEdge()) {
			this.timeAtEdge += Time.deltaTime;
			if (this.timeAtEdge >= 0.75f) {
				this.timeAtEdge = 0;
				this.movementAxis *= -1;
				this.transform.position += this.movementAxis * 0.1f;
			}
		} else {
			this.transform.position += this.movementSpeed * this.movementAxis;
		}
	}
}
