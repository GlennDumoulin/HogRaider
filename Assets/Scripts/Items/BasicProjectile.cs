using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    //declare const variables
    static readonly string[] RAYCAST_MASK = { "StaticLevel", "DynamicLevel" };
    const string FRIENDLY_TAG = "Friendly";
    const string ENEMY_TAG = "Enemy";

    //declare editor parameters
    [SerializeField, Range(1.0f, 30.0f)] private float _speed = 20.0f;
    [SerializeField] private int _damage = 5;
    [SerializeField, Range(0, 2)] private int _maxEnemiesPierced = 0;
    [SerializeField, Range(0.0f, 1.0f)] private float _critChance = 0.5f;
    [SerializeField, Range(1.0f, 2.5f)] private float _critDamage = 1.5f;
    [SerializeField] private TrailRenderer _trailRenderer = null;
    [SerializeField] private Collider _collider = null;
    [SerializeField] private AudioSource _fireSound = null;

    //declare editor getters/setters
    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    //declare script variables
    private int _currentEnemiesPierced = -1;
    private bool _isFired = false;
    private bool _isEnabled = false;
    private bool _isDestroyed = false;

    //declare script getters/setters
    public bool IsFired
    {
        get { return _isFired; }
        set { _isFired = value; }
    }

    private void Start()
    {
        //disable trail on spawn
        if (_trailRenderer)
            _trailRenderer.enabled = false;
    }

    private void FixedUpdate()
    {
        //ipnore updates if the projectile is not fired or if it should be destroyed
        if (!_isFired)
            return;

        if (_isDestroyed)
            return;

        //update projectile position
        if (!WallDetection())
            transform.position += transform.forward * Time.fixedDeltaTime * _speed;
    }

    private bool WallDetection()
    {
        //check if the projectile is hitting an object that is part of the RAYCAST_MASK
        Ray collisionRay = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(collisionRay,
            Time.fixedDeltaTime * _speed, LayerMask.GetMask(RAYCAST_MASK)))
        {
            //if so, destroy the projectile
            _isDestroyed = true;

            Destroy(gameObject);
            return true;
        }
        return false;
    }

    public void EnableProjectile(GameObject _playerTarget)
    {
        //enable projectile
        if (!_isEnabled)
        {
            //enable trail when fired
            if (_trailRenderer)
                _trailRenderer.enabled = true;

            //enable collider when fired
            if (_collider)
                _collider.enabled = true;

            //aim arrow towards player
            transform.LookAt(_playerTarget.transform.position + Vector3.up * 1.5f);

            //fire the projectile
            _isFired = true;

            //play fire sound
            if (_fireSound != null)
                _fireSound.Play();
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

        //ignore triggers after the projectile should be destroyed
        if (_isDestroyed)
            return;

        //get health component
        Health otherHealth = other.GetComponent<Health>();

        if(otherHealth != null)
        {
            //set initial damage
            int damage = _damage;

            //check if the hit is a critical hit
            if (Random.Range(0.0f, 1.0f) <= _critChance)
            {
                //if so, add some more damage
                damage = Mathf.RoundToInt(damage * _critDamage);
            }

            //apply damage to object
            otherHealth.Damage(damage);

            //increase hit counter and check if the projectile should be destroyed
            if (++_currentEnemiesPierced >= _maxEnemiesPierced)
            {
                _isDestroyed = true;

                Destroy(gameObject);
            }
        }
    }
}
