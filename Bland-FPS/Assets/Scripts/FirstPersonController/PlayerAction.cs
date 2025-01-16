using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAction : NetworkBehaviour
{
    [SerializeField]
    private Shooter shooter;



    private void FixedUpdate()
    {
        // netcode band-aid patch
        if (!IsOwner) return;

        if (Input.GetMouseButton(0) == true)
        {
            OnShoot();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ManualPause();
        }
    }

    private void OnShoot()
    {
        Debug.Log("Shooting");
        shooter.Shoot();
    }

    private void ManualPause()
    {
        Cursor.lockState = CursorLockMode.None;
    }

}
