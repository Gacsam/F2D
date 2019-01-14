using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    enum MenuState { MainMenu, LevelSelect, InGame, LevelCompleted };
    MenuState currentMenuState = MenuState.MainMenu;

    int selectedLevel = 1;


    void Start()
    {
        // Find all active and inactive buttons within the children of the canvas
        foreach (Button button in this.gameObject.GetComponentsInChildren<Button>(true))
        {
            button.onClick.AddListener(() => ButtonClicked(button.name));
        }
        RefreshUI();
        levelSelectPanelBackground = transform.Find("LevelSelect/BG");
    }

    void Update()
    {
        if (currentMenuState == MenuState.LevelSelect)
        {
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                LevelSelect(Input.GetAxisRaw("Horizontal"));
            }
        }
    }

    Transform levelSelectPanelBackground;
    void LevelSelect(float horizontalAxis)
    {
        if (horizontalAxis > 0)
        {
            // taking into mind a "level select" scene
            if (selectedLevel >= SceneManager.sceneCountInBuildSettings-1)
                return;
            selectedLevel++;
        }
        // using else-if instead of else lets me run LevelSelect(0) to load current level's image
        else if(horizontalAxis < 0)
        {
            if (selectedLevel <= 1)
                return;
            selectedLevel--;
        }
        ResourceLoader.LoadImage("UI/LevelSelect/" + selectedLevel, levelSelectPanelBackground);
    }

    void ButtonClicked(string buttonName)
    {
        if (buttonName == "Play")
        {
            currentMenuState = MenuState.LevelSelect;
        }
        else if (buttonName.Contains("Begin"))
        {
            currentMenuState = MenuState.InGame;
        }
        else if (buttonName == "Next")
        {
            currentMenuState = MenuState.InGame;
        }
        else if (buttonName == "Again")
        {
            currentMenuState = MenuState.InGame;
        }
        else if (buttonName == "Quit")
        {
            Application.Quit();
        }
        else
            Debug.Log(buttonName);
        RefreshUI();
        ResourceLoader.SoundPlay("ButtonClick", transform);
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(selectedLevel + 1);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    void RefreshUI()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        if(currentMenuState == MenuState.MainMenu)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else if(currentMenuState == MenuState.LevelSelect)
        {
            LevelSelect(0);
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }

}
