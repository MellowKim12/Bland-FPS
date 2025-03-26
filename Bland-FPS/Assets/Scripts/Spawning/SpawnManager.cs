using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;

public class SpawnManager : NetworkBehaviour
{
    public static SpawnManager spawnManager;

    [SerializeField]
    List<Vector3> spawnList = new List<Vector3>() { Vector3.zero };

    [SerializeField]
    GameObject playerObj;

    [SerializeField]

    NetworkManager networkManager;

    [SerializeField]
    GameObject RedSpawn;
    [SerializeField]    
    GameObject BlueSpawn;

    int spawnIndex = 0;


    private bool swapTeam = true;

    public List<int> playerIDs = new List<int>();
    public int playerIndex = 0;


    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }


    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
    }

    private void SceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log("sceneName" +  sceneName);
        if (IsHost && sceneName == "TestScene")
        {
            Debug.Log("yeah we here");

            foreach (ulong id in clientsCompleted)
            {
                Debug.Log("yeah we here");
                GameObject player = Instantiate(playerObj);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
            }
        }
    }



    
    public Vector3 GetNextSpawnPos()
    {
        spawnIndex = (spawnIndex + 1) % spawnList.Count;
        return spawnList[spawnIndex];
    }


    void ConnectionApprovalWithRandomSpawnPos(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // only use connection approval to set player's spawn posisiton, however connections the the server are always approved
        response.CreatePlayerObject = true;
        response.Position = GetNextSpawnPos();
        response.Rotation = Quaternion.identity;
        response.Approved = true;
    }

    
    
    public void AssignTeam(GameObject gameObject)
    {
        Debug.Log("Attempting to Spawn Player");
        GameObject currPlayer = Instantiate(playerObj);
       // currPlayer.GetComponent<PlayerMove>().seqNum = spawnIndex;
        playerIDs.Add(gameObject.GetInstanceID());
        spawnIndex++;
    }

    /*
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc()
    {


    } */
    
}

