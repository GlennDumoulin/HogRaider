using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDefence : MonoBehaviour
{
    //declare editor parameters
    [SerializeField] private int _minDamage = 5;
    [SerializeField] private int _maxDamage = 10;
    [SerializeField] private float _minRechargeTime = 0.0f;
    [SerializeField] private float _maxRechargeTime = 2.0f;
    [SerializeField] private float _minAttackRange = 20.0f;
    [SerializeField] private float _maxAttackRange = 40.0f;
    [SerializeField] private GameObject _regularVisuals = null;
    [SerializeField] private GameObject _destroyedVisuals = null;
    [SerializeField] private GameObject _rangeIndicator = null;

    //declare script variables
    protected Health _health = null;
    protected GameObject _playerTarget = null;
    protected int _currentDamage = 0;
    protected float _currentRechargeTime = 0.0f;
    protected float _currentAttackRange = 0.0f;
    protected bool _isDestroyed = false;

    //declare script getters/setters
    public bool IsDestroyed
    {
        get { return _isDestroyed; }
    }

    private void Awake()
    {
        //set initial values
        _health = GetComponent<Health>();
        _currentDamage = _minDamage;
        _currentRechargeTime = _maxRechargeTime;
        _currentAttackRange = _minAttackRange;

        //hide destroyed visuals
        if (_destroyedVisuals)
            _destroyedVisuals.SetActive(false);
    }

    protected virtual void Start()
    {
        //set player as defence target
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        if (player) _playerTarget = player.gameObject;
    }

    protected virtual void HandleStats()
    {
        //we can't handle updating the stats if there is no health component
        if (_health == null)
            return;

        //handle increasing damage based on remaining health
        _currentDamage = _minDamage + Mathf.RoundToInt((_maxDamage - _minDamage) * (1 - _health.HealthPercentage));
        if (_currentDamage < _minDamage) _currentDamage = _minDamage;

        //handle decreasing recharge time based on remaining health
        _currentRechargeTime = _minRechargeTime + ((_maxRechargeTime - _minRechargeTime) * _health.HealthPercentage);
        if (_currentRechargeTime < _minRechargeTime) _currentRechargeTime = _minRechargeTime;

        //handle increasing attack range based on remaining health
        _currentAttackRange = _minAttackRange + ((_maxAttackRange - _minAttackRange) * (1 - _health.HealthPercentage));
        if (_currentAttackRange < _minAttackRange) _currentAttackRange = _minAttackRange;
    }

    protected void HandleRangeIndicator(bool isActive)
    {
        //handle showing range indicator
        if (_rangeIndicator == null)
            return;

        //if player is in range, show range indicator
        if (isActive)
        {
            Vector3 rangeIndicatorScale = _rangeIndicator.transform.localScale;
            rangeIndicatorScale.x = _currentAttackRange * 2;
            rangeIndicatorScale.z = _currentAttackRange * 2;
                
            _rangeIndicator.transform.localScale = rangeIndicatorScale;
        }

        _rangeIndicator.SetActive(isActive);
    }

    protected void ChangeVisuals()
    {
        //change visuals
        if (_regularVisuals)
            _regularVisuals.SetActive(false);

        if (_destroyedVisuals)
            _destroyedVisuals.SetActive(true);
    }
}
