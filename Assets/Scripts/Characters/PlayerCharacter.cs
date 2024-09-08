using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : BasicCharacter
{
    //declare const variables
    const string FORWARD_MOVEMENT = "ForwardMovement";
    const string ATTACK = "Attack";
    const string USE_ABILITY = "UseAbility";
    const string LOOK_AROUND = "LookAround";
    const string SWITCH_ITEM = "SwitchItem";
    const string ATTACK_TRIGGER = "Attack";
    const string IDLE_STATE = "Idle";
    const string WEAPON_ATTACK_STATE = "WeaponAttack";

    //declare editor parameters
    [SerializeField] private List<TrailRenderer> _speedTrails = null;
    [SerializeField] private GameObject _smashAttack = null;
    [SerializeField, Range(5, 20)] private int _smashDamage = 10;
    [SerializeField] private AudioSource _runningSound = null;
    
    //declare editor getters/setters
    public int SmashDamage
    {
        get { return _smashDamage; }
    }

    //declare script variables
    private GameMode _gameMode = null;
    private Animator _smashAnimator = null;
    private bool _isSmashing = false;
    private bool _canDisableSmash = false;
    private float _healDelay = 1.0f;
    private float _currentHealDelay = 0.0f;
    
    //declare script getters/setters
    public GameMode GameMode
    {
        get { return _gameMode; }
    }

    protected override void Awake()
    {
        //execute BasicCharacter's Awake
        base.Awake();

        //get some needed components
        _gameMode = FindObjectOfType<GameMode>();

        if (_smashAttack != null)
            _smashAnimator = _smashAttack.GetComponent<Animator>();

        //lock cursor in screen
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //handle updating movement
        HandleMovementInput();

        //handle updating item based stuff
        HandleSwitchItem();
        HandleUseAbility();
        HandleAttack();

        //handle updating abilities
        HandleAbilities();
    }

    private void HandleMovementInput()
    {
        //check if a movement behaviour exists
        if (_movementBehaviour == null)
            return;

        //handle movement
        //only allow moving when holding the steering item and player is not smashing
        if (_itemBehaviour != null && _itemBehaviour.CurrentItemType == ItemBehaviour.ItemType.Steering && !_itemBehaviour.IsSmashing)
        {
            float forwardMovement = Input.GetAxis(FORWARD_MOVEMENT);

            Vector3 movement = forwardMovement * transform.forward;
            _movementBehaviour.DesiredMovementDirection = movement;

            //handle playing/stopping running sound
            if (forwardMovement != 0.0f)
            {
                //play running sound
                if (_runningSound != null)
                {
                    if (!_runningSound.isPlaying)
                        _runningSound.Play();
                }
            }
            else
            {
                //stop running sound
                if (_runningSound != null)
                {
                    if (_runningSound.isPlaying)
                        _runningSound.Stop();
                }
            }
        }
        else //if not holding the steering item or if smashing
        {
            _movementBehaviour.DesiredMovementDirection = Vector3.zero;

            //stop running sound
            if (_runningSound != null)
            {
                if (_runningSound.isPlaying)
                {
                    _runningSound.Stop();
                }
            }
        }

        //handle rotation
        float lookAroundDirection = Input.GetAxis(LOOK_AROUND);

        Vector3 rotation = lookAroundDirection * Vector3.up;
        _movementBehaviour.DesiredLookatPoint = rotation;
    }

    private void HandleSwitchItem()
    {
        //check if the item behaviour exists
        if (_itemBehaviour == null)
            return;

        //handle switching items
        if (Input.GetAxis(SWITCH_ITEM) != 0.0f)
        {
            _itemBehaviour.SwitchItem();
        }
    }
    private void HandleUseAbility()
    {
        //check if the needed components exist
        if (_itemBehaviour == null)
            return;

        if (_gameMode == null)
            return;

        //handle using an ability
        if (Input.GetButton(USE_ABILITY))
        {
            if (_itemBehaviour.CurrentItemType == ItemBehaviour.ItemType.Weapon)
            {
                //use player ability
                _itemBehaviour.UseAbility(_gameMode.PlayerAbility);
            }
            else
            {
                //use hog ability
                _itemBehaviour.UseAbility(_gameMode.HogAbility);
            }
        }
    }
    private void HandleAttack()
    {
        //check if the item behaviour exists
        if (_itemBehaviour == null)
            return;

        //handle attacking
        if (Input.GetButton(ATTACK))
        {
            _itemBehaviour.Attack();
        }
    }

    private void HandleAbilities()
    {
        //check if the item behaviour exists
        if (_itemBehaviour == null)
            return;

        //handle healing the player
        if (_itemBehaviour.HealValue > 0)
        {
            HealPlayer(_itemBehaviour.HealValue);
        }
        else if (_currentHealDelay > 0.0f)
        {
            _currentHealDelay = 0.0f;
        }

        //handle speed multiplier for player
        if (_movementBehaviour != null)
        {
            _movementBehaviour.SpeedMultiplier = _itemBehaviour.SpeedMultiplier;

            //show/hide trails
            bool isEnabled = (_itemBehaviour.SpeedMultiplier > 1.0f);
            foreach (TrailRenderer trail in _speedTrails)
            {
                if (trail != null)
                {
                    trail.enabled = isEnabled;
                }
            }
        }

        //handle smashing
        if (_smashAnimator != null)
        {
            if (_smashAnimator.GetCurrentAnimatorStateInfo(0).IsName(IDLE_STATE) && _itemBehaviour.IsSmashing)
            {
                if (!_isSmashing)
                {
                    //enable smash attack
                    _smashAnimator.SetTrigger(ATTACK_TRIGGER);
                    _isSmashing = true;
                }
                else
                {
                    if (_canDisableSmash)
                    {
                        //disable smash attack
                        _itemBehaviour.IsSmashing = false;
                        _canDisableSmash = false;
                        _isSmashing = false;
                    }
                }
            }

            if (_smashAnimator.GetCurrentAnimatorStateInfo(0).IsName(WEAPON_ATTACK_STATE) && _itemBehaviour.IsSmashing && _isSmashing)
            {
                _canDisableSmash = true;
            }
        }
    }

    private void HealPlayer(int amount)
    {
        //get health component
        Health playerHealth = GetComponent<Health>();

        if (playerHealth != null)
        {
            //update heal delay
            _currentHealDelay += Time.deltaTime;

            if (_currentHealDelay >= _healDelay)
            {
                //heal the player a little
                playerHealth.Heal(amount);
                _currentHealDelay -= _healDelay;
            }
        }
    }
}
