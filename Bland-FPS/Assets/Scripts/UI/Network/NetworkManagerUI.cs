    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEditor.Rendering;
using System.Linq;
using TMPro;
using System.Net;
using Unity.Netcode.Transports.UTP;

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
    private Button ipBtn;

    [SerializeField]
    private TMP_InputField ipField;

    [SerializeField]
    private TMP_Text ipText;

    private string targetIp;

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
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            gameStart = true;
            GetLocalIPAddress();
            DisableButtons();
            //spawnManager.GetComponent<SpawnManager>().AssignTeam(player);
            // on start host we need to load a network and load the scene into the game
        });

        clientBtn.onClick.AddListener(() =>
        { 
            gameStart = true;
            SetIPAddress();
            NetworkManager.Singleton.StartClient();
            DisableButtons();
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
                        spawnBtn.gameObject.SetActive(false);
                        // send the clientID rather than object reference
                        clientPlayer.gameObject.GetComponent<PlayerMove>().RespawnRpc(clientPlayer.GetComponent<NetworkObject>().OwnerClientId);   // THIS NEEDS TO BE A RPC TO REFLECT ON SERVER AND CLIENT SIDE
                    }
                    else
                        Debug.Log("client does not match");
                }
            }
        });

        ipBtn.onClick.AddListener(() =>
        {
            targetIp = ipField.text;
        });

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ManualPause();
        }

        if(Input.GetKeyUp(KeyCode.Backslash))
        {
            LoaderNew.Init();
        }

    }

    private void ManualPause()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    // helper function, disables all buttons
    private void DisableButtons()
    {
        serverBtn.gameObject.SetActive(false);
        clientBtn.gameObject.SetActive(false);
        spawnBtn.gameObject.SetActive(false);
        hostBtn.gameObject.SetActive(false);
    }

    public void ActivateRespawn()
    {
        spawnBtn.gameObject.SetActive(true); 
    }

    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                ipText.text = ip.ToString();
                targetIp = ip.ToString();
                return ip.ToString();
            }
        }
        throw new System.Exception("No Network Adapters with an IPv4 address");
    }

    private void SetIPAddress()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = targetIp;
    }


}
