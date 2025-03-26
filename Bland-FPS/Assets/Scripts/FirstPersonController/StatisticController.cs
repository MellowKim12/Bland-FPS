using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StatisticController : NetworkBehaviour
{
    [SerializeField]
    private LayerMask Mask;
    [SerializeField]
    private Transform SpawnPoint;

    private Transform Direction;


    [SerializeField]
    private float TrackTime;

    private void Awake()
    {
        Direction = GetComponentInChildren<Camera>().transform;
    }
        
    private void FixedUpdate()
    {

    }

    // function to check whether or not a player is tracking players through walls
    private void IllegalTraceCheck()
    {
        if (Physics.Raycast(SpawnPoint.position, Direction.forward, out RaycastHit hit, float.MaxValue, Mask))
        {
            if (hit.collider.gameObject.tag == "Enemy" && !(hit.collider.gameObject.GetComponent<MeshRenderer>().isVisible))
            {
                Debug.Log("tracking a player");
                TrackTime += Time.time;
            }
        }
    }



}
