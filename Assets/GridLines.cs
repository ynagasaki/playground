using UnityEngine;

public class GridLines : MonoBehaviour {
	public int lineCount = 100;
	public Material gridLineMaterial = null;

	void Start() {
		int lineCount2 = lineCount * 2;

		LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.SetWidth(0.1f, 0.1f);
		lineRenderer.SetVertexCount(lineCount2);
		lineRenderer.useWorldSpace = false;
		lineRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
		lineRenderer.receiveShadows = false;
		lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

		if (gridLineMaterial != null) {
			lineRenderer.material = gridLineMaterial;
		}

		Vector3[] points = new Vector3[lineCount2];
		int start = -lineCount;
		float lineSpacing = this.gridlineSpacing();
		float z = start / 4 + lineSpacing * 0.5f;
		bool altLine = false;
		int i = 0;
		for (; i < lineCount - 1; i += 2) {
			points[i]     = new Vector3(altLine ? lineCount : start, 0f, z);
			points[i + 1] = new Vector3(altLine ? start : lineCount, 0f, z);
			altLine = !altLine;
			z += lineSpacing;
		}
		for (; i < lineCount2 - 1; i += 2) {
			points[i]     = new Vector3(z, 0f, altLine ? lineCount : start);
			points[i + 1] = new Vector3(z, 0f, altLine ? start : lineCount);
			altLine = !altLine;
			z -= lineSpacing;
		}
		lineRenderer.SetPositions(points);
	}

	public float gridlineSpacing() {
		return 1f;
	}

	public Vector3 closestTilePosition(Vector3 pos) {
		// This is assuming grid line spacing == 1f, not sure about the calcs if this is not true
		return new Vector3(Mathf.Round(pos.x), pos.y, Mathf.Round(pos.z));
	}
}
