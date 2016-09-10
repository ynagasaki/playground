﻿using UnityEngine;
using System.Collections.Generic;

public class RailCurveRenderer : MonoBehaviour {
	public float radius = 0.5f;
	public int vertexCount = 10;

	void Start () {
		float PI_HALF = Mathf.PI * 0.5f;
		float STEP = PI_HALF / (float) vertexCount;
		List<Vector3> positionsList = new List<Vector3>();

		for (float theta = 0f; theta <= PI_HALF; theta += STEP) {
			positionsList.Add(calcCirclePos(theta));
		}

		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetVertexCount(positionsList.Count);
		lineRenderer.SetPositions(positionsList.ToArray());
		lineRenderer.SetWidth(0.1f, 0.1f);
	}

	Vector3 calcCirclePos(float theta) {
		return new Vector3(Mathf.Cos(theta) * radius - 0.5f, 0f, Mathf.Sin(theta) * radius - 0.5f);
	}
}
