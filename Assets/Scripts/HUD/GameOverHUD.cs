using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHUD : MonoBehaviour
{
    //declare const variables
    const string START_SCREEN = "StartScreen";
    const string ABILITIES_SCREEN = "AbilitiesScreen";
    const string TUTORIAL_LEVEL = "TutorialLevel";

    public void GoToStartScreen()
    {
        //load start screen
        SceneManager.LoadScene(START_SCREEN);
    }
    public void GoToAbilitiesScreen()
    {
        //load abilities screen
        SceneManager.LoadScene(ABILITIES_SCREEN);
    }

    public void RestartTutorial()
    {
        //load tutorial
        SceneManager.LoadScene(TUTORIAL_LEVEL);
    }

    public void CloseGame()
    {
        //close application
        Application.Quit();
    }
}
