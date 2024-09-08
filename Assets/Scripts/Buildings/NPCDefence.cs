using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDefence : BasicDefence
{
    //declare editor parameters
    [SerializeField] private GameObject _spawnTemplate = null;
    [SerializeField] private List<GameObject> _spawnPoints = null;
    [SerializeField] private GameObject _enemy = null;
    [SerializeField] private GameObject _weaponSocket = null;
    [SerializeField] private GameObject _spawnParent = null;

    //declare script variables
    private EnemyCharacter _enemyCharacter = null;
    private RangeWeapon _weapon = null;

    protected override void Start()
    {
        //execute the BasicDefence's Start
        base.Start();

        //get some needed components
        if (_enemy) _enemyCharacter = _enemy.GetComponent<EnemyCharacter>();
        if (_weaponSocket) _weapon = _weaponSocket.GetComponentInChildren<RangeWeapon>();
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
                //spawn troops
                SpawnTroops();

                //change visuals
                ChangeVisuals();

                //destroy the building
                _isDestroyed = true;

                return;
            }

            //handle increasing defence stats
            HandleStats();
        }

        //handle range indicator
        if (_playerTarget == null)
            return;

        if (_enemy == null)
            return;

        bool isPlayerInRange = (_enemy.transform.position - _playerTarget.transform.position).sqrMagnitude
            < _currentAttackRange * _currentAttackRange;

        HandleRangeIndicator(isPlayerInRange);
    }

    protected override void HandleStats()
    {
        //execute increasing BasicDefence stats
        base.HandleStats();

        //apply stats to weapon and enemy
        if (_weapon)
        {
            _weapon.Damage = _currentDamage;
            _weapon.RechargeTime = _currentRechargeTime;
        }

        if (_enemyCharacter)
            _enemyCharacter.AttackRange = _currentAttackRange;
    }

    private void SpawnTroops()
    {
        //check if there is a template that should be spawned
        if (_spawnTemplate == null)
            return;

        //spawn a troop at every spawn point
        for (int i = 0; i < _spawnPoints.Count; ++i)
        {
            //check if the spawn point still exists
            if (_spawnPoints[i] == null)
                continue;

            //spawn an enemy
            GameObject spawnedObject = Instantiate(_spawnTemplate,
                _spawnPoints[i].transform, true);
            spawnedObject.transform.localPosition = Vector3.zero;
            spawnedObject.transform.localRotation = Quaternion.identity;

            //detach enemy from defence
            spawnedObject.transform.parent = (_spawnParent != null) ? _spawnParent.transform : null;
        }
    }
}
