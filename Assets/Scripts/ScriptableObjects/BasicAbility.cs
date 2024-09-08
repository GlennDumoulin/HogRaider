using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//create an asset for in the Unity editor so that we can make new Abilities in the editor itself
[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability", order = 1)]
public class BasicAbility : ScriptableObject
{
    //declare enum types
    public enum AbilityType
    {
        Player,
        Hog,
    }
    public enum AbilityTarget
    {
        Damage,
        Health,
        CritChance,
        CritDamage,
        Smash,
        Stun,
        Speed,
    }

    //declare scriptable object variables
    public AbilityType _type = AbilityType.Player;
    public AbilityTarget _target = AbilityTarget.Damage;
    public string _name = "";
    public string _description = "";
    [Range(-1, 20)] public int _duration = 10;
    [Range(10, 60)] public int _cooldown = 30;
    public Sprite _thumbnail = null;
}
