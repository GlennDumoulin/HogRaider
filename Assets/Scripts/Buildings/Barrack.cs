using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrack : MonoBehaviour
{
    //declare editor parameters
    [SerializeField] private GameObject _spawnTemplate = null;
    [SerializeField] private GameObject _spawnPoint = null;
    [SerializeField] private float _minWaveFrequency = 5.0f;
    [SerializeField] private float _maxWaveFrequency = 15.0f;
    [SerializeField] private int _minSpawnsPerWave = 1;
    [SerializeField] private int _maxSpawnsPerWave = 3;
    [SerializeField] private GameObject _regularVisuals = null;
    [SerializeField] private GameObject _destroyedVisuals = null;
    [SerializeField] private GameObject _spawnParent = null;

    //declare script variables
    private Health _health = null;
    private float _nextHealthStage = 1.0f;
    private float _heathStageStepSize = 0.2f;
    private float _currentWaveFrequency = 0.0f;
    private int _currentSpawnsPerWave = 0;
    private float _timeTillNextWave = 0.0f;
    private bool _isDestroyed = false;

    //declare script getters/setters
    public bool IsDestroyed
    {
        get { return _isDestroyed; }
    }

    private void Awake()
    {
        //set initial values
        _health = GetComponent<Health>();
        _nextHealthStage -= _heathStageStepSize;
        _currentWaveFrequency = _maxWaveFrequency;
        _currentSpawnsPerWave = _minSpawnsPerWave;
        _timeTillNextWave = _minWaveFrequency;

        //hide destroyed visuals
        if (_destroyedVisuals)
        {
            _destroyedVisuals.SetActive(false);
        }
    }

    private void Update()
    {
        //ignore updates when barrack should be destroyed
        if (_isDestroyed)
            return;

        //handle health based stuff
        if (_health != null)
        {
            //check if the building is destroyed
            if (_health.HealthPercentage <= 0.0f)
            {
                //spawn barbarians
                SpawnWave();

                //change visuals
                if (_regularVisuals)
                    _regularVisuals.SetActive(false);

                if (_destroyedVisuals)
                    _destroyedVisuals.SetActive(true);

                //destroy the building
                _isDestroyed = true;

                return;
            }

            //check if the building reached next health stage
            if (_health.HealthPercentage <= _nextHealthStage)
            {
                //spawn barbarians
                SpawnWave();

                //set next health stage
                _nextHealthStage -= _heathStageStepSize;
            }

            //handle increasing nr of spawns based on remaining health
            _currentSpawnsPerWave = _minSpawnsPerWave + Mathf.RoundToInt((_maxSpawnsPerWave - _minSpawnsPerWave) * (1 - _health.HealthPercentage));
            if (_currentSpawnsPerWave < _minSpawnsPerWave) _currentSpawnsPerWave = _minSpawnsPerWave;

            //handle increasing wave frequency based on remaining health
            _currentWaveFrequency = _minWaveFrequency + ((_maxWaveFrequency - _minWaveFrequency) * _health.HealthPercentage);
            if (_currentWaveFrequency < _minWaveFrequency) _currentWaveFrequency = _minWaveFrequency;
            if (_currentWaveFrequency < _timeTillNextWave) _timeTillNextWave = _currentWaveFrequency;
        }

        //handle spawning the next wave
        _timeTillNextWave -= Time.deltaTime;

        if (_timeTillNextWave <= 0.0f)
        {
            _timeTillNextWave = _currentWaveFrequency;

            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        //check if there is a template that should be spawned
        if (_spawnTemplate == null)
            return;

        //check if there is a point where they should be spawned
        if (_spawnPoint == null)
            return;

        //handle spawning the desired amount of enemies
        for (int i = 0; i < _currentSpawnsPerWave; ++i)
        {
            //spawn an enemy
            GameObject spawnedObject = Instantiate(_spawnTemplate,
                _spawnPoint.transform.position, _spawnPoint.transform.rotation);

            //detach enemy from barrack
            spawnedObject.transform.parent = (_spawnParent != null) ? _spawnParent.transform : null;

            EnemyCharacter enemy = spawnedObject.GetComponent<EnemyCharacter>();

            //set heal orb drop chance to the barrack's drop chance
            if (enemy != null && _health != null)
                enemy.SetHealOrbDropChance(_health.HealOrbDropChance);
        }
    }
}
