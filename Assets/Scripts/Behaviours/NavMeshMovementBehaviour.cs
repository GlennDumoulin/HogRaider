using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//when a game object has this script, it should also have a nav mesh agent component
[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMovementBehaviour : MovementBehaviour
{
    //declare const variables
    const float MOVEMENT_EPSILON = 0.25f;

    //declare script variables
    private NavMeshAgent _navMeshAgent;
    private Vector3 _previousTargetPosition = Vector3.zero;

    protected override void Awake()
    {
        //execute the MovementBehaviour's Awake
        base.Awake();

        //set nav mesh agent
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = _movementSpeed;

        //set initial previous target position to agent's own position
        _previousTargetPosition = transform.position;
    }

    protected override void HandleMovement()
    {
        //handle stopping the agent when it has no target
        if (_target == null)
        {
            _navMeshAgent.isStopped = true;
            return;
        }

        //set an initial destination for the agent to it's target's position
        if (_navMeshAgent.destination == transform.position)
        {
            _navMeshAgent.SetDestination(_target.transform.position);
            _navMeshAgent.isStopped = false;
            _previousTargetPosition = _target.transform.position;

            return;
        }

        //should the target move, we should recalculate out path
        if ((_target.transform.position - _previousTargetPosition).sqrMagnitude
            > MOVEMENT_EPSILON)
        {
            _navMeshAgent.SetDestination(_target.transform.position);
            _navMeshAgent.isStopped = false;
            _previousTargetPosition = _target.transform.position;
        }
        
        //handle looking at the target
        transform.LookAt(_desiredLookatPoint, Vector3.up);
    }
}
