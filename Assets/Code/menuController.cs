using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class menuController : MonoBehaviour {

	private static menuController singleton;
	private WorldController worldControllerScript;
	private Camera mainCamera;

	enum MenuState{MainMenu, LevelSelect, InGame, LevelCompleted};
	MenuState currentMenuState = MenuState.MainMenu;

	[SerializeField]
	private Vector2Int[] arrayLevelCubeFace;
	private Vector2Int selectedLevelCube = Vector2Int.one;

	[SerializeField]
	private float breakTime = 1;

	void Start () {
		// Find all active and inactive buttons within the children of the canvas
		foreach (Button button in this.gameObject.GetComponentsInChildren<Button>(true)) {
			button.onClick.AddListener (() => ButtonClicked (button.name));
		}
		worldControllerScript = GameObject.FindObjectOfType<WorldController> ();
		mainCamera = Camera.main;
	}

	void Awake(){
		if (singleton) {
			if (this == singleton) {
				return;
			} else
				Destroy (gameObject);
		}
		else {
			singleton = this;
			DontDestroyOnLoad (singleton);
		}
	}

	bool currentlyRotating = false;

	void Update () {
		if (Input.GetKeyUp (KeyCode.Escape))
			ReturnToMenu ();

		if (currentMenuState == MenuState.LevelSelect && !currentlyRotating) {
			if (Input.GetAxis ("Horizontal") != 0) {
				StartCoroutine (SetCubeFace ());
			}
		}
	}
		
	IEnumerator SetCubeFace(){
		int levelChange = (int)Mathf.Sign (Input.GetAxis ("Horizontal"));
		int minMaxLevel = levelChange > 0 ? arrayLevelCubeFace.Length : 1;
		int newLevelID = selectedLevelCube.x + levelChange;

		// if selected level is already maximum level
		if (selectedLevelCube.x == minMaxLevel) {
			yield break;
		}
		currentlyRotating = true;
		Text levelText = transform.Find ("LevelsPanel").GetComponentInChildren<Text> ();
		levelText.text = "Level " + (newLevelID);

		// If the selected Cube is not the cube it should be
		if (selectedLevelCube.y != arrayLevelCubeFace [newLevelID - 1].x) {
			worldControllerScript.ModLevel (levelChange, false);
		}

		yield return new WaitUntil (() => worldControllerScript.isSpinning() == false);

		if (arrayLevelCubeFace [newLevelID - 1].y != worldControllerScript.GetCurrentFace ()) {
			worldControllerScript.RotateToFace (arrayLevelCubeFace [newLevelID - 1].y);
		}
		selectedLevelCube = new Vector2Int (newLevelID, arrayLevelCubeFace [newLevelID - 1].x);

		yield return new WaitUntil (() => worldControllerScript.isSpinning() == false);
		yield return new WaitForSeconds (breakTime);
		currentlyRotating = false;
	}
		
	void ButtonClicked(string buttonName){
		if (buttonName == "Play") {
			currentMenuState = MenuState.LevelSelect;
		} else if (buttonName.Contains ("Level_")) {
			currentMenuState = MenuState.InGame;
			worldControllerScript.StartGame ();
		} else if (buttonName == "Next") {
			currentMenuState = MenuState.InGame;
            worldControllerScript.StartGame();
		} else if (buttonName == "Again") {
			currentMenuState = MenuState.InGame;
			worldControllerScript.ReloadCurrentlevel (true);
		} else if (buttonName == "Quit") {
			Application.Quit ();
		} else
			Debug.Log (buttonName);
		UpdateMenu ();
	}

	void UpdateMenu(){
		Button buttonSelect = null;
		transform.Find ("LevelsPanel").gameObject.SetActive (false);
		transform.Find ("MainMenuPanel").gameObject.SetActive (false);
		transform.Find ("LevelCompleted").gameObject.SetActive (false);
		if (currentMenuState == MenuState.MainMenu) {
			worldControllerScript.UpdateState (WorldController.GameState.Menu);
			GameObject activeMenu = transform.Find ("MainMenuPanel").gameObject;
			activeMenu.SetActive (true);
			buttonSelect = activeMenu.transform.GetChild (0).gameObject.GetComponent<Button>();
		} else if (currentMenuState == MenuState.LevelSelect) {
			GameObject activeMenu = transform.Find ("LevelsPanel").gameObject;
			activeMenu.SetActive (true);
			buttonSelect = activeMenu.transform.GetChild (0).gameObject.GetComponent<Button> ();
		} else if (currentMenuState == MenuState.LevelCompleted) {
			transform.Find ("LevelCompleted").gameObject.SetActive (true);
			cameraCoroutine = StartCoroutine (CameraZoom (false));
		}else{
			cameraCoroutine = StartCoroutine (CameraZoom (true));
			worldControllerScript.UpdateState (WorldController.GameState.Player);
		}
		if (buttonSelect) {
			EventSystem.current.SetSelectedGameObject (buttonSelect.gameObject);
			buttonSelect.OnSelect (null);
		}
			

		foreach (GatewayScript door in worldControllerScript.GetComponentsInChildren<GatewayScript>()) {
			if (door.isFinal)
				door.playerEntered += PlayerEntered;
		}
	}

	private void PlayerEntered(GatewayScript theScript)
	{
		if (theScript.isFinal)
			LevelFinished (true);
	}

	[SerializeField]
	int cameraTime = 10;
	Coroutine cameraCoroutine;
	IEnumerator CameraZoom (bool isZoomingIn)
	{
		int fovCheck = isZoomingIn ? 60 : 20;
		if (mainCamera.fieldOfView == fovCheck) {
			int zoomTime = 40 / cameraTime;
			int timer = 0;
			while (true) {
				timer += zoomTime;
				mainCamera.fieldOfView -= isZoomingIn ? zoomTime : -zoomTime;
				if (timer >= 40) {
					StopCoroutine (cameraCoroutine);
				}
				yield return new WaitForSeconds (Time.deltaTime);
			}
		}
	}

	public void LevelFinished(bool success){
		currentMenuState = MenuState.LevelCompleted;
		UpdateMenu ();
	}

	public void ReturnToMenu(){
		currentMenuState = MenuState.MainMenu;
		worldControllerScript.ReloadCurrentlevel (false);
		cameraCoroutine = StartCoroutine (CameraZoom (false));
		UpdateMenu ();
	}

	public void PlayerDied(){
		int selectedFace = arrayLevelCubeFace [selectedLevelCube.x-1].y;
		if (selectedFace != worldControllerScript.GetCurrentFace ()) {
			worldControllerScript.RotateToFace (arrayLevelCubeFace [selectedFace].y);
		}
		worldControllerScript.ReloadCurrentlevel (true);
	}
}
