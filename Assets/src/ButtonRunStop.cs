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
		setText();
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
		trainInstance.setCurvePos(0f);
		trainInstance.IsRunning = isRunning = true;
		editPanel.SetActive(false);
	}

	public void stopTrain() {
		if (!isRunning) {
			return;
		}

		trainInstance.IsRunning = isRunning = false;
		trainInstance.setCurrentTile(tileMap.StartNode);
		trainInstance.setCurvePos(0f);
		editPanel.SetActive(true);
	}

	void setText() {
		GetComponentInChildren<Text>().text = isRunning ? "Stop" : "Run";
	}
}
