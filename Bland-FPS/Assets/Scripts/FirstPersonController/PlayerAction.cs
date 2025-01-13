using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private Shooter shooter;



    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0) == true)
        {
            OnShoot();
        }
    }


    public void OnShoot()
    {
        Debug.Log("Shooting");
        shooter.Shoot();
    }
}
