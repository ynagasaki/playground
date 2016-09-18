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

	public GameObject getNextRail(Transform targetTransform) {
		// determine the next rail piece by "jumping forward" a bit and seeing what tile that position corresponds to
		Vector3 jumpedPos = targetTransform.position + targetTransform.forward * 0.1f;
		Vector3 closestTilePos = this.gridLines.closestTilePosition(jumpedPos);
		GameObject result = null;
		this.posToRailMap.TryGetValue(toMapKey(closestTilePos), out result);
		return result;
	}
}
