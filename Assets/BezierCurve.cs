using UnityEngine;
using System.Collections.Generic;

/**
 * http://catlikecoding.com/unity/tutorials/curves-and-splines/
 */
public class BezierCurve : MonoBehaviour {
	public Vector3[] points;
	public Vector3 offset = Vector3.zero;

	public BezierCurve() {
		Reset();
	}

	void Start() {
		for (int i = 0; i < points.Length; i ++) {
			points[i] = points[i] + offset;
		}

		List<Vector3> positionsList = new List<Vector3>();
		const float step = 0.1f;

		for (float t = 0f; t <= 1.0001f; t += step) {
			Vector3 pt = Bezier.GetPoint(points[0], points[1], points[2], points[3], t);
			positionsList.Add(pt);
		}

		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetVertexCount(positionsList.Count);
		lineRenderer.SetPositions(positionsList.ToArray());
	}

	public void Reset() {
		points = new Vector3[] {
			new Vector3(-2f, 0f, 0f) + offset,
			new Vector3(-1f, 0f, 0f) + offset,
			new Vector3(1f, 0f, 0f) + offset,
			new Vector3(2f, 0f, 0f) + offset
		};
	}

	public Vector3 GetPoint(float t) {
		return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], points[3], t));
	}

	public Vector3 GetVelocity(float t) {
		return transform.TransformPoint(
			Bezier.GetFirstDerivative(points[0], points[1], points[2], points[3], t)) - transform.position;
	}

	public Vector3 GetDirection(float t) {
		return GetVelocity(t).normalized;
	}
}