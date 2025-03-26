using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPlayer : NetworkBehaviour
{



    public void spawn()
    {
        Debug.Log("recieved request to assign team");
        SpawnManager.spawnManager.AssignTeam(this.gameObject);
    }

    
}
