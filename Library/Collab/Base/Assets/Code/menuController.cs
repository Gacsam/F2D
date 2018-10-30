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
//			levelControlScript.ReloadCurrentlevel ();
		} else if (buttonName == "Again") {
			currentMenuState = MenuState.InGame;
			levelControlScript.ReloadCurrentlevel ();
		} else if (buttonName == "Quit") {
			Application.Quit ();
		} else
			Debug.Log (buttonName);
		UpdateMenu ();
	}

	void UpdateMenu(){
		Button buttonSelect = null;
		levelControlScript.enabled = false;
		Camera.main.fieldOfView = 60;
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
		}else{
			Camera.main.fieldOfView = 20;
		}
		if (buttonSelect) {
			EventSystem.current.SetSelectedGameObject (buttonSelect.gameObject);
			buttonSelect.OnSelect (null);
		}
	}

	public void LevelFinished(bool success){
		currentMenuState = MenuState.LevelCompleted;
		UpdateMenu ();
	}

	public void ReturnToMenu(){
		currentMenuState = MenuState.MainMenu;
		UpdateMenu ();
	}
}
