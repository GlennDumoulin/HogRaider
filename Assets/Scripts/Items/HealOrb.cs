using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOrb : MonoBehaviour
{
    //declare const variables
    const string FRIENDLY_TAG = "Friendly";

    //declare editor parameters
    [SerializeField] private float _minHealValue = 0;
    [SerializeField] private float _maxHealValue = 10;
    [SerializeField] private AudioSource _pickupSound = null;

    //declare script variables
    private bool _isDestroyed = false;
    private bool _isSoundPlaying = false;

    private void Update()
    {
        //ignore updates after the heal orb should be destroyed
        if (_isDestroyed)
            return;

        //destroy object when sound has finished playing
        if (_isSoundPlaying)
        {
            if (_pickupSound != null)
            {
                if (!_pickupSound.isPlaying)
                {
                    _isSoundPlaying = false;
                    _isDestroyed = true;

                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //make sure we only hit friendly
        if (other.tag != FRIENDLY_TAG)
            return;

        //ignore triggers after the heal orb should be destroyed
        if (_isDestroyed)
            return;

        //ignore triggers if sound is playing
        if (_isSoundPlaying)
            return;

        //get health component
        Health otherHealth = other.GetComponent<Health>();

        if (otherHealth != null)
        {
            //add some health to the player
            int healValue = Mathf.RoundToInt(Random.Range(_minHealValue, _maxHealValue));

            otherHealth.Heal(healValue);

            //play sound effect
            if (_pickupSound != null)
            {
                _pickupSound.Play();
                _isSoundPlaying = true;

                //disable the heal orb itself
                MeshRenderer meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
                ParticleSystem particles = gameObject.GetComponentInChildren<ParticleSystem>();

                if (meshRenderer != null)
                {
                    meshRenderer.enabled = false;
                }
                if (particles != null)
                {
                    particles.Stop();
                }
            }
            else
            {
                //if there is no sound effect, just destroy the heal orb
                Destroy(gameObject);
            }
        }
    }
}
