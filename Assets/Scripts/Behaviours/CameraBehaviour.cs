using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    //declare const variables
    const string LOOK_BACK = "LookBack";

    //declare editor parameters
    [SerializeField] private GameObject _player = null;
    [SerializeField] private float _followSpeed = 5.0f;

    private void FixedUpdate()
    {
        if (_player != null)
        {
            //handle camera position
            transform.position = Vector3.Lerp(transform.position,
                _player.transform.position, _followSpeed * Time.fixedDeltaTime);

            //handle camera rotation
            Quaternion desiredRotation = Quaternion.Euler(
                _player.transform.eulerAngles +
                (180.0f * Convert.ToInt32(Input.GetButton(LOOK_BACK)) * Vector3.up)
            );
            
            transform.rotation = Quaternion.Lerp(transform.rotation,
                desiredRotation, _followSpeed * Time.fixedDeltaTime);
        }
    }
}
