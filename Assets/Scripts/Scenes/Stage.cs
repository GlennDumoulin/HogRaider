using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    //declare editor parameters
    [SerializeField] private List<GameObject> _targets = new List<GameObject>();
    [SerializeField] private List<GameObject> _exits = new List<GameObject>();
    [SerializeField] private List<GameObject> _enables = new List<GameObject>();

    //declare editor getters/setters
    public List<GameObject> Targets
    {
        get { return _targets; }
    }
    public List<GameObject> Exits
    {
        get { return _exits; }
    }
    public List<GameObject> Enables
    {
        get { return _enables; }
    }
}
