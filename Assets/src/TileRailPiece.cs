using UnityEngine;
using System.Collections;

public class TileRailPiece : MonoBehaviour {
	private BezierCurve curve;

	void Start() {
		this.curve = GetComponentInChildren<BezierCurve>();
		Debug.Assert(this.curve != null);
	}

	public Vector3 StartPoint {
		get {
			return this.transform.TransformPoint(curve.points[0]);
		}
	}

	public Vector3 EndPoint {
		get {
			return this.transform.TransformPoint(curve.points[curve.points.Length - 1]);
		}
	}

	public Vector3 getDirection(float t) {
		return this.curve.GetDirection(t);
	}

	public void reversePath() {
		this.curve.reversePoints();
	}
}
