using UnityEngine;
using UnityEngine.UI;

public class ButtonRunStop : MonoBehaviour {
	public TileMap tileMap;
	public GameObject editPanel;

	private bool isRunning = false;

	public void Start() {
		setText();
	}

	public void toggleRun() {
		if (isRunning) {
			stopTrain();
		} else {
			runTrain();
		}
	}

	public void runTrain() {
		if (tileMap.startTrain()) {
			editPanel.SetActive(false);
			isRunning = true;
			setText();
		}
	}

	public void stopTrain() {
		if (tileMap.stopTrain()) {
			editPanel.SetActive(true);
			isRunning = false;
			setText();
		}
	}

	void setText() {
		GetComponentInChildren<Text>().text = isRunning ? "Stop" : "Run";
	}
}
