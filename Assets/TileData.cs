using UnityEngine;
using System.Collections.Generic;

public class TileData : MonoBehaviour {
	public delegate void UpdateListener(TileData data);

	public string[] names;
	public GameObject[] pieces;
	public int[] counts;

	private List<UpdateListener> updateListeners = new List<UpdateListener>();

	public void registerUpdateListener(UpdateListener handler) {
		updateListeners.Add(handler);
	}

	public void decrPiece(int id) {
		counts[id]--;
		foreach (UpdateListener listener in updateListeners) {
			listener(this);
		}
	}

	public bool hasPiece(int id) {
		return counts[id] > 0;
	}

	public int count() {
		return names.Length;
	}
}
