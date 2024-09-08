using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileDefence : BasicDefence
{
    //declare const variables
    const string RELOAD_DEFENCE = "ReloadDefence";

    //declare editor parameters
    [SerializeField] private GameObject _rotatorSocket = null;
    [SerializeField] private GameObject _projectileSocket = null;
    [SerializeField] private GameObject _projectileTemplate = null;
    [SerializeField] private float _initialRechargeTime = 1.0f;

    //declare script variables
    private GameObject _currentProjectile = null;
    private float _remainingRechargeTime = 0.0f;
    private bool _hasEnteredRange = false;
    private bool _hasAttacked = false;

    protected override void Start()
    {
        //execute the BasicDefence's Start
        base.Start();

        //set initial recharge time
        _remainingRechargeTime = _initialRechargeTime;

        //spawn an initial projectile
        SpawnProjectile();
    }

    private void Update()
    {
        //ignore updates when building should be destroyed
        if (_isDestroyed)
            return;

        //handle health based stuff
        if (_health != null)
        {
            //check if the building is destroyed
            if (_health.HealthPercentage <= 0.0f)
            {
                //change visuals
                ChangeVisuals();

                //destroy the building
                _isDestroyed = true;

                return;
            }

            //handle increasing defence stats
            HandleStats();
        }

        //handle range indicator and attacking player
        if (_playerTarget == null)
            return;

        bool isPlayerInRange = (transform.position - _playerTarget.transform.position).sqrMagnitude
            < _currentAttackRange * _currentAttackRange;

        //handle range indicator
        HandleRangeIndicator(isPlayerInRange);

        //handle attacking player
        if (isPlayerInRange)
        {
            //set first shot recharge time to initial recharge time
            if (!_hasEnteredRange)
            {
                _remainingRechargeTime = _initialRechargeTime;

                _hasEnteredRange = true;
            }

            HandleAttack();
        }
        else
        {
            //reset has entered range variable
            if (_hasEnteredRange)
                _hasEnteredRange = false;
        }
    }

    private void HandleAttack()
    {
        if (!_hasAttacked)
        {
            //aim defence at player
            if (_rotatorSocket == null)
                return;

            _rotatorSocket.transform.LookAt(_playerTarget.transform.position + Vector3.up * 0.75f);
        }

        //handle recharge before next attack
        _remainingRechargeTime -= Time.deltaTime;

        if (_remainingRechargeTime > 0.0f)
            return;

        if (_currentProjectile == null)
            return;

        //detach projectile from weapon
        _currentProjectile.transform.parent = null;

        //fire the projectile
        _currentProjectile.GetComponent<BasicProjectile>().EnableProjectile(_playerTarget);

        //make sure the projectile gets destroyed
        _currentProjectile.AddComponent<AutoKill>().Init(5.0f);

        //stop defence from rotating for a while after shooting
        _hasAttacked = true;

        //reset variables
        _currentProjectile = null;
        _remainingRechargeTime = _currentRechargeTime;
        Invoke(RELOAD_DEFENCE, 1.0f);
    }

    private void SpawnProjectile()
    {
        //check if there is a place where the projectile should be spawned
        if (_projectileSocket == null)
            return;

        //check if there is a template to spawn and that there isn't a projectile already
        if (_projectileTemplate != null && _currentProjectile == null)
        {
            //spawn projectile
            _currentProjectile = Instantiate(_projectileTemplate,
                _projectileSocket.transform, true);
            _currentProjectile.transform.localPosition = Vector3.zero;
            _currentProjectile.transform.localRotation = Quaternion.identity;
        }
    }

    private void ReloadDefence()
    {
        //spawn a new projectile
        SpawnProjectile();

        //enable the defence to rotate towards the player
        _hasAttacked = false;
    }
}
