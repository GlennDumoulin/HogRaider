using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCharacter : MonoBehaviour
{
    //declare script variables
    protected MovementBehaviour _movementBehaviour;
    protected ItemBehaviour _itemBehaviour;

    protected virtual void Awake()
    {
        //get components from character
        _movementBehaviour = GetComponent<MovementBehaviour>();
        _itemBehaviour = GetComponent<ItemBehaviour>();
    }
}
