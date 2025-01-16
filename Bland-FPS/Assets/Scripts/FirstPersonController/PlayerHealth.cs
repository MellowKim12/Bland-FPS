using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField]
    private int health = 1;

    public void Damage(int damage)
    {
        health -= damage;
    }

    private void FixedUpdate()
    {
        // netcode band-aid patch
        if (!IsOwner) return;   

        if (health <= 0)
            Destroy(this.gameObject);
    }

}
