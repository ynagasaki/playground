using UnityEngine;
using System.Collections;

public class Train : MonoBehaviour {
	private GameObject currentTile;
	private float curvePos;

	public void setCurrentTile(GameObject tile) {
		currentTile = tile;
	}

	public void setCurvePos(float t) {
		if (currentTile == null) {
			return;
		}

		BezierCurve curve = currentTile.GetComponentInChildren<BezierCurve>();
		if (curve == null) {
			return;
		}

		this.transform.position = curve.GetPoint(t);
		this.transform.forward = curve.GetDirection(t);
		this.curvePos = t;
	}

	void Update() {
		this.curvePos += Time.deltaTime;
		if (this.curvePos > 1f) {
			this.curvePos = 1f - this.curvePos;
		}
		this.setCurvePos(this.curvePos);
	}
}
