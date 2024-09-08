using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    //declare const variables
    const string COLOR_PARAMETER = "_Color";
    const string RESET_COLOR_METHOD = "ResetColor";

    //declare editor parameters
    [SerializeField] private int _startHealth = 10;
    [SerializeField] private Color _flickerColor = Color.white;
    [SerializeField] private float _flickerDuration = 0.1f;
    [SerializeField] private List<GameObject> _flickerList = new List<GameObject>();
    [SerializeField] private GameObject _healthBar = null;
    [SerializeField] private GameObject _healthBarPivot = null;
    [SerializeField] private bool _shouldDestroy = true;
    [SerializeField] private GameObject _healOrb = null;
    [SerializeField, Range(0.0f, 1.0f)] private float _healOrbDropChance = 0.3f;

    //declare editor getters/setters
    public float HealOrbDropChance
    {
        get { return _healOrbDropChance; }
        set { _healOrbDropChance = value; }
    }

    //declare script variables
    private int _currentHealth = 0;
    private List<Color> _startColors;
    private List<Material> _attachedMaterials;
    private PlayerCharacter _player = null;

    //declare other getters/setters
    public float HealthPercentage
    {
        get
        {
            return ((float)_currentHealth) / _startHealth;
        }
    }

    private void Awake()
    {
        //initiate the object at max health
        _currentHealth = _startHealth;

        //initiate the lists for colors and materials
        _startColors = new List<Color>();
        _attachedMaterials = new List<Material>();
    }

    private void Start()
    {
        //get all materials and their original color from the parts that need to flicker
        for (int i = 0; i < _flickerList.Count; ++i)
        {
            //check if the object that needs to flicker still exists
            if (_flickerList[i] == null)
                continue;

            MeshRenderer meshRenderer = _flickerList[i].GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                //add material to list
                _attachedMaterials.Add(meshRenderer.material);

                if (_attachedMaterials[i])
                {
                    //add start color to list
                    _startColors.Add(_attachedMaterials[i].GetColor(COLOR_PARAMETER));
                }
            }
        }

        //if the object has a health bar, find the player
        if (_healthBar != null && _healthBarPivot != null)
        {
            _player = FindObjectOfType<PlayerCharacter>();
        }
    }

    private void Update()
    {
        //check if the needed components exist
        if (_healthBar == null)
            return;

        if (_healthBarPivot == null)
            return;

        if (_player == null)
            return;

        //rotate the healthBar towards the player
        //calculate rotation for the health bar
        Vector3 lookRotation = Quaternion.LookRotation(_player.transform.position - _healthBar.transform.position).eulerAngles;
        lookRotation.x = 0.0f;
        lookRotation.z = 0.0f;

        //apply rotation to the health bar
        _healthBar.transform.rotation = Quaternion.Euler(lookRotation);

        //calculate scale to show remaining health
        Vector3 scale = _healthBarPivot.transform.localScale;
        scale.x = HealthPercentage;

        //apply scale to health bar
        _healthBarPivot.transform.localScale = scale;
    }

    public void Damage(int amount)
    {
        //ignore if health component is disabled
        if (!enabled)
            return;

        //apply damage to object
        _currentHealth -= amount;

        //handle flickering all objects in the flicker list
        for (int i = 0; i < _attachedMaterials.Count; ++i)
        {
            if (_attachedMaterials[i])
            {
                //indicate the object has taken damage
                _attachedMaterials[i].SetColor(COLOR_PARAMETER, _flickerColor);
            }
        }
        Invoke(RESET_COLOR_METHOD, _flickerDuration);

        //destroy the object when the health reaches 0
        if (_currentHealth <= 0)
        {
            //handle spawning healing orb
            if (Random.Range(0.0f, 1.0f) <= _healOrbDropChance)
            {
                if (_healOrb != null)
                {
                    Instantiate(_healOrb, gameObject.transform.position, gameObject.transform.rotation);
                }
            }

            //handle destroying the object
            if (_shouldDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
    public void Kill()
    {
        //ignore if health component is disabled
        if (!enabled)
            return;

        //handle flickering all objects in the flicker list
        for (int i = 0; i < _attachedMaterials.Count; ++i)
        {
            if (_attachedMaterials[i])
            {
                //indicate the object has taken damage
                _attachedMaterials[i].SetColor(COLOR_PARAMETER, _flickerColor);
            }
        }
        Invoke(RESET_COLOR_METHOD, _flickerDuration);

        //handle spawning healing orb
        if (Random.Range(0.0f, 1.0f) <= _healOrbDropChance)
        {
            if (_healOrb != null)
            {
                Instantiate(_healOrb, gameObject.transform.position, gameObject.transform.rotation);
            }
        }

        //handle destroying the object no matter how many health is left
        if (_shouldDestroy)
        {
            Destroy(gameObject);
        }
    }
    public void Heal(int amount)
    {
        //add some health to the object
        _currentHealth = Mathf.Min(_currentHealth + amount, _startHealth);
    }

    private void ResetColor()
    {
        //reset the color of all objects in flicker list
        for (int i = 0; i < _attachedMaterials.Count; ++i)
        {
            if (!_attachedMaterials[i])
                return;

            //reset the material color to it's original
            _attachedMaterials[i].SetColor(COLOR_PARAMETER, _startColors[i]);
        }
    }

    private void OnDestroy()
    {
        //handle destroying all created materials
        for (int i = 0; i < _attachedMaterials.Count; ++i)
        {
            if (!_attachedMaterials[i])
                return;

            //clean up the materials we created
            Destroy(_attachedMaterials[i]);
        }
    }
}
