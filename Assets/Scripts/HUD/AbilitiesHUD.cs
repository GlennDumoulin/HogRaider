using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AbilitiesHUD : MonoBehaviour
{
    //declare const variables
    const string DURATION_NOT_SPECIFIED = "Not specified!";
    const string LEVEL_1 = "Level1";
    const string START_SCREEN = "StartScreen";

    //declare editor parameters
    [SerializeField] private TextMeshProUGUI _playerName = null;
    [SerializeField] private TextMeshProUGUI _hogName = null;
    [SerializeField] private TextMeshProUGUI _playerDescription = null;
    [SerializeField] private TextMeshProUGUI _hogDescription = null;
    [SerializeField] private TextMeshProUGUI _playerDuration = null;
    [SerializeField] private TextMeshProUGUI _hogDuration = null;
    [SerializeField] private TextMeshProUGUI _playerCooldown = null;
    [SerializeField] private TextMeshProUGUI _hogCooldown = null;
    [SerializeField] private AudioSource _selectSound = null;
    
    //declare script variables
    private GameMode _gameMode = null;

    private void Start()
    {
        //make sure the cursor is unlocked for HUD inputs
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //set initial values
        _gameMode = GameMode.Instance;

        if (_gameMode != null)
        {
            UpdatePlayerAbilityInfo(_gameMode.PlayerAbility);
            UpdateHogAbilityInfo(_gameMode.HogAbility);
        }
    }

    public void SetPlayerAbilityInfo(GameObject toggleObject)
    {
        //check if the needed components exist
        if (toggleObject == null)
            return;

        Toggle toggle = toggleObject.GetComponent<Toggle>();

        if (toggle == null)
            return;

        if (!toggle.isOn)
            return;

        ToggleValue toggleValue = toggleObject.GetComponent<ToggleValue>();

        if (toggleValue == null)
            return;

        //play select sound
        if (_selectSound != null)
            _selectSound.Play();

        //handle updating the player ability info
        UpdatePlayerAbilityInfo(toggleValue.Ability);
    }
    private void UpdatePlayerAbilityInfo(BasicAbility ability)
    {
        //update selected player ability info
        if (ability == null)
            return;

        if (_gameMode != null)
            _gameMode.PlayerAbility = ability;

        if (_playerName != null)
            _playerName.text = ability._name;

        if (_playerDescription != null)
            _playerDescription.text = ability._description;

        if (_playerDuration != null)
        {
            if (ability._duration != -1)
            {
                _playerDuration.text = ability._duration.ToString();
            }
            else
            {
                _playerDuration.text = DURATION_NOT_SPECIFIED;
            }
        }
        

        if (_playerCooldown != null)
            _playerCooldown.text = ability._cooldown.ToString();
    }

    public void SetHogAbilityInfo(GameObject toggleObject)
    {
        //check if the needed components exist
        if (toggleObject == null)
            return;

        Toggle toggle = toggleObject.GetComponent<Toggle>();

        if (toggle == null)
            return;

        if (!toggle.isOn)
            return;

        ToggleValue toggleValue = toggleObject.GetComponent<ToggleValue>();

        if (toggleValue == null)
            return;

        //play select sound
        if (_selectSound != null)
            _selectSound.Play();

        //handle updating the hog ability info
        UpdateHogAbilityInfo(toggleValue.Ability);
    }
    private void UpdateHogAbilityInfo(BasicAbility ability)
    {
        //update selected hog ability info
        if (ability == null)
            return;

        if (_gameMode != null)
            _gameMode.HogAbility = ability;

        if (_hogName != null)
            _hogName.text = ability._name;

        if (_hogDescription != null)
            _hogDescription.text = ability._description;

        if (_hogDuration != null)
        {
            if (ability._duration != -1)
            {
                _hogDuration.text = ability._duration.ToString();
            }
            else
            {
                _hogDuration.text = DURATION_NOT_SPECIFIED;
            }
        }

        if (_hogCooldown != null)
            _hogCooldown.text = ability._cooldown.ToString();
    }

    public void StartLevel()
    {
        //load level
        SceneManager.LoadScene(LEVEL_1);
    }

    public void BackToMenu()
    {
        //load start screen
        SceneManager.LoadScene(START_SCREEN);
    }
}
