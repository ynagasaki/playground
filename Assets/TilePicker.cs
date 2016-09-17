using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TilePicker : MonoBehaviour {
	public Camera piecePreviewCamera;
	public GameObject gridLinesObject;
	public GameObject markerObject;
	public TileData tileData;

	private GameObject pieceToSet = null;

	private readonly Dictionary<Vector3, GameObject> posToRailMap = new Dictionary<Vector3, GameObject>();
	private readonly Dictionary<Vector3, GameObject> posToTrainMap = new Dictionary<Vector3, GameObject>();

	void Update() {
		// https://docs.unity3d.com/ScriptReference/EventSystems.EventSystem.IsPointerOverGameObject.html
		if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject()) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3? intersectionPoint = findGridPlaneIntersection(ray);
			if (intersectionPoint.HasValue) {
				if (pieceToSet != null) {
					if (placePieceIfAppropriate(intersectionPoint.Value)) {
						int pieceId = tileData.getId(pieceToSet);
						pieceToSet = null;
						setPiece(pieceId);
					}
				} else {
					removeExistingPieceIfAny(intersectionPoint.Value);
				}
			}
		}
	}

	bool isTrain(GameObject piece) {
		return piece.GetComponent<Train>() != null;
	}

	bool placeTrainIfAppropriate(Vector3 coord) {
		if (posToTrainMap.ContainsKey(coord) || !posToRailMap.ContainsKey(coord)) {
			return false;
		}

		Train train = pieceToSet.GetComponent<Train>();
		train.setCurrentTile(posToRailMap[coord]);
		train.setCurvePos(0.5f);

		posToTrainMap.Add(coord, pieceToSet);
		tileData.decrPiece(tileData.getId(pieceToSet));

		return true;
	}

	bool placeRailIfAppropriate(Vector3 coord) {
		if (posToRailMap.ContainsKey(coord)) {
			return false;
		}
		pieceToSet.transform.position = coord;
		posToRailMap.Add(coord, pieceToSet);
		tileData.decrPiece(tileData.getId(pieceToSet));
		return true;
	}

	bool placePieceIfAppropriate(Vector3 coord) {
		if (isTrain(pieceToSet)) {
			return placeTrainIfAppropriate(coord);
		}
		return placeRailIfAppropriate(coord);
	}

	void removeExistingPieceIfAny(Vector3 coord) {
		// choose which map to discard from: prefer removing train pieces first
		Dictionary<Vector3, GameObject> map = posToTrainMap.ContainsKey(coord) ? 
			posToTrainMap : posToRailMap.ContainsKey(coord) ? posToRailMap : null;
		if (map != null) {
			GameObject piece = map[coord];
			Destroy(piece);
			tileData.incrPiece(tileData.getId(piece));
			map.Remove(coord);
		}
	}

	Vector3? findGridPlaneIntersection(Ray ray) {
		// assume plane is horizontal: figure out where ray intersects
		Plane plane = new Plane(Vector3.up, gridLinesObject.transform.position);
		float rayDistance;
		if (plane.Raycast(ray, out rayDistance)) {
			return gridLinesObject.GetComponent<GridLines>().closestTilePosition(ray.GetPoint(rayDistance));
		}
		return null;
	}

	public void setPiece(int pieceId) {
		if (pieceToSet != null) {
			Destroy(pieceToSet);
		}
		if (pieceId >= 0 && tileData.hasPiece(pieceId)) {
			Vector3 pos = piecePreviewCamera.transform.position + piecePreviewCamera.transform.forward * 1.25f;
			pieceToSet = GameObject.Instantiate(tileData.pieces[pieceId], pos, Quaternion.identity) as GameObject;
		} else {
			pieceToSet = null;
		}
	}

	public void rotatePieceCW() {
		if (pieceToSet == null) {
			return;
		}
		pieceToSet.transform.Rotate(Vector3.up * 90f);
	}

	public void rotatePieceCCW() {
		if (pieceToSet == null) {
			return;
		}
		pieceToSet.transform.Rotate(Vector3.up * -90f);
	}
}
