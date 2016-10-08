using UnityEngine;
using System.Collections;

public class Train : MonoBehaviour {
	public float maxSpeed = 10f;

	private TileMap.RailNode currentRailNode;
	private float curvePos;
	private bool isRunning = false;

	private float currSpeed = 0f;
	private float targetSpeed = 0f;

	public bool IsRunning {
		get {
			return isRunning;
		}
		set {
			isRunning = value;

			if (!value) { // TODO: clean this up too, this is gross
				targetSpeed = currSpeed = 0f;
			} else {
				targetSpeed = maxSpeed;
			}
		}
	}

	public void setCurrentTile(TileMap.RailNode tile) {
		currentRailNode = tile;

		// TODO: clean this up, make this part of each tile or something; also, this
		// should be more proactive
		if (currentRailNode.tileRail.name.Contains("corner")) {
			targetSpeed = maxSpeed * 0.2f;
		} else if (currentRailNode.tileRail.name.Contains("straight")) {
			targetSpeed = maxSpeed;
		}
	}

	public void setCurvePos(float t) {
		if (currentRailNode == null) {
			return;
		}

		BezierCurve curve = currentRailNode.tileRail.GetComponentInChildren<BezierCurve>(); // move this to tile rail piece
		if (curve == null) {
			return;
		}

		this.transform.position = curve.GetPoint(t);
		this.transform.forward = curve.GetDirection(t);
		this.curvePos = t;
	}

	void Update() {
		if (!IsRunning || this.currentRailNode == null) {
			return;
		}

		this.curvePos += Time.deltaTime * currSpeed;

		if (currSpeed < targetSpeed) {
			this.currSpeed = Mathf.Min(currSpeed + Time.deltaTime, targetSpeed);
		} else if (currSpeed > targetSpeed) {
			this.currSpeed = Mathf.Max(currSpeed - Time.deltaTime * 5f, targetSpeed);
		}

		if (this.curvePos > 1f) { // TODO: fix this
			var tile = currentRailNode.getNext(currentRailNode.tileRail.EndPoint);
			Debug.Assert(tile != null);
			setCurrentTile(tile);
			this.curvePos -= 1f;
		}

		this.setCurvePos(this.curvePos);
	}
}
