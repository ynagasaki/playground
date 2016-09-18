using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class TilePieceDropdown : MonoBehaviour {
	public TileData tileData;
	public TilePicker tilePicker;

	void Start() {
		tileData.registerUpdateListener(this.resetOptions);
		resetOptions(tileData);
	}

	public void resetOptions(TileData data) {
		Dropdown dropdown = GetComponent<Dropdown>();
		List<string> options = new List<string>();
		options.Add("Erase");
		for (int id = 0; id < data.count(); id ++) {
			string option = tileData.names[id] + " (" + data.counts[id] + ")";
			options.Add(option);
		}
		dropdown.ClearOptions();
		dropdown.AddOptions(options);
	}

	public void optionChanged(int id) {
		tilePicker.setPiece(id - 1);
		tilePicker.setEraseMode(id == 0);
	}
}
