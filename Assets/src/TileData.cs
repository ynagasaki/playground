using UnityEngine;
using System.Collections.Generic;

public class TileData : MonoBehaviour {
	public delegate void UpdateListener(TileData data);

	public string[] names;
	public GameObject[] pieces;
	public int[] counts;

	private List<UpdateListener> updateListeners = new List<UpdateListener>();
	private Dictionary<string, int> pieceNameToId = new Dictionary<string, int>();

	void Start() {
		for (int i = 0; i < pieces.Length; i++) {
			GameObject piece = pieces[i];
			pieceNameToId.Add(piece.name, i);
		}
	}

	public void registerUpdateListener(UpdateListener handler) {
		updateListeners.Add(handler);
	}

	public void decrPiece(int id) {
		counts[id]--;
		notifyChangeListeners();
	}

	public void incrPiece(int id) {
		counts[id]++;
		notifyChangeListeners();
	}

	public bool hasPiece(int id) {
		return counts[id] > 0;
	}

	public int count() {
		return names.Length;
	}

	public int getId(GameObject piece) {
		int choppy = piece.name.LastIndexOf("(Clone)");
		string origPieceName = choppy < 0 ? piece.name : piece.name.Substring(0, choppy);
		return pieceNameToId[origPieceName];
	}

	private void notifyChangeListeners() {
		foreach (UpdateListener listener in updateListeners) {
			listener(this);
		}
	}
}
