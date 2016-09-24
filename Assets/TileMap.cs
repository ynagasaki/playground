using UnityEngine;
using System.Collections.Generic;

public class TileMap : MonoBehaviour {
	public GridLines gridLines;

	private readonly Dictionary<Vector2, GameObject> posToRailMap = new Dictionary<Vector2, GameObject>();
	private readonly Dictionary<Vector2, GameObject> posToTrainMap = new Dictionary<Vector2, GameObject>();

	private Vector2 toMapKey(Vector3 v) {
		return new Vector2(v.x, v.z);
	}

	public bool railExists(Vector3 pos) {
		return posToRailMap.ContainsKey(toMapKey(pos));
	}

	public bool trainExists(Vector3 pos) {
		return posToTrainMap.ContainsKey(toMapKey(pos));
	}

	public GameObject getRail(Vector3 pos) {
		return posToRailMap[toMapKey(pos)];
	}

	public GameObject getTrain(Vector3 pos) {
		return posToTrainMap[toMapKey(pos)];
	}

	public void putRail(Vector3 pos, GameObject rail) {
		posToRailMap[toMapKey(pos)] = rail;
	}

	public void putTrain(Vector3 pos, GameObject train) {
		posToTrainMap[toMapKey(pos)] = train;
	}

	public GameObject removeRail(Vector3 pos) {
		GameObject piece;
		if (!posToRailMap.TryGetValue(toMapKey(pos), out piece)) {
			return null;
		}
		posToRailMap.Remove(toMapKey(pos));
		return piece;
	}

	public GameObject removeTrain(Vector3 pos) {
		GameObject piece;
		if (!posToTrainMap.TryGetValue(toMapKey(pos), out piece)) {
			return null;
		}
		posToTrainMap.Remove(toMapKey(pos));
		return piece;
	}

	public bool buildTrack() {
		int count = posToRailMap.Count;

		if (count == 0) {
			return false;
		}

		float gridLineSpacing20Pct = gridLines.gridlineSpacing() * 0.2f;

		// pick a tile, pick last endpoint
		Dictionary<Vector2, GameObject>.Enumerator iter = posToRailMap.GetEnumerator();
		iter.MoveNext();
		GameObject tile = iter.Current.Value;
		BezierCurve curve = tile.GetComponentInChildren<BezierCurve>();
		GameObject startTile = tile;

		do {
			Vector3 endpoint = curve.points[curve.points.Length - 1];

			// figure out the world space of the endpoint
			Vector3 endpointWorldSpace = tile.transform.TransformPoint(endpoint);

			// figure out if that leads to another tile
			Vector3 slightlyOverTileBoundary = endpointWorldSpace + curve.GetDirection(1f) * gridLineSpacing20Pct;
			Vector3 expectedNeighborTilePos = gridLines.closestTilePosition(slightlyOverTileBoundary);

			// if not, return false
			if (!posToRailMap.ContainsKey(toMapKey(expectedNeighborTilePos))) {
				return false;
			}

			// if so, [connect the pieces and] figure out if endpoints need to be swapped
			GameObject neighborTile = posToRailMap[toMapKey(expectedNeighborTilePos)];
			BezierCurve neighborCurve = neighborTile.GetComponentInChildren<BezierCurve>();
			Vector3 neighborEndpoint = neighborCurve.points[neighborCurve.points.Length - 1];
			Vector3 neighborEndpointWorldSpace = neighborTile.transform.TransformPoint(neighborEndpoint);

			if (Mathf.Abs((endpointWorldSpace - neighborEndpointWorldSpace).magnitude) < gridLineSpacing20Pct) {
				neighborCurve.reversePoints();
			}

			tile = neighborTile;
			curve = neighborCurve;
			count --;

			// keep going; stop until loop is complete
		} while (tile != startTile && count > 0);

		// [figure out if we've hit all the tiles. if not, just remove the ones that we didn't hit i guess]
		return tile == startTile;
	}

	public GameObject getNextRail(Transform targetTransform) {
		// determine the next rail piece by "jumping forward" a bit and seeing what tile that position corresponds to
		Vector3 jumpedPos = targetTransform.position + targetTransform.forward * 0.1f;
		Vector3 closestTilePos = this.gridLines.closestTilePosition(jumpedPos);
		GameObject result = null;
		this.posToRailMap.TryGetValue(toMapKey(closestTilePos), out result);
		return result;
	}
}
