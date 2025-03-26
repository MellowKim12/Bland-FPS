    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEditor.Rendering;
using System.Linq;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button serverBtn;
    [SerializeField]
    private Button hostBtn;
    [SerializeField]
    private Button clientBtn;
    [SerializeField]
    private Button spawnBtn;
   

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject spawnManager;

    private GameObject currPlayer;

    private bool gameStart = false;
    private bool canSpawn = false;

    public List<GameObject> playerList = new List<GameObject>();

    private void Awake()
    {
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        }); 

        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            gameStart = true;
            //spawnManager.GetComponent<SpawnManager>().AssignTeam(player);
            // on start host we need to load a network and load the scene into the game
        });

        clientBtn.onClick.AddListener(() =>
        { 
            gameStart = true;
            NetworkManager.Singleton.StartClient();
            //spawnManager.GetComponent<SpawnManager>().AssignTeam(player);
        });

        spawnBtn.onClick.AddListener(() =>
        {
            Debug.Log("Respawn Button was pressed");
            
            if (gameStart)
            {
                Debug.Log("Attempting to Search");
                foreach (GameObject clientPlayer in playerList)
                {
                    Debug.Log("Searching through players");
                    if (clientPlayer.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                    {
                        Debug.Log("Found player, respawning now");
                        // send the clientID rather than object reference
                        clientPlayer.gameObject.GetComponent<PlayerMove>().RespawnRpc(clientPlayer.GetComponent<NetworkObject>().OwnerClientId);   // THIS NEEDS TO BE A RPC TO REFLECT ON SERVER AND CLIENT SIDE
                    }
                    else
                        Debug.Log("client does not match");
                }
            }
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ManualPause();
        }
    }

    private void ManualPause()
    {
        Cursor.lockState = CursorLockMode.None;
    }



}
