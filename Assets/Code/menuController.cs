using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour {

    public static MenuController Instance;
	private WorldController worldControllerScript;
	private Camera mainCamera;

	enum MenuState{MainMenu, LevelSelect, InGame, LevelCompleted};
	MenuState currentMenuState = MenuState.MainMenu;

	[SerializeField]
	private Vector3Int[] arrayLevelCubeFaceHamster;
    private int[] collected;
	private Vector2Int selectedLevelCube = Vector2Int.one;

	void Start () {
		// Find all active and inactive buttons within the children of the canvas
		foreach (Button button in this.gameObject.GetComponentsInChildren<Button>(true)) {
			button.onClick.AddListener (() => ButtonClicked (button.name));
		}
		worldControllerScript = GameObject.FindObjectOfType<WorldController> ();
		mainCamera = Camera.main;
//        HamsterReset();
        ResourceLoader.SoundMenu("MainMenuMusic");
	}
	void Awake(){
		if (Instance) {
			if (this == Instance) {
				return;
			} else
				Destroy (gameObject);
		}
		else {
			Instance = this;
			DontDestroyOnLoad (Instance);
		}
	}

	bool currentlyRotating = false;

	void Update() {
		if (Input.GetKeyUp (KeyCode.Escape))
			ReturnToMenu ();

        if (currentMenuState == MenuState.LevelSelect && !currentlyRotating)
        {
            float axisVal = Input.GetAxis("Horizontal");
            if (axisVal != 0)
            {
                StartCoroutine(SetCubeFace(axisVal));
            }
        }
        else if (currentMenuState == MenuState.InGame)
        {
            if (firstTime)
            {
                if(Input.GetAxis("Jump") != 0){
                    transform.Find("ControllerScreen").gameObject.SetActive(false);
                    firstTime = false;
                }
            }
        }

	}
		
	IEnumerator SetCubeFace(float axisVal){
        int levelChange = (int)Mathf.Sign (axisVal);
		int minMaxLevel = levelChange > 0 ? arrayLevelCubeFaceHamster.Length : 1;
		int newLevelID = selectedLevelCube.x + levelChange;

		// if selected level is already maximum level
		if (selectedLevelCube.x == minMaxLevel) {
			yield break;
		}
		currentlyRotating = true;

        ResourceLoader.SoundPlay("LevelScroll", transform);

		// If the selected Cube is not the cube it should be
		if (selectedLevelCube.y != arrayLevelCubeFaceHamster [newLevelID - 1].x) {
			worldControllerScript.ModLevel (levelChange, false);
		}

		yield return new WaitUntil (() => WorldController.isSpinning() == false);

		if (arrayLevelCubeFaceHamster [newLevelID - 1].y != worldControllerScript.GetCurrentFace ()) {
			worldControllerScript.RotateToFace (arrayLevelCubeFaceHamster [newLevelID - 1].y);
		}
		selectedLevelCube = new Vector2Int (newLevelID, arrayLevelCubeFaceHamster [newLevelID - 1].x);

		yield return new WaitUntil (() => WorldController.isSpinning() == false);
		yield return new WaitForSeconds (0.1f);
		currentlyRotating = false;
	}
		
	void ButtonClicked(string buttonName){
		if (buttonName == "Play") {
			currentMenuState = MenuState.LevelSelect;
		} else if (buttonName.Contains ("Begin")) {
			currentMenuState = MenuState.InGame;
			worldControllerScript.StartGame ();
		} else if (buttonName == "Next") {
			currentMenuState = MenuState.InGame;
            worldControllerScript.StartGame();
		} else if (buttonName == "Again") {
			currentMenuState = MenuState.InGame;
            Checkpoint.Reset();
			worldControllerScript.ReloadCurrentlevel (true);
		} else if (buttonName == "Quit") {
			Application.Quit ();
		} else
			Debug.Log (buttonName);
		UpdateUI ();
        ResourceLoader.SoundPlay("ButtonClick", transform);
    }

    bool firstTime = true;
	void UpdateUI(){
		Button buttonSelect = null;
		transform.Find ("LevelsPanel").gameObject.SetActive (false);
		transform.Find ("MainMenuPanel").gameObject.SetActive (false);
		transform.Find ("LevelCompleted").gameObject.SetActive (false);
        transform.Find("Game").gameObject.SetActive(false);
		if (currentMenuState == MenuState.MainMenu) {
            ResourceLoader.SoundMenu("MainMenuMusic");
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
			StartCoroutine (CameraZoom (false));
        }else if(currentMenuState == MenuState.InGame){
            // IN GAME
			StartCoroutine (CameraZoom (true));
			worldControllerScript.UpdateState (WorldController.GameState.Player);
            transform.Find("Game").gameObject.SetActive(true);
            ReloadHamsterUI();
            ResourceLoader.SoundMenu("GameMusic");
            if (firstTime)
            {
                transform.Find("ControllerScreen").gameObject.SetActive(true);
            }
                
		}
		if (buttonSelect) {
			EventSystem.current.SetSelectedGameObject (buttonSelect.gameObject);
			buttonSelect.OnSelect (null);
		}

	}

    public void PlayerEntered(GatewayScript gateScript)
	{
		if (gateScript.thisGateway == GatewayType.Final) {
			if(AllHamstersSaved())
				LevelFinished (true);
		}
	}

	[SerializeField]
	int cameraTime = 10;
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
					yield break;
				}
				yield return new WaitForSeconds (Time.deltaTime);
			}
		}
	}

	public void LevelFinished(bool success){
		currentMenuState = MenuState.LevelCompleted;
		worldControllerScript.ReloadCurrentlevel (false);
        ResetGemHamster();
	}

    private void ResetGemHamster(){
        HamsterReset ();
        GemsManager.ResetGems();
        UpdateUI ();
    }

	public void ReturnToMenu(){
		currentMenuState = MenuState.MainMenu;
		worldControllerScript.ReloadCurrentlevel (false);
		StartCoroutine (CameraZoom (false));
        ResetGemHamster();
	}

    int deathCount = 0;
    [SerializeField]
    Text[] deathText;
	public void PlayerDied(){
        deathCount++;
        foreach (Text death in deathText)
        {
            death.text = deathCount.ToString();
        }
		int selectedFace = arrayLevelCubeFaceHamster [selectedLevelCube.x-1].y;
		if (selectedFace != worldControllerScript.GetCurrentFace ()) {
			worldControllerScript.RotateToFace (arrayLevelCubeFaceHamster [selectedFace].y);
		}
		worldControllerScript.ReloadCurrentlevel (true);
        ResetGemHamster();
	}

	static int currentHamsters = 0;
    public GameObject[] hamsterInfo;
    [SerializeField]
    float hamsterSavedPopupTime = 1;
	public void HamsterSaved(Transform origin){
		currentHamsters++;
        ResourceLoader.SoundPlay("HamsterSaved", this.transform);
        HamsterSave.Instance.SaveTriggered(this.transform, hamsterSavedPopupTime);
        ReloadHamsterUI();
	}

	public static int GetHamsters(){
		return currentHamsters;
	}

	public void HamsterReset(){
		if (Checkpoint.GetHamsters() != 0)
			currentHamsters = Checkpoint.GetHamsters ();
		else
			currentHamsters = 0;

        ReloadHamsterUI();
	}

	bool AllHamstersSaved(){
		int req = selectedLevelCube.x - 1;
		return currentHamsters == arrayLevelCubeFaceHamster [req].z;
	}

    void ReloadHamsterUI(){
        foreach (GameObject singleHamster in hamsterInfo)
        {
            ResourceLoader.ImageUnloadAll(singleHamster.transform);

            if (currentMenuState == MenuState.InGame)
            {
                int hamstersInTheLevel = arrayLevelCubeFaceHamster[selectedLevelCube.x - 1].z;

                int counter = 0;
                for (int i = 0; i < hamstersInTheLevel; i++)
                {
                    if (counter < currentHamsters)
                    {
                        ResourceLoader.LoadImageObject("UI/Hamster_Ball_Saved", singleHamster.transform);
                    }
                    else
                    {
                        ResourceLoader.LoadImageObject("UI/Hamster_Ball", singleHamster.transform);
                    }
                    counter++;
                }
            }
        }
    }
}
