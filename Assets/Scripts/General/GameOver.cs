using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    //declare const variables
    const string ABILITIES_SCREEN = "AbilitiesScreen";

    //declare editor parameters
    [SerializeField] private GameObject _player = null;
    [SerializeField] private GameObject _townHall = null;
    [SerializeField] private GameObject _loseHUD = null;
    [SerializeField] private GameObject _winHUD = null;
    [SerializeField] private AudioSource _hogRidaaSound = null;

    //declare script variables
    private TownHall _townHallComponent = null;
    private bool _isGameOver = false;

    private void Start()
    {
        //set initial values
        if (_townHall != null)
        {
            _townHallComponent = _townHall.GetComponent<TownHall>();
        }

        if (_loseHUD != null)
        {
            _loseHUD.SetActive(false);
        }

        if (_winHUD != null)
        {
            _winHUD.SetActive(false);
        }

        //making sure updates are enabled
        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        //ignore updates if the game is over
        if (_isGameOver)
            return;

        //handle player lost
        if (_player == null)
            TriggerGameLost();

        //handle player won
        if (_townHallComponent != null && _townHallComponent.IsDestroyed)
            TriggerGameWon();
    }

    public void TriggerGameLost()
    {
        //handle enabling the player lost HUD
        if (_loseHUD != null)
        {
            _loseHUD.SetActive(true);

            _isGameOver = true;

            //stop updates
            Time.timeScale = 0.0f;

            //unlock the mouse for HUD inputs
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            //play hog ridaa sound
            if (_hogRidaaSound != null)
            {
                _hogRidaaSound.pitch = 0.8f;
                _hogRidaaSound.Play();
            }
        }
    }

    public void TriggerGameWon()
    {
        //handle enabling the player won HUD
        if (_winHUD != null)
        {
            _winHUD.SetActive(true);

            _isGameOver = true;

            //stop updates
            Time.timeScale = 0.0f;

            //unlock the mouse for HUD inputs
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            //play hog ridaa sound
            if (_hogRidaaSound != null)
                _hogRidaaSound.Play();
        }
    }
}
