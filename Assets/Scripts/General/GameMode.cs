using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMode : MonoBehaviour
{
    //declare const variables
    const string RETURN_TO_MENU = "ReturnToMenu";
    const string START_SCREEN = "StartScreen";

    //declare editor parameters
    [SerializeField] private BasicAbility _initialPlayerAbility = null;
    [SerializeField] private BasicAbility _initialHogAbility = null;

    //declare script variables
    private BasicAbility _playerAbility = null;
    private BasicAbility _hogAbility = null;
 
    //declare script getters/setters
    public BasicAbility PlayerAbility
    {
        get { return _playerAbility; }
        set { _playerAbility = value; }
    }
    public BasicAbility HogAbility
    {
        get { return _hogAbility; }
        set { _hogAbility = value; }
    }

    //handle GameMode being a Singleton
    #region SINGLETON
    private static GameMode _instance;
    public static GameMode Instance
    {
        get
        {
            if (_instance == null && !_applicationQuitting)
            {
                //find it in case it was placed in the scene
                _instance = FindObjectOfType<GameMode>();
                if (_instance == null)
                {
                    //none was found in the scene, create a new instance
                    GameObject newObject = new GameObject("Singleton_GameMode");
                    _instance = newObject.AddComponent<GameMode>();
                }
            }

            return _instance;
        }
    }
    private static bool _applicationQuitting = false;
    public void OnApplicationQuit()
    {
        _applicationQuitting = true;
    }

    private void Awake()
    {
        //initialize the default abilities
        if (_initialPlayerAbility != null)
            _playerAbility = _initialPlayerAbility;

        if (_initialHogAbility != null)
            _hogAbility = _initialHogAbility;

        //we want this object to persist when a scene changes
        DontDestroyOnLoad(gameObject);
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Update()
    {
        //handle returning to menu
        if (Input.GetButtonUp(RETURN_TO_MENU))
        {
            SceneManager.LoadScene(START_SCREEN);
        }
    }
}
