using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TilePicker : MonoBehaviour {
	public Camera piecePreviewCamera;
	public GameObject gridLinesObject;
	public GameObject markerObject;
	public TileData tileData;
	public TileMap tileMap;

	private GameObject pieceToSet = null;
	private bool eraseMode = false;

	void Update() {
		// https://docs.unity3d.com/ScriptReference/EventSystems.EventSystem.IsPointerOverGameObject.html
		if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject()) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3? intersectionPoint = findGridPlaneIntersection(ray);
			if (intersectionPoint.HasValue) {
				if (pieceToSet != null) {
					if (placePieceIfAppropriate(intersectionPoint.Value)) {
						Debug.Log("Tile pos: " + intersectionPoint.Value);
						int pieceId = tileData.getId(pieceToSet);
						pieceToSet = null;
						setPiece(pieceId);
					}
				} else if (eraseMode) {
					removeExistingPieceIfAny(intersectionPoint.Value);
				}
			}
		}
	}

	bool isTrain(GameObject piece) {
		return piece.GetComponent<Train>() != null;
	}

	bool placeTrainIfAppropriate(Vector3 coord) {
		if (tileMap.trainExists(coord) || !tileMap.railExists(coord)) {
			return false;
		}

		Train train = pieceToSet.GetComponent<Train>();
		train.setCurrentTile(tileMap.getRail(coord));
		train.setCurvePos(0.5f);

		tileMap.putTrain(coord, pieceToSet);
		tileData.decrPiece(tileData.getId(pieceToSet));

		return true;
	}

	bool placeRailIfAppropriate(Vector3 coord) {
		if (tileMap.railExists(coord)) {
			return false;
		}

		pieceToSet.transform.position = coord;
		tileMap.putRail(coord, pieceToSet);
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
		GameObject pieceToRemove = tileMap.removeTrain(coord) ?? tileMap.removeRail(coord);
		if (pieceToRemove != null) {
			Destroy(pieceToRemove);
			tileData.incrPiece(tileData.getId(pieceToRemove));
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

	public void setEraseMode(bool val) {
		eraseMode = val;
	}

	public void setPiece(int pieceId) {
		if (pieceToSet != null) {
			Destroy(pieceToSet);
		}
		if (pieceId >= 0 && tileData.hasPiece(pieceId)) {
			GameObject piece = tileData.pieces[pieceId];

			if (isTrain(piece) && !tileMap.buildTrack()) {
				Debug.Log("Cannot set train b/c track building failed.");
				pieceToSet = null;
				return;
			}

			Vector3 pos = piecePreviewCamera.transform.position + piecePreviewCamera.transform.forward * 1.25f;
			pieceToSet = GameObject.Instantiate(piece, pos, Quaternion.identity) as GameObject;
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
