using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    //declare const variables
    const string FORWARD_MOVEMENT = "ForwardMovement";
    const string ENEMY_LAYER = "Enemy";

    //declare enum types
    public enum ItemType
    {
        Weapon,
        Steering,
        None,
    }

    //declare editor parameters
    [SerializeField] private GameObject _weaponTemplate = null;
    //[SerializeField] private GameObject _playerAbilityTemplate = null;
    //[SerializeField] private GameObject _hogAbilityTemplate = null;
    [SerializeField] private GameObject _steeringTemplate = null;
    [SerializeField] private GameObject _itemSocket = null;
    [SerializeField] private ItemType _currentItemType = ItemType.None;
    [SerializeField] private float _switchItemDelay = 1.0f;
    [SerializeField, Range(0, 10)] private int _knockbackDamage = 0;
    [SerializeField, Range(0.0f, 5.0f)] private float _knockbackStrengthMultiplier = 0.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float _stunChance = 0.0f;
    [SerializeField, Range(1.0f, 10.0f)] private float _stunDuration = 1.0f;
    [SerializeField] private AudioSource _useAbilitySound = null;
    [SerializeField] private AudioSource _abilityOnCooldownSound = null;
    [SerializeField] private AudioSource _smashSound = null;
    [SerializeField] private AudioSource _hitSound = null;

    //declare script variables
    private GameObject _currentItem = null;
    private float _currentSwitchItemDelay = 0.0f;
    private bool _hasSwitchedItem = true;
    private BasicWeapon _weapon = null;
    private float _knockbackStrength = 20.0f;

    //declare script getters/setters
    public ItemType CurrentItemType
    {
        get { return _currentItemType; }
        set { _currentItemType = value; }
    }

    //declare other getters/setters
    public float SwitchItemDelayPercentage
    {
        get
        {
            return (_hasSwitchedItem) ? (_switchItemDelay - _currentSwitchItemDelay) / _switchItemDelay : 0.0f;
        }
    }

    //declare ability variables
    private float _remainingPlayerAbilityDuration = 0.0f;
    private float _remainingPlayerAbilityCooldown = 5.0f;
    private BasicAbility.AbilityTarget _playerAbilityTarget = BasicAbility.AbilityTarget.Damage;
    private float _remainingHogAbilityDuration = 0.0f;
    private float _remainingHogAbilityCooldown = 10.0f;
    private BasicAbility.AbilityTarget _hogAbilityTarget = BasicAbility.AbilityTarget.Smash;

    //declare ability getters/setters
    public float RemainingPlayerAbilityDuration
    {
        get { return _remainingPlayerAbilityDuration; }
    }
    public float RemainingPlayerAbilityCooldown
    {
        get { return _remainingPlayerAbilityCooldown; }
    }
    public float RemainingHogAbilityDuration
    {
        get { return _remainingHogAbilityDuration; }
    }
    public float RemainingHogAbilityCooldown
    {
        get { return _remainingHogAbilityCooldown; }
    }

    //declare ability value parameters
    private float _damageMultiplier = 1.0f;
    private int _remainingCritThrows = 0;
    private float _critDamageMultiplier = 1.0f;
    private int _healValue = 0;
    private bool _isSmashing = false;
    private bool _isStunning = false;
    private float _speedMultiplier = 1.0f;

    //declare ability value getters/setters
    public int HealValue
    {
        get { return _healValue; }
    }
    public bool IsSmashing
    {
        get { return _isSmashing; }
        set { _isSmashing = value; }
    }
    public float SpeedMultiplier
    {
        get { return _speedMultiplier; }
    }

    private void Awake()
    {
        //handle spawning initial item
        switch (_currentItemType)
        {
            //initial item is weapon type
            case ItemType.Weapon:
                {
                    if (_weaponTemplate != null && _itemSocket != null)
                    {
                        //spawn weapon item
                        _currentItem = Instantiate(_weaponTemplate,
                        _itemSocket.transform, true);
                        _currentItem.transform.localPosition = Vector3.zero;
                        _currentItem.transform.localRotation = Quaternion.identity;

                        //set weapon
                        _weapon = _currentItem.GetComponent<BasicWeapon>();

                        //set item type
                        _currentItemType = ItemType.Weapon;
                    }

                    break;
                }

            //initial item is steering type
            case ItemType.Steering:
                {
                    if (_steeringTemplate != null && _itemSocket != null)
                    {
                        //spawn steering item
                        _currentItem = Instantiate(_steeringTemplate,
                        _itemSocket.transform, true);
                        _currentItem.transform.localPosition = Vector3.zero;
                        _currentItem.transform.localRotation = Quaternion.identity;

                        //set item type
                        _currentItemType = ItemType.Steering;
                    }

                    break;
                }

            //initial item is unknown type
            default:
                break;
        }

        //set switch item delay
        _currentSwitchItemDelay = _switchItemDelay;
    }

    private void Update()
    {
        //update delay between switching items
        if (_hasSwitchedItem)
        {
            _currentSwitchItemDelay += Time.deltaTime;

            if (_currentSwitchItemDelay >= _switchItemDelay)
            {
                //reset switch item variables
                _currentSwitchItemDelay = 0.0f;
                _hasSwitchedItem = false;
            }
        }

        //update player ability duration and cooldown
        UpdatePlayerAbility();

        //update hog ability duration and cooldown
        UpdateHogAbility();
    }
    private void UpdatePlayerAbility()
    {
        //update remaining duration
        if (_remainingPlayerAbilityDuration > 0.0f)
            _remainingPlayerAbilityDuration = Mathf.Max(_remainingPlayerAbilityDuration - Time.deltaTime, 0.0f);

        //handle disabling ability when duration is over
        if (_remainingPlayerAbilityDuration == 0.0f)
            DisableAbility(_playerAbilityTarget);

        //update remaining cooldown
        if (_remainingPlayerAbilityCooldown > 0.0f)
            _remainingPlayerAbilityCooldown = Mathf.Max(_remainingPlayerAbilityCooldown - Time.deltaTime, 0.0f);
    }
    private void UpdateHogAbility()
    {
        //update remaining duration
        if (_remainingHogAbilityDuration > 0.0f)
            _remainingHogAbilityDuration = Mathf.Max(_remainingHogAbilityDuration - Time.deltaTime, 0.0f);

        //handle disabling ability when duration is over
        if (_remainingHogAbilityDuration == 0.0f)
            DisableAbility(_hogAbilityTarget);

        //update remaining cooldown
        if (_remainingHogAbilityCooldown > 0.0f)
            _remainingHogAbilityCooldown = Mathf.Max(_remainingHogAbilityCooldown - Time.deltaTime, 0.0f);

    }

    public bool SwitchItem()
    {
        //ignore switch item inputs when switch item delay isn't over
        if (!_hasSwitchedItem)
        {
            //handle switching the item
            switch (_currentItemType)
            {
                //handle switching from weapon type to steering type
                case ItemType.Weapon:
                    {
                        //save remaining crit throws
                        _remainingCritThrows = _weapon.RemainingCritThrows;

                        //destroy previous item and remove reference to previous weapon item
                        Destroy(_currentItem);
                        _weapon = null;

                        //spawn steering item
                        _currentItem = Instantiate(_steeringTemplate,
                        _itemSocket.transform, true);
                        _currentItem.transform.localPosition = Vector3.zero;
                        _currentItem.transform.localRotation = Quaternion.identity;

                        //set item type
                        _currentItemType = ItemType.Steering;

                        //set has switched item to true
                        _hasSwitchedItem = true;

                        break;
                    }

                //handle switching from steering type to weapon type
                case ItemType.Steering:
                    {
                        //destroy previous item
                        Destroy(_currentItem);

                        //spawn weapon item
                        _currentItem = Instantiate(_weaponTemplate,
                        _itemSocket.transform, true);
                        _currentItem.transform.localPosition = Vector3.zero;
                        _currentItem.transform.localRotation = Quaternion.identity;

                        //set weapon data
                        _weapon = _currentItem.GetComponent<BasicWeapon>();
                        _weapon.DamageMultiplier = _damageMultiplier;
                        _weapon.RemainingCritThrows = _remainingCritThrows;
                        _weapon.CritDamageMultiplier = _critDamageMultiplier;

                        //set item type
                        _currentItemType = ItemType.Weapon;

                        //set has switched item to true
                        _hasSwitchedItem = true;

                        break;
                    }

                //handle switching if current item type is unknown
                default:
                    break;
            }

            return _hasSwitchedItem;
        }

        return false;
    }

    public void Attack(GameObject playerTarget = null)
    {
        //handle attacking
        if (_weapon != null)
            _weapon.Attack(playerTarget);
    }

    public void UseAbility(BasicAbility ability)
    {
        //handle using ability
        switch (ability._type)
        {
            //handle using player ability
            case BasicAbility.AbilityType.Player:
                {
                    //ignore if ability is still on cooldown
                    if (_remainingPlayerAbilityCooldown > 0.0f)
                    {
                        //play ability on cooldown sound
                        if (_abilityOnCooldownSound != null)
                        {
                            if (!_abilityOnCooldownSound.isPlaying)
                            {
                                _abilityOnCooldownSound.Play();
                            }
                        }

                        return;
                    }

                    //set ability duration and cooldown
                    if (ability._duration != -1)
                    {
                        _remainingPlayerAbilityDuration = ability._duration;
                    }
                    _remainingPlayerAbilityCooldown = ability._cooldown;

                    //play use ability sound
                    if (_useAbilitySound != null)
                        _useAbilitySound.Play();

                    //enable ability
                    _playerAbilityTarget = ability._target;
                    EnableAbility(ability._target);

                    break;
                }

            //handle using hog ability
            case BasicAbility.AbilityType.Hog:
                {
                    //ignore if ability is still on cooldown
                    if (_remainingHogAbilityCooldown > 0.0f)
                    {
                        //play ability on cooldown sound
                        if (_abilityOnCooldownSound != null)
                        {
                            if (!_abilityOnCooldownSound.isPlaying)
                            {
                                _abilityOnCooldownSound.Play();
                            }
                        }

                        return;
                    }

                    //set ability duration and cooldown
                    if (ability._duration != -1)
                    {
                        _remainingHogAbilityDuration = ability._duration;
                    }
                    _remainingHogAbilityCooldown = ability._cooldown;

                    //play use ability sound
                    if (_useAbilitySound != null)
                        _useAbilitySound.Play();

                    //enable ability
                    _hogAbilityTarget = ability._target;
                    EnableAbility(ability._target);

                    break;
                }

            //handle using ability if type is unknown
            default:
                break;
        }
    }

    private void EnableAbility(BasicAbility.AbilityTarget abilityTarget)
    {
        //handle enabling ability based on what value it target's
        switch (abilityTarget)
        {
            case BasicAbility.AbilityTarget.Damage:
                {
                    _damageMultiplier = 1.5f;

                    if (_weapon != null)
                        _weapon.DamageMultiplier = 1.5f;

                    break;
                }

            case BasicAbility.AbilityTarget.Health:
                {
                    _healValue = 5;

                    break;
                }

            case BasicAbility.AbilityTarget.CritChance:
                {
                    _remainingCritThrows = 3;

                    if (_weapon != null)
                        _weapon.RemainingCritThrows = 3;

                    break;
                }

            case BasicAbility.AbilityTarget.CritDamage:
                {
                    _critDamageMultiplier = 2.0f;

                    if (_weapon != null)
                        _weapon.CritDamageMultiplier = 2.0f;

                    break;
                }

            case BasicAbility.AbilityTarget.Smash:
                {
                    _isSmashing = true;

                    //play smash sound
                    if (_smashSound != null)
                        _smashSound.Play();

                    break;
                }

            case BasicAbility.AbilityTarget.Stun:
                {
                    _isStunning = true;

                    break;
                }

            case BasicAbility.AbilityTarget.Speed:
                {
                    _speedMultiplier = 1.5f;

                    break;
                }

            //handle enabling ability if target value is unknown
            default:
                break;
        }
    }
    private void DisableAbility(BasicAbility.AbilityTarget abilityTarget)
    {
        //handle disabling ability based on what value it target's
        switch (abilityTarget)
        {
            case BasicAbility.AbilityTarget.Damage:
                {
                    _damageMultiplier = 1.0f;

                    if (_weapon != null)
                        _weapon.DamageMultiplier = 1.0f;

                    break;
                }

            case BasicAbility.AbilityTarget.Health:
                {
                    _healValue = 0;

                    break;
                }

            case BasicAbility.AbilityTarget.CritDamage:
                {
                    _critDamageMultiplier = 1.0f;

                    if (_weapon != null)
                        _weapon.CritDamageMultiplier = 1.0f;

                    break;
                }

            case BasicAbility.AbilityTarget.Stun:
                {
                    _isStunning = false;

                    break;
                }

            case BasicAbility.AbilityTarget.Speed:
                {
                    _speedMultiplier = 1.0f;

                    break;
                }

            //handle disabling ability if target value is unknown
            default:
                break;
        }
    }

    private void KnockBack(Rigidbody target)
    {
        //handle knocking back the enemy
        if (target != null)
        {
            //calculate the direction in which the enemy should be knocked back
            Vector3 direction = target.position - transform.position;

            direction = direction.normalized * _knockbackStrength * _knockbackStrengthMultiplier;

            //apply the knockback to the enemy
            target.AddForce(direction, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //ignore if the collider somehow doesn't exist anymore
        if (other == null)
            return;

        //only trigger when holding the steering item
        if (_currentItemType != ItemType.Steering)
            return;

        //only continue if the player is moving forwards or smashing
        float forwardMovement = Input.GetAxis(FORWARD_MOVEMENT);

        if (forwardMovement <= 0.0f && !_isSmashing)
            return;

        //make sure we only hit enemies
        if (other.gameObject.layer != LayerMask.NameToLayer(ENEMY_LAYER))
            return;

        //get enemy character
        EnemyCharacter enemy = other.GetComponent<EnemyCharacter>();

        if (enemy == null)
            return;

        //ignore if enemy has already been knocked back
        if (enemy.IsKnockedBack)
            return;

        //handle damaging enemy
        Health otherHealth = other.GetComponent<Health>();

        if (otherHealth != null)
        {
            //set default damage
            int damage = _knockbackDamage;

            //add some more damage if the enemy got hit by smash attack
            if (_isSmashing)
            {
                PlayerCharacter player = gameObject.GetComponent<PlayerCharacter>();

                if (player)
                {
                    damage += player.SmashDamage;
                }
            }

            //apply damage to the enemy
            otherHealth.Damage(damage);
        }

        //check if the enemy is already stunned
        if (!enemy.IsStunned) //if not stunned, handle knockback and possible stun
        {
            //handle knocking back enemy
            KnockBack(other.GetComponent<Rigidbody>());
            enemy.IsKnockedBack = true;

            //handle stunning enemy
            if (_isStunning || (Random.Range(0.0f, 1.0f) <= _stunChance))
            {
                enemy.StunEnemy(_stunDuration);

                if (enemy.StunParticleSocket != null && enemy.StunParticleTemplate != null)
                {
                    GameObject stunParticle = Instantiate(enemy.StunParticleTemplate,
                        enemy.StunParticleSocket.transform, true);
                    stunParticle.transform.localPosition = Vector3.zero;

                    stunParticle.AddComponent<AutoKill>().Init(_stunDuration);
                }
            }

            //play hit sound
            if (_hitSound != null)
                _hitSound.Play();
        }
        else //if stunned, kill the enemy
        {
            otherHealth.Kill();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //make sure we only handle enemies
        if (other.gameObject.layer != LayerMask.NameToLayer(ENEMY_LAYER))
            return;

        //get enemy character
        EnemyCharacter enemy = other.GetComponent<EnemyCharacter>();

        if (enemy != null)
        {
            //reset is knocked back state
            if (enemy.IsKnockedBack)
            {
                enemy.IsKnockedBack = false;
            }
        }
    }
}
