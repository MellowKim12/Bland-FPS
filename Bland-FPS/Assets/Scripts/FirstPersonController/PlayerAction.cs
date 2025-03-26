using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAction : NetworkBehaviour
{
    [SerializeField]
    private Shooter shooter;

    [SerializeField]
    private int hp = 5;

    private void Update()
    {
        // netcode band-aid patch
        if (!IsOwner)
        {
            //Debug.Log("NOT THE OWNER");
            return;
        }
       

        if (Input.GetMouseButton(0) == true)
        {
            ShootServerRpc();
        }

    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        Debug.Log("Shooting");
        shooter.Shoot();
    }



    public void DealDamage()
    {
        this.hp = hp - 1;
    }

}
