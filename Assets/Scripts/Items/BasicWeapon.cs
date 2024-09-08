using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeapon : MonoBehaviour
{
    //declare const variables
    protected const string ATTACK_TRIGGER = "Attack";
    protected const string IDLE_STATE = "Idle";
    protected const string WEAPON_ATTACK_STATE = "WeaponAttack";
    const string FRIENDLY_TAG = "Friendly";
    const string ENEMY_TAG = "Enemy";

    //declare editor parameters
    [SerializeField] protected int _damage = 5;
    [SerializeField, Range(1, 5)] private int _maxHitsPerAttack = 1;
    [SerializeField, Range(0.0f, 1.0f)] private float _critChance = 0.5f;
    [SerializeField, Range(1.0f, 2.5f)] private float _critDamage = 1.5f;
    [SerializeField] private AudioSource _attackSound = null;
    [SerializeField] private AudioSource _hitSound = null;

    //declare editor getters/setters
    public virtual int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    //declare script variables
    protected Animator _animator = null;
    protected bool _isAttacking = false;
    protected int _currentAttackHits = 0;
    private bool _isCritting = false;

    //declare ability value variables
    private float _damageMultiplier = 1.0f;
    private int _remainingCritThrows = 0;
    private float _critDamageMultiplier = 1.0f;

    //declare ability value getters/setters
    public float DamageMultiplier
    {
        set { _damageMultiplier = value; }
    }
    public int RemainingCritThrows
    {
        get { return _remainingCritThrows; }
        set { _remainingCritThrows = value; }
    }
    public float CritDamageMultiplier
    {
        set { _critDamageMultiplier = value; }
    }

    private void Awake()
    {
        //get animator component
        _animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        //handle returning to idle state after attacking
        if (_animator != null)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(WEAPON_ATTACK_STATE) && _isAttacking)
            {
                //reset attack variables
                _isAttacking = false;
                _currentAttackHits = 0;

                _isCritting = false;
            }
        }
    }

    public virtual void Attack(GameObject playerTarget = null)
    {
        if (_animator == null)
            return;

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(IDLE_STATE) && !_isAttacking)
        {
            //handle attack
            _animator.SetTrigger(ATTACK_TRIGGER);
            _isAttacking = true;

            //play attack sound
            if (_attackSound != null)
                _attackSound.Play();

            //handle crit throws
            if (_remainingCritThrows > 0)
            {
                _isCritting = true;
                --_remainingCritThrows;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //make sure we only hit friendly or enemies
        if (other.tag != FRIENDLY_TAG && other.tag != ENEMY_TAG)
            return;

        //only hiy the opposing team
        if (other.tag == tag)
            return;

        //ignore hits after max hits has been reached
        if (_currentAttackHits >= _maxHitsPerAttack)
            return;

        //get health component
        Health otherHealth = other.GetComponent<Health>();

        if (otherHealth != null)
        {
            //set initial damage
            int damage = Mathf.RoundToInt(_damage * _damageMultiplier);

            //check if the hit is a critical hit
            if (_isCritting || (Random.Range(0.0f, 1.0f) <= _critChance))
            {
                //if so, add some more damage
                damage = Mathf.RoundToInt(damage * _critDamage * _critDamageMultiplier);
            }

            //apply damage to the object
            otherHealth.Damage(damage);

            //increase amount of htis in current throw
            ++_currentAttackHits;
        }

        //play attack sound
        if (_hitSound != null)
            _hitSound.Play();
    }
}
