using UnityEngine;
using System.Collections.Generic;

public class TileMap : MonoBehaviour {
	public class RailNode {
		public TileRailPiece tileRail;
		public Dictionary<Vector3, RailNode> nexts;

		public RailNode(GameObject railPiece) {
			this.tileRail = railPiece.GetComponent<TileRailPiece>();
			this.nexts = new Dictionary<Vector3, RailNode>();
			Debug.Assert(this.tileRail != null);
		}

		public RailNode getNext(Vector3 v) {
			foreach (Vector3 k in nexts.Keys) {
				if (v == k) {
					return this.nexts[k];
				}
			}
			return null;
		}
	}

	private RailNode head = null;
	private Dictionary<Vector3, RailNode> currentEndpoints = new Dictionary<Vector3, RailNode>();

	public bool IsEmpty {
		get {
			return head == null;
		}
	}

	public RailNode StartNode {
		get {
			return head;
		}
	}

	public bool IsReady {
		get {
			return currentEndpoints.Count == 0 && !IsEmpty;
		}
	}

	public bool connect(GameObject railPiece, EndpointType? whichEnd, Vector3? targetEndpoint) {
		RailNode node = new RailNode(railPiece);

		if (IsEmpty) {
			currentEndpoints.Add(node.tileRail.StartPoint, node);
			currentEndpoints.Add(node.tileRail.EndPoint, node);
			head = node;
			return true;
		}

		Debug.Assert(whichEnd.HasValue && targetEndpoint.HasValue);

		string debugMessage = "";
		Vector3 endpoint = whichEnd.Value == EndpointType.Start ? node.tileRail.StartPoint : node.tileRail.EndPoint;

		// make sure it has an appropriate rotation
		Vector3 dir = node.tileRail.getDirection(whichEnd.Value == EndpointType.Start ? 0f : 1f);

		// if the angle between the two direction vectors is > 5 deg or < 175 deg then error
		TileRailPiece targetTile = currentEndpoints[targetEndpoint.Value].tileRail;
		Vector3 targetDir = targetTile.getDirection(targetTile.StartPoint == targetEndpoint.Value ? 0f : 1f);
		float angle = Mathf.Acos(Vector3.Dot(targetDir, dir));\
		if (angle > Mathf.Deg2Rad * 3f && angle < Mathf.Deg2Rad * 175f) {
			Debug.Log("Not placing piece, because not continuous.");
			return false;
		}

		Debug.Assert(targetEndpoint.Value == endpoint);

		// reorient the new rail piece's curve's direction to match the target piece
		RailNode targetNode = currentEndpoints[targetEndpoint.Value];
		EndpointType targetEndpointType = getEndpointType(targetNode, targetEndpoint.Value);
		EndpointType newEndpointType = getEndpointType(node, endpoint);

		if (targetEndpointType == newEndpointType) {
			debugMessage += "reversing path;";
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

				debugMessage += "Linked up dangling loose-end node;";
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

		Debug.Log(debugMessage + "currentEndpoints size: " + currentEndpoints.Count);
		return true;
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
