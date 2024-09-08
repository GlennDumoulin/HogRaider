using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartHUD : MonoBehaviour
{
    //declare const variables
    const string ABILITIES_SCREEN = "AbilitiesScreen";
    const string TUTORIAL_LEVEL = "TutorialLevel";

    private void Start()
    {
        //make sure the cursor is unlocked for HUD inputs
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GoToAbilitiesScreen()
    {
        //load abilities screen
        SceneManager.LoadScene(ABILITIES_SCREEN);
    }
    public void StartTutorial()
    {
        //load tutorial level
        SceneManager.LoadScene(TUTORIAL_LEVEL);
    }

    public void CloseGame()
    {
        //close application
        Application.Quit();
    }
}
