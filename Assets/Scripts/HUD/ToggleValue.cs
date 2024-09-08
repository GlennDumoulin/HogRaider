using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleValue : MonoBehaviour
{
    //declare editor parameters
    [SerializeField] private BasicAbility _ability = null;

    //declare editor getters/setters
    public BasicAbility Ability
    {
        get { return _ability; }
    }
}
