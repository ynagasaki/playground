using UnityEngine;
using System.Collections.Generic;

public class TileMap : MonoBehaviour {
	class RailNode {
		public TileRailPiece tileRail;
		public Dictionary<Vector3, RailNode> nexts;

		public RailNode(GameObject railPiece) {
			this.tileRail = railPiece.GetComponent<TileRailPiece>();
			this.nexts = new Dictionary<Vector3, RailNode>();
			Debug.Assert(this.tileRail != null);
		}
	}

	private int pieceCount = 0;
	private Dictionary<Vector3, RailNode> currentEndpoints = new Dictionary<Vector3, RailNode>();

	public bool IsEmpty {
		get {
			return pieceCount == 0;
		}
	}

	public void connect(GameObject railPiece, EndpointType? whichEnd, Vector3? targetEndpoint) {
		RailNode node = new RailNode(railPiece);

		if (IsEmpty) {
			currentEndpoints.Add(node.tileRail.StartPoint, node);
			currentEndpoints.Add(node.tileRail.EndPoint, node);
			pieceCount++;
			return;
		}

		Debug.Assert(whichEnd.HasValue && targetEndpoint.HasValue);

		Vector3 endpoint = whichEnd.Value == EndpointType.Start ? node.tileRail.StartPoint : node.tileRail.EndPoint;

		Debug.Assert(targetEndpoint.Value == endpoint);

		// reorient the new rail piece's curve's direction to match the target piece
		RailNode targetNode = currentEndpoints[targetEndpoint.Value];
		EndpointType targetEndpointType = getEndpointType(targetNode, targetEndpoint.Value);
		EndpointType newEndpointType = getEndpointType(node, endpoint);

		if (targetEndpointType == newEndpointType) {
			Debug.Log("reversing path");
			node.tileRail.reversePath();
			newEndpointType = getEndpointType(node, endpoint);
		}

		// set up the nexts (target -> new node, or new node -> target)
		setupNext(targetNode, targetEndpointType, node, endpoint);

		// see if any of the remaining endpoints need to connect to the new node's other end (i.e. completing a loop)
		Vector3 otherEnd = (newEndpointType == EndpointType.Start) ? node.tileRail.EndPoint : node.tileRail.StartPoint;
		bool closedLoop = false;

		foreach (Vector3 danglingEndpoint in this.currentEndpoints.Keys) {
			if (danglingEndpoint == otherEnd) {
				RailNode looseEndNode = currentEndpoints[danglingEndpoint];
				EndpointType danglingEndpointType = getEndpointType(looseEndNode, danglingEndpoint);

				// the loose-end-endpoint should be the opposite end-type as "other end"
				Debug.Assert(danglingEndpointType == newEndpointType);

				// link up the nexts
				setupNext(looseEndNode, danglingEndpointType, node, danglingEndpoint);

				Debug.Log("Linked up dangling loose-end node.");
				currentEndpoints.Remove(danglingEndpoint);
				closedLoop = true;

				break;
			}
		}

		// update the current endpoints...
		currentEndpoints.Remove(targetEndpoint.Value);
		if (!closedLoop) {
			currentEndpoints.Add(otherEnd, node);
		}

		Debug.Log("currentEndpoints size: " + currentEndpoints.Count);
		pieceCount++;
	}

	public bool getEndpointWithin(float sqrDist, Vector3 pos, out Vector3? foundEndpoint) {
		foundEndpoint = null;
		foreach (Vector3 endpoint in currentEndpoints.Keys) {
			if ((pos - endpoint).sqrMagnitude <= sqrDist) {
				foundEndpoint = endpoint;
				return true;
			}
		}
		return false;
	}

	private void setupNext(RailNode node1, EndpointType node1EndpointType, RailNode node2, Vector3 endpoint) {
		if (node1EndpointType == EndpointType.End) {
			node1.nexts.Add(endpoint, node2);
		} else {
			node2.nexts.Add(endpoint, node1);
		}
	}

	private EndpointType getEndpointType(RailNode node, Vector3 endpoint) {
		if (node.tileRail.EndPoint == endpoint) {
			return EndpointType.End;
		}
		Debug.Assert(node.tileRail.StartPoint == endpoint);
		return EndpointType.Start;
	}
}
