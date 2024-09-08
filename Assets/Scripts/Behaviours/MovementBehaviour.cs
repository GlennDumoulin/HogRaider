using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBehaviour : MonoBehaviour
{
    //declare editor parameters
    [SerializeField] protected float _movementSpeed = 1.0f;
    [SerializeField] protected float _rotationSpeed = 1.0f;

    //declare script variables
    protected Rigidbody _rigidBody = null;
    private float _speedMultiplier = 1.0f;
    protected Vector3 _desiredMovementDirection = Vector3.zero;
    protected Vector3 _desiredLookatPoint = Vector3.zero;
    protected GameObject _target;

    //declare script getters/setters
    public float SpeedMultiplier
    {
        set { _speedMultiplier = value; }
    }
    public Vector3 DesiredMovementDirection
    {
        get { return _desiredMovementDirection; }
        set { _desiredMovementDirection = value; }
    }
    public Vector3 DesiredLookatPoint
    {
        get { return _desiredLookatPoint; }
        set { _desiredLookatPoint = value; }
    }
    public GameObject Target
    {
        get { return _target; }
        set { _target = value; }
    }

    protected virtual void Awake()
    {
        //get rigidbody component
        _rigidBody = GetComponent<Rigidbody>();
    }

    protected virtual void FixedUpdate()
    {
        //handle movement and rotation
        HandleMovement();
        HandleRotation();
    }

    protected virtual void HandleMovement()
    {
        //calculate movement based on direction and speed
        Vector3 movement = _desiredMovementDirection.normalized;
        movement *= _movementSpeed * _speedMultiplier;
        movement.y = _rigidBody.velocity.y;

        //apply movement to rigidbody
        _rigidBody.velocity = movement;
    }
    protected virtual void HandleRotation()
    {
        //calculate rotation based on look at point and speed
        Vector3 rotation = _desiredLookatPoint.normalized;
        rotation *= _rotationSpeed * Time.fixedDeltaTime;

        //apply rotation to object
        transform.Rotate(rotation);
    }
}
