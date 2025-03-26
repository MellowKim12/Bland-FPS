using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMove : NetworkBehaviour
{
    public float speed = 5;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    Animator animator;

    Rigidbody rb;
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    [SerializeField]
    private bool MTEAM = true;

    [SerializeField]
    private GameObject mSpawn;
    [SerializeField]
    private GameObject zSpawn;
    [SerializeField]
    private GameObject networkContainer;

    public ulong seqNum = 0;

    [SerializeField] private float positionRange = 5f;


    // Network Data Transfer


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        mSpawn = GameObject.FindGameObjectWithTag("MTeamSpawn");
        zSpawn = GameObject.FindGameObjectWithTag("ZTeamSpawn");

        /*if (!IsOwner)
        {
            Debug.Log("Player is NOT the owner");
            gameObject.GetComponentInChildren<Camera>().enabled = false;
            this.enabled = false;
        }
        else
        {
            Debug.Log("Player IS the owner");
            gameObject.GetComponentInChildren<Camera>().enabled = true;
            /*
            if (MTEAM)
            {
                MTEAM = !MTEAM;
                this.transform.position = mSpawn.transform.position;
            }
            else
            {
                MTEAM = !MTEAM;
                this.transform.position = zSpawn.transform.position;
            }
            //gameObject.GetComponentInChildren<Camera>().enabled = true;

        }*/


    }

    
    public override void OnNetworkSpawn()
    {
        networkContainer = GameObject.FindGameObjectWithTag("respawnAnchor");
        networkContainer.GetComponent<NetworkManagerUI>().playerList.Add(this.gameObject);
        Debug.Log(this.gameObject.GetInstanceID());
        UpdatePositionServerRpc();
        
    }


    private void FixedUpdate()
    {
        // netcode band-aid patch
        if (!IsOwner) return;

        IsRunning = canRun && Input.GetKey(runningKey);

        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        // get target velocity from input
        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);
        rb.velocity = transform.rotation * new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.y);

        // connect to the animator
        animator.SetFloat("Speed", rb.velocity.magnitude);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePositionServerRpc()
    {

        if (MTEAM)
        {
            MTEAM = !MTEAM;
            transform.position = mSpawn.transform.position + new Vector3(0, 1, 0);
        }
        else
        {
            MTEAM = !MTEAM;
            transform.position = zSpawn.transform.position + new Vector3(0, 1, 0);
        }
        transform.rotation = new Quaternion(0, 100, 0, 0);

        Debug.Log("this is running?");
    }

    // restructure respawn code to work off of a clientID rather than an object reference.


    [Rpc(SendTo.Everyone)]
    public void RespawnRpc(ulong clientPlayerID)
    {
        Debug.Log("Respawn RPC Called and Running");
        // first need to re-reference the id to the actual game object
        foreach(GameObject clientPlayer in networkContainer.GetComponent<NetworkManagerUI>().playerList)
        {
            Debug.Log("Searching for players");
            if (clientPlayerID == clientPlayer.GetComponent<NetworkObject>().OwnerClientId)
            {
                Debug.Log("Found the player, attempting to respawn");
                // found the object, reset spawn.
                clientPlayer.SetActive(true);
                clientPlayer.GetComponent<PlayerHealth>().ResetHealth();
                clientPlayer.transform.position = mSpawn.transform.position;
            }
        }
    }



    /*
    public void Respawn(ulong clientPlayer)
    {
        //Debug.Log(clientPlayer.name);
        //Debug.Log(clientPlayer.GetInstanceID());
        // need to pass on player reference from network manager
        foreach (GameObject client in networkContainer.GetComponent<NetworkManagerUI>().playerList)
        {

        }
        clientPlayer.SetActive(true);
        clientPlayer.GetComponent<PlayerHealth>().ResetHealth();

        UpdatePositionServerRpc();
    }
    */
}
