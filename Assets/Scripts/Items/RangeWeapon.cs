using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : BasicWeapon
{
    //declare editor parameters
    [SerializeField] private GameObject _projectileTemplate = null;
    [SerializeField] private GameObject _projectileSocket = null;
    [SerializeField, Range(0.0f, 5.0f)] private float _rechargeTime = 1.0f;

    //declare editor getters/setters
    public float RechargeTime
    {
        get { return _rechargeTime; }
        set { _rechargeTime = value; }
    }

    //declare script variables
    private GameObject _currentProjectile = null;
    private float _currentRechargeTime = 0.0f;
    private float _projectileLifeTime = 5.0f;

    //declare other getters/setters
    public override int Damage
    {
        get
        {
            if (_currentProjectile)
            {
                BasicProjectile projectile = _currentProjectile.GetComponent<BasicProjectile>();

                if (projectile)
                {
                    return projectile.Damage;
                }
            }

            return _damage;
        }
        set
        {
            if (_currentProjectile)
            {
                BasicProjectile projectile = _currentProjectile.GetComponent<BasicProjectile>();

                if (projectile)
                {
                    projectile.Damage = value;
                }
            }

            _damage = value;
        }
    }

    private void Start()
    {
        //spawn an initial projectile
        SpawnProjectile();
    }

    protected override void Update()
    {
        if (_animator != null)
        {
            //handle going back to idle state after attacking
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(WEAPON_ATTACK_STATE) && _isAttacking)
            {
                //reset variables
                _isAttacking = false;

                //spawn new projectile
                SpawnProjectile();
            }
        }
    }

    public override void Attack(GameObject playerTarget)
    {
        if (_animator == null)
            return;

        //handle attacking and attack animation
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(IDLE_STATE) && !_isAttacking)
        {
            //handle recharge before next attack
            _currentRechargeTime += Time.deltaTime;

            if (_currentRechargeTime < _rechargeTime)
                return;

            //handle attack
            _animator.SetTrigger(ATTACK_TRIGGER);
            _isAttacking = true;

            if (_currentProjectile == null)
                return;

            //detach projectile from weapon
            _currentProjectile.transform.parent = null;

            //fire the projectile
            _currentProjectile.GetComponent<BasicProjectile>().EnableProjectile(playerTarget);

            //make sure the projectile gets destroyed
            _currentProjectile.AddComponent<AutoKill>().Init(_projectileLifeTime);

            //reset variables
            _currentProjectile = null;
            _currentAttackHits = 0;
            _currentRechargeTime = 0.0f;
        }
    }

    private void SpawnProjectile()
    {
        //check if there is a place to spawn the projectile
        if (_projectileSocket == null)
            return;

        //check if there is a template that should be spawned and that there isn't a projectile already
        if (_projectileTemplate != null && _currentProjectile == null)
        {
            //spawn projectile
            _currentProjectile = Instantiate(_projectileTemplate,
                _projectileSocket.transform, true);
            _currentProjectile.transform.localPosition = Vector3.zero;
            _currentProjectile.transform.localRotation = Quaternion.identity;
        }
    }
}
