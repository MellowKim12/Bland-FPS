using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticController : MonoBehaviour
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
        if (Physics.Raycast(SpawnPoint.position, Direction.forward, out RaycastHit hit, float.MaxValue, Mask))
        {
            if (hit.collider.gameObject.tag == "Enemy")
            {
                Debug.Log("tracking a player");
                TrackTime += Time.time;
            }
        }
    }

}
