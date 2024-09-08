using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour
{
    //declare editor parameters
    [SerializeField] private Health _healthComponent = null;
    [SerializeField] private ParticleSystem _fireParticles = null;

    //declare script variables
    private bool _isBurning = false;
    private bool _isDestroyed = false;

    private void Update()
    {
        //ignore updates when walls should be destroyed
        if (_isDestroyed)
            return;

        //check if there is a health component
        if (_healthComponent == null)
            return;

        //check if the walls are burning
        if (!_isBurning)
        {
            //handle activating fire particles when below half health
            if (_healthComponent.HealthPercentage <= 0.5f)
            {
                if (_fireParticles != null)
                {
                    _fireParticles.Play();

                    _isBurning = true;
                }
            }
        }

        //handle destroying the walls
        if (_healthComponent.HealthPercentage <= 0.0f)
        {
            _isDestroyed = true;

            Destroy(gameObject);
        }
    }
}
