using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCharacter : BasicCharacter
{
    //declare const variables
    private const string BEGIN_AIM_TRIGGER = "BeginAim";
    private const string END_AIM_TRIGGER = "EndAim";
    private const string IDLE_STATE = "Idle";
    private const string AIMING_STATE = "Aiming";
    private const string BUILDING_LAYER = "Building";

    //declare editor parameters
    [SerializeField] private float _attackRange = 2.0f;
    [SerializeField] private GameObject _healthIndicatorTemplate = null;
    [SerializeField] private GameObject _healthIndicatorSocket = null;
    [SerializeField] private GameObject _stunParticleSocket = null;
    [SerializeField] private GameObject _stunParticleTemplate = null;
    [SerializeField] private bool _canMove = true;
    [SerializeField] private bool _canAttack = true;

    //declare editor getters/setters
    public float AttackRange
    {
        get { return _attackRange; }
        set { _attackRange = value; }
    }
    public GameObject StunParticleSocket
    {
        get { return _stunParticleSocket; }
    }
    public GameObject StunParticleTemplate
    {
        get { return _stunParticleTemplate; }
    }

    //declare script variables
    private GameObject _playerTarget = null;
    private GameObject _healthIndicator = null;
    private bool _hasHealthIndicator = true;
    private Health _health = null;
    private bool _isAiming = false;
    private Animator _animator = null;
    private bool _isKnockedBack = false;
    private bool _isStunned = false;
    private float _remainingStunnedTime = 0.0f;

    //declare script getters/setters
    public bool IsKnockedBack
    {
        get { return _isKnockedBack; }
        set { _isKnockedBack = value; }
    }
    public bool IsStunned
    {
        get { return _isStunned; }
    }

    protected override void Awake()
    {
        //execute BasicCharacter's Awake
        base.Awake();

        //handle spawning a health indicator
        if (_healthIndicatorTemplate != null && _healthIndicatorSocket != null)
        {
            //spawn health indicator
            _healthIndicator = Instantiate(_healthIndicatorTemplate,
            _healthIndicatorSocket.transform, true);
            _healthIndicator.transform.localPosition = Vector3.zero;
            _healthIndicator.transform.localRotation = Quaternion.identity;
        }

        //get some needed components
        _health = GetComponent<Health>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        //set player as enemy target
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();

        if (player) _playerTarget = player.gameObject;
    }

    private void Update()
    {
        //ignore updates when the enemy is stunned
        if (!_isStunned)
        {
            //parts of the tutorial level have enemy movement disabled
            if (_canMove)
                HandleMovement();

            //parts of the tutorial level have enemy attacks disabled
            if (_canAttack)
                HandleAttacking();
        }
        else
        {
            //remove the enemy target when stunned
            if (_movementBehaviour != null)
            {
                _movementBehaviour.Target = null;
            }
        }

        //handle updating the health indicator and stun effect
        HandleHealthIndicator();
        HandleIsStunned();
    }

    private void HandleMovement()
    {
        //check if the needed components exist
        if (_movementBehaviour == null)
            return;

        if (_playerTarget == null)
            return;

        //set player as enemy target and look at the player
        _movementBehaviour.Target = _playerTarget;
        _movementBehaviour.DesiredLookatPoint = _playerTarget.transform.position;
    }
    private void HandleAttacking()
    {
        //check if the needed components exist
        if (_itemBehaviour == null)
            return;

        if (_playerTarget == null)
            return;

        //if we are in range of the player, attack the player
        if ((transform.position - _playerTarget.transform.position).sqrMagnitude
            < _attackRange * _attackRange)
        {
            //check if the attack has an animation that needs to play
            if (_animator == null)
            {
                _itemBehaviour.Attack();
            }
            else
            {
                //don't aim or shoot if a building is in the way
                if (Physics.Raycast(transform.position, transform.forward, _attackRange, LayerMask.GetMask(BUILDING_LAYER)))
                    return;
                
                //handle aiming at player
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(IDLE_STATE) && !_isAiming)
                {
                    //handle begin aiming
                    _animator.SetTrigger(BEGIN_AIM_TRIGGER);

                    _isAiming = true;
                }

                //only attack when aiming at the player
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(AIMING_STATE) && _isAiming)
                {
                    _itemBehaviour.Attack(_playerTarget);
                }
            }
        }
        else //if we are not in range of the player
        {
            //check if there is an animation that needs to play
            if (_animator != null)
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName(AIMING_STATE) && _isAiming)
                {
                    //handle end aiming
                    _animator.SetTrigger(END_AIM_TRIGGER);
                
                    _isAiming = false;
                }
            }
        }
    }
    private void HandleHealthIndicator()
    {
        //check if the needed components exist
        if (_health == null)
            return;

        if (_healthIndicator == null)
            return;

        if (!_hasHealthIndicator)
            return;

        //lose health indicator when below half health
        if (_health.HealthPercentage <= 0.5f)
        {
            //detach health indicator from enemy
            _healthIndicator.transform.parent = null;

            //add rigidbody to health indicator
            _healthIndicator.AddComponent<Rigidbody>();

            _healthIndicator.AddComponent<AutoKill>().Init(5.0f);

            _hasHealthIndicator = false;
        }
    }
    private void HandleIsStunned()
    {
        //handle updating the stun effect on the enemy
        if (_isStunned)
        {
            _remainingStunnedTime = Mathf.Max(_remainingStunnedTime - Time.deltaTime, 0.0f);

            if (_remainingStunnedTime == 0.0f)
            {
                _isStunned = false;
            }
        }
    }

    public void StunEnemy(float duration)
    {
        //stun the enemy for certain duration
        _isStunned = true;
        _remainingStunnedTime = duration;
    }
    public void SetHealOrbDropChance(float value)
    {
        //set heal orb drop chance
        if (_health != null)
            _health.HealOrbDropChance = Mathf.Clamp(value, 0.0f, 1.0f);
    }
}
