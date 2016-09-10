using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TilePicker : MonoBehaviour {
	public Camera piecePreviewCamera;
	public GameObject gridLinesObject;
	public GameObject markerObject;
	public TileData tileData;

	private GameObject pieceToSet = null;
	private int pieceToSetId;

	void Update () {
		// https://docs.unity3d.com/ScriptReference/EventSystems.EventSystem.IsPointerOverGameObject.html
		if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject()) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3? intersectionPoint = findGridPlaneIntersection(ray);
			if (intersectionPoint.HasValue) {
				if (pieceToSet != null) {
					pieceToSet.transform.position = intersectionPoint.Value;
					pieceToSet = null;
					tileData.decrPiece(pieceToSetId);
					setPiece(pieceToSetId);
				}
			}
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
			Vector3 pos = piecePreviewCamera.transform.position + piecePreviewCamera.transform.forward;
			pieceToSet = GameObject.Instantiate(tileData.pieces[pieceId], pos, Quaternion.identity) as GameObject;
			pieceToSetId = pieceId;
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
