using UnityEngine;
using System.Collections;

public class Train : MonoBehaviour {
	public float maxSpeed = 10f;

	private TileMap tileMap;
	private GameObject currentTile;
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

	void Start() {
		GameObject tileMapObject = GameObject.Find("TileMap");
		Debug.Assert(tileMapObject != null);
		tileMap = tileMapObject.GetComponent<TileMap>();
		Debug.Assert(tileMap != null);
	}

	public void setCurrentTile(GameObject tile) {
		currentTile = tile;

		// TODO: clean this up, make this part of each tile or something; also, this
		// should be more proactive
		if (currentTile.name.Contains("corner")) {
			targetSpeed = maxSpeed * 0.2f;
		} else if (currentTile.name.Contains("straight")) {
			targetSpeed = maxSpeed;
		}
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
		if (!IsRunning || this.currentTile == null) {
			return;
		}

		this.curvePos += Time.deltaTime * currSpeed;

		if (currSpeed < targetSpeed) {
			this.currSpeed = Mathf.Min(currSpeed + Time.deltaTime, targetSpeed);
		} else if (currSpeed > targetSpeed) {
			this.currSpeed = Mathf.Max(currSpeed - Time.deltaTime * 5f, targetSpeed);
		}

		if (this.curvePos > 1f) { // TODO: fix this
			IsRunning = false;
			this.curvePos -= 1f;
		}

		this.setCurvePos(this.curvePos);
	}
}
