using UnityEngine;
using System.Collections.Generic;

public class TileMap : MonoBehaviour {
	class RailNode {
		public TileRailPiece tileRail;

		public RailNode(GameObject railPiece) {
			this.tileRail = railPiece.GetComponent<TileRailPiece>();
			Debug.Assert(this.tileRail != null);
		}
	}

	int pieceCount = 0;
	Stack<RailNode> currentEndpieces = new Stack<RailNode>();
	Dictionary<Vector3, GameObject> currentEndpoints = new Dictionary<Vector3, GameObject>();

	public bool IsEmpty {
		get {
			return pieceCount == 0;
		}
	}

	public void connect(GameObject railPiece, Vector3? endpoint, GameObject targetPiece, Vector3? targetEndpoint) {
		if (IsEmpty) {
			RailNode node = new RailNode(railPiece);
			currentEndpieces.Push(node);
			currentEndpoints.Add(node.tileRail.StartPoint, railPiece);
			currentEndpoints.Add(node.tileRail.EndPoint, railPiece);
			pieceCount++;
			return;
		}

		Debug.Assert(endpoint.HasValue && targetEndpoint.HasValue && targetPiece != null);
		Debug.Log("NEED TO DO!!");
	}

	public bool getEndpointWithin(float sqrDist, Vector3 pos, out GameObject foundRailPiece, out Vector3? foundEndpoint) {
		foundRailPiece = null;
		foundEndpoint = null;
		foreach (Vector3 endpoint in currentEndpoints.Keys) {
			if ((pos - endpoint).sqrMagnitude <= sqrDist) {
				foundEndpoint = endpoint;
				foundRailPiece = currentEndpoints[endpoint];
				return true;
			}
		}
		return false;
	}
}
