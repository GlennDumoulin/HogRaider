using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownHall : MonoBehaviour
{
    //declare editor parameters
    [SerializeField] private GameObject _regularVisuals = null;
    [SerializeField] private GameObject _destroyedVisuals = null;

    //declare script variables
    private Health _health = null;
    private bool _isDestroyed = false;

    //declare script getters/setters
    public bool IsDestroyed
    {
        get { return _isDestroyed; }
    }

    private void Awake()
    {
        //get health component
        _health = GetComponent<Health>();

        //hide destroyed visuals
        if (_destroyedVisuals)
        {
            _destroyedVisuals.SetActive(false);
        }
    }

    private void Update()
    {
        //ignore updates when town hall should be destroyed
        if (_isDestroyed)
            return;

        //check if the building is destroyed
        if (_health.HealthPercentage <= 0.0f)
        {
            //change visuals
            if (_regularVisuals)
                _regularVisuals.SetActive(false);

            if (_destroyedVisuals)
                _destroyedVisuals.SetActive(true);

            //destroy the building
            _isDestroyed = true;

            return;
        }
    }
}
