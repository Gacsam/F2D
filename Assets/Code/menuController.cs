using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class menuController : MonoBehaviour {

	private static menuController singleton;
	private LevelControl levelControlScript;

	enum MenuState{MainMenu, LevelSelect, InGame, LevelCompleted};
	MenuState currentMenuState = MenuState.MainMenu;

	// Use this for initialization
	void Start () {
		// Find all active and inactive buttons within the children of the canvas
		foreach (Button button in this.gameObject.GetComponentsInChildren<Button>(true)) {
			button.onClick.AddListener (() => ButtonClicked (button.name));
		}
		levelControlScript = GameObject.FindObjectOfType<LevelControl> ();
	}

	void Awake(){
		if (singleton)
			return;
		else {
			singleton = this;
			DontDestroyOnLoad (singleton);
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.Escape))
			ReturnToMenu ();

		if (currentMenuState == MenuState.LevelSelect) {
			Text levelText = transform.Find ("LevelsPanel").GetComponentInChildren<Text> ();
			levelText.text = "Level " + levelControlScript.GetCurrentLevel();
		}
	}
					
	void ButtonClicked(string buttonName){
		if (buttonName == "Play") {
			currentMenuState = MenuState.LevelSelect;
		} else if (buttonName.Contains ("Level_")) {
			currentMenuState = MenuState.InGame;
			levelControlScript.StartGame ();
		} else if (buttonName == "Next") {
			currentMenuState = MenuState.InGame;
			levelControlScript.ModLevel (1, true);
		} else if (buttonName == "Again") {
			currentMenuState = MenuState.InGame;
			levelControlScript.ReloadCurrentlevel (true);
		} else if (buttonName == "Quit") {
			Application.Quit ();
		} else
			Debug.Log (buttonName);
		UpdateMenu ();
	}

	void UpdateMenu(){
		Button buttonSelect = null;
		levelControlScript.enabled = false;
		transform.Find ("LevelsPanel").gameObject.SetActive (false);
		transform.Find ("MainMenuPanel").gameObject.SetActive (false);
		transform.Find ("LevelCompleted").gameObject.SetActive (false);
		if (currentMenuState == MenuState.MainMenu) {
			GameObject activeMenu = transform.Find ("MainMenuPanel").gameObject;
			activeMenu.SetActive (true);
			buttonSelect = activeMenu.transform.GetChild (0).gameObject.GetComponent<Button>();
		} else if (currentMenuState == MenuState.LevelSelect) {
			levelControlScript.enabled = true;
			GameObject activeMenu = transform.Find ("LevelsPanel").gameObject;
			activeMenu.SetActive (true);
			buttonSelect = activeMenu.transform.GetChild (0).gameObject.GetComponent<Button> ();
		} else if (currentMenuState == MenuState.LevelCompleted) {
			transform.Find ("LevelCompleted").gameObject.SetActive (true);
			cameraCoroutine = StartCoroutine (CameraZoom (false));
		}else{
			cameraCoroutine = StartCoroutine (CameraZoom (true));
		}

		if (buttonSelect) {
			EventSystem.current.SetSelectedGameObject (buttonSelect.gameObject);
			buttonSelect.OnSelect (null);
		}
	}

	[SerializeField]
	int cameraTime = 10;
	Coroutine cameraCoroutine;
	IEnumerator CameraZoom (bool isZoomingIn)
	{
		int fovCheck = isZoomingIn ? 60 : 20;
		if (Camera.main.fieldOfView == fovCheck) {
			int zoomTime = 40 / cameraTime;
			int timer = 0;
			while (true) {
				timer += zoomTime;
				Camera.main.fieldOfView -= isZoomingIn ? zoomTime : -zoomTime;
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
		levelControlScript.ReloadCurrentlevel (false);
		cameraCoroutine = StartCoroutine (CameraZoom (false));
		UpdateMenu ();
	}
}
