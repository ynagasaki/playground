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
	}

	public void stopTrain() {
	}

	void setText() {
		GetComponentInChildren<Text>().text = isRunning ? "Stop" : "Run";
	}
}
