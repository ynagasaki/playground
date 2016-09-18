using UnityEngine;
using System.Collections;

public class Train : MonoBehaviour {
	public float kmph = 10f;

	private TileMap tileMap;
	private GameObject currentTile;
	private float curvePos;

	void Start() {
		GameObject tileMapObject = GameObject.Find("TileMap");
		Debug.Assert(tileMapObject != null);
		tileMap = tileMapObject.GetComponent<TileMap>();
		Debug.Assert(tileMap != null);
	}

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
		if (this.currentTile == null) {
			return;
		}

		this.curvePos += Time.deltaTime * kmph * 0.2778f;

		if (this.curvePos > 1f) {
			this.setCurrentTile(this.tileMap.getNextRail(this.transform));
			this.curvePos = 1f - this.curvePos;
		}

		this.setCurvePos(this.curvePos);
	}
}
