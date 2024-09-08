using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoKill : MonoBehaviour
{
    //declare const variables
    const string KILL_METHOD = "Kill";

    public void Init(float lifeTime)
    {
        //set a remaining life time
        Invoke(KILL_METHOD, lifeTime);
    }

    private void Kill()
    {
        Destroy(gameObject);
    }
}
