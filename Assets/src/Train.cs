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
			targetSpeed = maxSpeed * 0.5f;
		} else if (currentRailNode.tileRail.name.Contains("straight")) {
			targetSpeed = maxSpeed;
		}
	}

	public void setCurvePos(float t) {
		if (currentRailNode == null) {
			return;
		}

		this.curvePos = t;
		this.transform.position = currentRailNode.tileRail.getPosition(t);
		this.transform.forward = currentRailNode.tileRail.getDirection(t);
	}

	void Update() {
		if (!IsRunning || this.currentRailNode == null) {
			return;
		}

		// curve pos (=t) is the ratio of distance traveled to length of the rail's entire b-curve
		float distanceTraveled = Time.deltaTime * currSpeed;
		this.curvePos += distanceTraveled / currentRailNode.tileRail.getApproxLength();

		if (currSpeed < targetSpeed) {
			this.currSpeed = Mathf.Min(currSpeed + Time.deltaTime, targetSpeed);
		} else if (currSpeed > targetSpeed) {
			this.currSpeed = Mathf.Max(currSpeed - Time.deltaTime * 5f, targetSpeed);
		}

		if (this.curvePos > 1f) { // TODO: fix this for when train goes so fast it should skip a rail piece
			var tile = currentRailNode.getNext(currentRailNode.tileRail.EndPoint);
			Debug.Assert(tile != null);
			setCurrentTile(tile);
			this.curvePos -= 1f;
		}

		this.setCurvePos(this.curvePos);
	}
}
