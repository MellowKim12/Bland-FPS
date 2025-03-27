using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField]
    private int health = 1;

    private GameObject uiContainer;

    private void Awake()
    {
        uiContainer = GameObject.FindGameObjectWithTag("respawnAnchor");
    }

    public void Damage(int damage)
    {
        Debug.Log("Attempting to do damage");
        DealDamageServerRpc(damage);
    }

    private void Update()
    {
        // netcode band-aid patch
        if (!IsOwner) return;

        if (health <= 0)
        {
            uiContainer.GetComponent<NetworkManagerUI>().ActivateRespawn();
            Cursor.lockState = CursorLockMode.None;
            DestroyPlayerServerRpc();
        }
    }

    public void ResetHealth()
    {
        Debug.Log("Resetting Health");
        health = 1;
    }

    [Rpc(SendTo.Owner)]
    private void DealDamageServerRpc(int damage)
    {
        Debug.Log("We did Damage Here");
        health -= damage;
    }


    [Rpc(SendTo.Everyone)]
    private void DestroyPlayerServerRpc()
    {
        Debug.Log("Destroying Player");
        //this.gameObject.GetComponent<NetworkObject>().Despawn();
        //this.gameObject.GetComponent<NetworkObject>().gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

}
