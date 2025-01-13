using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int health = 1;

    public void Damage(int damage)
    {
        health -= damage;
    }

    private void FixedUpdate()
    {
        if (health <= 0)
            Destroy(this.gameObject);
    }

}
