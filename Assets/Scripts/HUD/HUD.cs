using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    //declare editor parameters
    //health parameters
    [SerializeField] private Image _healthBar = null;
    [SerializeField] private Image _healthBarBackground = null;
    [SerializeField] private Color _healthBarBackgroundStartColor = Color.black;
    [SerializeField] private Color _healthBarBackgroundHealingColor = Color.black;

    //item parameters
    [SerializeField] private Image _activeItem = null;
    [SerializeField] private Image _steeringItem = null;
    [SerializeField] private Image _weaponItem = null;
    [SerializeField] private Image _switchItemDelay = null;

    //player ability parameters
    [SerializeField] private Image _playerAbility = null;
    [SerializeField] private Image _chargedPlayerAbility = null;
    [SerializeField] private Image _chargingPlayerAbility = null;
    [SerializeField] private TextMeshProUGUI _chargingPlayerAbilityTime = null;

    //hog ability parameters
    [SerializeField] private Image _hogAbility = null;
    [SerializeField] private Image _chargedHogAbility = null;
    [SerializeField] private Image _chargingHogAbility = null;
    [SerializeField] private TextMeshProUGUI _chargingHogAbilityTime = null;

    //other ability parameters
    [SerializeField] private Image _inactiveAbility = null;

    //declare script variables
    private Health _playerHealth = null;
    private ItemBehaviour _playerItemBehaviour = null;
    private PlayerCharacter _playerCharacter = null;
    private float _currentHealingColorLerp = 0.0f;
    private bool _hasHealingColor = false;

    private void Start()
    {
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();

        if (player != null)
        {
            //get some needed components
            _playerHealth = player.GetComponent<Health>();
            _playerItemBehaviour = player.GetComponent<ItemBehaviour>();
            _playerCharacter = player.GetComponent<PlayerCharacter>();

            //set ability thumbnails
            if (player.GameMode != null)
            {
                if (player.GameMode.PlayerAbility != null)
                    _playerAbility.sprite = player.GameMode.PlayerAbility._thumbnail;
                
                if (player.GameMode.HogAbility != null)
                    _hogAbility.sprite = player.GameMode.HogAbility._thumbnail;
            }
        }
    }

    private void Update()
    {
        //handle syncing all data
        SyncData();
    }

    private void SyncData()
    {
        SyncHealth();
        SyncHealthBackground();
        SyncActiveItem();
        SyncSwitchItemDelay();
        SyncPlayerAbility();
        SyncHogAbility();
        SyncInactiveAbility();
    }

    private void SyncHealth()
    {
        //health
        if (_healthBar && _playerHealth)
        {
            _healthBar.transform.localScale =
                new Vector3(_playerHealth.HealthPercentage, 1.0f, 1.0f);
        }
    }
    private void SyncHealthBackground()
    {
        //health background
        if (_healthBarBackground && _playerItemBehaviour)
        {
            if (_playerItemBehaviour.HealValue != 0)
            {
                //update healing color lerp value
                if (!_hasHealingColor)
                {
                    //increase lerp value
                    _currentHealingColorLerp += Time.deltaTime;

                    if (_currentHealingColorLerp >= 1.0f)
                    {
                        _currentHealingColorLerp = 1.0f;

                        _hasHealingColor = true;
                    }
                }
                else
                {
                    //decrease lerp value
                    _currentHealingColorLerp -= Time.deltaTime;

                    if (_currentHealingColorLerp <= 0.0f)
                    {
                        _currentHealingColorLerp = 0.0f;

                        _hasHealingColor = false;
                    }
                }

                //apply lerp value
                _healthBarBackground.color = Color.Lerp(_healthBarBackgroundStartColor, _healthBarBackgroundHealingColor, _currentHealingColorLerp);
            }
            else //if not healing
            {
                //set color to start color
                _healthBarBackground.color = _healthBarBackgroundStartColor;
            }
        }
    }
    private void SyncActiveItem()
    {
        //active item
        if (_activeItem && _playerItemBehaviour)
        {
            //handle active item and switch delay positions
            switch (_playerItemBehaviour.CurrentItemType)
            {
                case ItemBehaviour.ItemType.Steering:
                    {
                        if (_steeringItem)
                        {
                            _activeItem.transform.position = _steeringItem.transform.position;

                            if (_switchItemDelay && _weaponItem)
                            {
                                _switchItemDelay.transform.position = _weaponItem.transform.position;
                            }
                        }

                        break;
                    }

                case ItemBehaviour.ItemType.Weapon:
                    {
                        if (_weaponItem)
                        {
                            _activeItem.transform.position = _weaponItem.transform.position;

                            if (_switchItemDelay && _steeringItem)
                            {
                                _switchItemDelay.transform.position = _steeringItem.transform.position;
                            }
                        }

                        break;
                    }
            }
        }
    }
    private void SyncSwitchItemDelay()
    {
        //switch item delay
        if (_switchItemDelay && _playerItemBehaviour)
        {
            _switchItemDelay.transform.localScale =
                new Vector3(1.0f, _playerItemBehaviour.SwitchItemDelayPercentage, 1.0f);
        }
    }
    private void SyncPlayerAbility()
    {
        //player ability
        if (_playerItemBehaviour && _playerCharacter)
        {
            if (_chargedPlayerAbility != null)
            {
                //check if the ability cooldown is over
                if (_playerItemBehaviour.RemainingPlayerAbilityCooldown == 0.0f)
                {
                    //show charged ability border
                    _chargedPlayerAbility.enabled = true;

                    _chargedPlayerAbility.transform.localScale = Vector3.one;
                }
                else //if still on cooldown
                {
                    if (_playerCharacter.GameMode.PlayerAbility != null)
                    {
                        //handle updating remaining duration indicator
                        float durationPercentage = _playerItemBehaviour.RemainingPlayerAbilityDuration / _playerCharacter.GameMode.PlayerAbility._duration;

                        if (durationPercentage > 0.0f)
                        {
                            _chargedPlayerAbility.enabled = true;

                            _chargedPlayerAbility.transform.localScale =
                            new Vector3(1.0f, durationPercentage, 1.0f);
                        }
                        else
                        {
                            _chargedPlayerAbility.enabled = false;
                        }
                    }
                }
            }
            //handle updating charging background and time
            if (_chargingPlayerAbilityTime != null)
            {
                int roundedTime = Mathf.RoundToInt(_playerItemBehaviour.RemainingPlayerAbilityCooldown);

                if (roundedTime > 0)
                {
                    _chargingPlayerAbilityTime.enabled = true;
                    _chargingPlayerAbilityTime.text = roundedTime.ToString();
                }
                else
                {
                    _chargingPlayerAbilityTime.enabled = false;
                }
            }
            if (_chargingPlayerAbility != null)
            {
                if (_playerCharacter.GameMode.PlayerAbility != null)
                {
                    float chargingPercentage = _playerItemBehaviour.RemainingPlayerAbilityCooldown / _playerCharacter.GameMode.PlayerAbility._cooldown;

                    _chargingPlayerAbility.transform.localScale =
                    new Vector3(1.0f, chargingPercentage, 1.0f);
                }
            }
        }
    }
    private void SyncHogAbility()
    {
        //hog ability
        if (_playerItemBehaviour && _playerCharacter)
        {
            if (_chargedHogAbility != null)
            {
                //check if the ability cooldown is over
                if (_playerItemBehaviour.RemainingHogAbilityCooldown == 0.0f)
                {
                    //show charged ability border
                    _chargedHogAbility.enabled = true;

                    _chargedHogAbility.transform.localScale = Vector3.one;
                }
                else //if still on cooldown
                {
                    if (_playerCharacter.GameMode.HogAbility != null)
                    {
                        //handle updating remaining duration indicator
                        float durationPercentage = _playerItemBehaviour.RemainingHogAbilityDuration / _playerCharacter.GameMode.HogAbility._duration;

                        if (durationPercentage > 0.0f)
                        {
                            _chargedHogAbility.enabled = true;

                            _chargedHogAbility.transform.localScale =
                            new Vector3(1.0f, durationPercentage, 1.0f);
                        }
                        else
                        {
                            _chargedHogAbility.enabled = false;
                        }
                    }
                }
            }
            //handle updating charging background and time
            if (_chargingHogAbilityTime != null)
            {
                int roundedTime = Mathf.RoundToInt(_playerItemBehaviour.RemainingHogAbilityCooldown);

                if (roundedTime > 0)
                {
                    _chargingHogAbilityTime.enabled = true;
                    _chargingHogAbilityTime.text = roundedTime.ToString();
                }
                else
                {
                    _chargingHogAbilityTime.enabled = false;
                }
            }
            if (_chargingHogAbility != null)
            {
                if (_playerCharacter.GameMode.HogAbility != null)
                {
                    float chargingPercentage = _playerItemBehaviour.RemainingHogAbilityCooldown / _playerCharacter.GameMode.HogAbility._cooldown;

                    _chargingHogAbility.transform.localScale =
                    new Vector3(1.0f, chargingPercentage, 1.0f);
                }
            }
        }
    }

    private void SyncInactiveAbility()
    {
        //inactive ability
        if (_inactiveAbility && _playerItemBehaviour && _playerCharacter)
        {
            //handle inactive ability position
            switch (_playerItemBehaviour.CurrentItemType)
            {
                case ItemBehaviour.ItemType.Steering:
                    {
                        if (_chargedPlayerAbility)
                            _inactiveAbility.transform.position = _chargedPlayerAbility.transform.position;

                        break;
                    }

                case ItemBehaviour.ItemType.Weapon:
                    {
                        if (_chargedHogAbility)
                            _inactiveAbility.transform.position = _chargedHogAbility.transform.position;

                        break;
                    }
            }
        }
    }
}
