using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TilePicker : MonoBehaviour {
	public Camera piecePreviewCamera;
	public GameObject gridLinesObject;
	public TileData tileData;
	public TileMap tileMap;

	private GameObject pieceToSet = null;
	private bool eraseMode = false;
	private bool isDragging = false;

	private const float WITHIN_DIST_SQR = 0.04f;

	// https://docs.unity3d.com/ScriptReference/EventSystems.EventSystem.IsPointerOverGameObject.html
	bool PointerIsOverUIElement {
		get {
			return EventSystem.current.IsPointerOverGameObject();
		}
	}

	void Update() {
		if (isDragging && !PointerIsOverUIElement) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3? intersectionPoint = findGridPlaneIntersection(ray, false);
			pieceToSet.transform.position = intersectionPoint.Value;
		} else if (eraseMode && Input.GetMouseButtonUp(0) && !PointerIsOverUIElement) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3? intersectionPoint = findGridPlaneIntersection(ray, true);
			removeExistingPieceIfAny(intersectionPoint.Value);
		}
	}

	bool performTilePlacement() {
		if (Input.GetMouseButtonUp(0) && !PointerIsOverUIElement) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3? intersectionPoint = findGridPlaneIntersection(ray, true);
			if (intersectionPoint.HasValue) {
				if (pieceToSet != null) {
					if (placeRailIfAppropriate(intersectionPoint.Value)) {
						int pieceId = tileData.getId(pieceToSet);
						pieceToSet = null;
						setPiece(pieceId);
						return true;
					}
				}
			}
		}
		return false;
	}

	bool isTrain(GameObject piece) {
		return piece.GetComponent<Train>() != null;
	}

	bool placeRailIfAppropriate(Vector3 coord) {
		TileRailPiece tileRail = pieceToSet.GetComponent<TileRailPiece>();

		Vector3? foundEndpoint = null;
		EndpointType? endpointTypeToConnect = null;

		if (!tileMap.IsEmpty) {
			// figure out if this is an appropriate place to drop our new piece
			if (!tileMap.getEndpointWithin(WITHIN_DIST_SQR, tileRail.StartPoint, out foundEndpoint)) {
				if (!tileMap.getEndpointWithin(WITHIN_DIST_SQR, tileRail.EndPoint, out foundEndpoint)) {
					Debug.Log("Not close enough.");
					return false;
				} else {
					endpointTypeToConnect = EndpointType.End;
				}
			} else {
				endpointTypeToConnect = EndpointType.Start;
			}
			Debug.Assert(foundEndpoint.HasValue && endpointTypeToConnect.HasValue);
		}

		// this has to be set first...
		pieceToSet.transform.position = coord;
		// connect the piece to existing rail pieces
		if (tileMap.connect(pieceToSet, endpointTypeToConnect, foundEndpoint)) {
			// drop the new piece
			tileData.decrPiece(tileData.getId(pieceToSet));
			return true;
		}

		return false;
	}

	void removeExistingPieceIfAny(Vector3 coord) {
		Debug.Log("Not implemented!!!");
	}

	Vector3? findGridPlaneIntersection(Ray ray, bool snapToGrid) {
		// assume plane is horizontal: figure out where ray intersects
		Plane plane = new Plane(Vector3.up, gridLinesObject.transform.position);
		float rayDistance;
		if (plane.Raycast(ray, out rayDistance)) {
			Vector3 planeIntersectionLoc = ray.GetPoint(rayDistance);
			if (snapToGrid) {
				return gridLinesObject.GetComponent<GridLines>().closestTilePosition(planeIntersectionLoc);
			}
			return planeIntersectionLoc;
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
			pieceToSet = GameObject.Instantiate(piece, Vector3.zero, Quaternion.identity) as GameObject;
			placePieceInPreview();
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

	public void dragPieceBegin() {
		if (pieceToSet == null) {
			return;
		}
		isDragging = true;
	}

	public void dragPieceEnd() {
		if (!isDragging) {
			return;
		}
		isDragging = false;
		if (PointerIsOverUIElement || !performTilePlacement()) {
			placePieceInPreview();
		}
	}

	void placePieceInPreview() {
		if (pieceToSet == null) {
			return;
		}
		pieceToSet.transform.position = piecePreviewCamera.transform.position + piecePreviewCamera.transform.forward * 1.25f;
	}
}
