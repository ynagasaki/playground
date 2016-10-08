using UnityEngine;
using UnityEngine.UI;

public class ButtonRunStop : MonoBehaviour {
	public TileMap tileMap;
	public GameObject editPanel;
	public Train train;

	private bool isRunning = false;
	private Train trainInstance = null;

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
		if (isRunning) {
			return;
		}

		if (!tileMap.IsReady) {
			Debug.Log("Not ready to run train.");
			return;
		}

		if (trainInstance == null) {
			trainInstance = Instantiate(train.gameObject).GetComponent<Train>();
		}

		trainInstance.setCurrentTile(tileMap.StartNode);
		trainInstance.IsRunning = isRunning = true;
	}

	public void stopTrain() {
		if (!isRunning) {
			return;
		}

		trainInstance.IsRunning = isRunning = false;
		trainInstance.setCurrentTile(tileMap.StartNode);
	}

	void setText() {
		GetComponentInChildren<Text>().text = isRunning ? "Stop" : "Run";
	}
}
