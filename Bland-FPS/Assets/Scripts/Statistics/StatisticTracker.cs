using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticTracker : MonoBehaviour
{
    // TO DO:
    // 1. TIMER TO TRACK PLAYERS THROUGH WALL #################### DONE ####################
    // 2. TRIGGER BOT DETECTION
    // 3. FLICK MOUSE MOVEMENT DETECTION

    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private GameObject cameraDirection;

    [SerializeField]
    private LayerMask wallMask;

    private int trackTime = 0;
    private int triggerTime = 0;
    private float highestMouseSpeed = 0f;

    private float mouseXSpeed = 0f;
    private float mouseYSpeed = 0f;
    private float mouseSpeed = 0f;

    private float timer = 0f;

    private void Update()
    {
        // TOPIC 1: WallHack Defeater
        #region
        Vector3 direction = cameraDirection.transform.forward;

        RaycastHit[] hits;
        hits = Physics.SphereCastAll(spawnPoint.position, 0.75f, direction, float.MaxValue);
        System.Array.Sort(hits, (x,y) => x.distance.CompareTo(y.distance));
        bool wallFirst = false;

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider.gameObject.layer == 0) wallFirst = true;
            if (wallFirst && hit.collider.gameObject.layer == 6)
            {
                // increment
                trackTime += 1;
            }
        }
        Debug.Log("TrackTime: " + trackTime);
        #endregion

        // TOPIC 2: Trigger bot detection
        #region
        if (Physics.SphereCast(spawnPoint.position, 0.25f, direction, out RaycastHit triggerHit, float.MaxValue))
        {
            // we have hit a player
            if (triggerHit.collider.gameObject.layer == 6)
            {
                triggerTime += 1;
                timer = 0f;
            }
            else
            {
                timer += Time.deltaTime;
                if (timer > 10f)
                {
                    timer = 0f;
                    triggerTime = 0;
                    Debug.Log("Resetting Timer");
                }
            }
        }

        Debug.Log("Trigger Time: " + triggerTime);
        #endregion

        // TOPIC 3: Erratic Mouse Movement Detection
        #region
        mouseXSpeed = Input.GetAxis("Mouse X") / Time.deltaTime;
        mouseYSpeed = Input.GetAxis("Mouse Y") / Time.deltaTime;

        mouseSpeed = (mouseXSpeed + mouseYSpeed) / 2.0f;

        if (mouseSpeed > highestMouseSpeed)
            highestMouseSpeed = mouseSpeed;
        #endregion

    }

    // helper function to reset tracktime on kill also writes to csv file
    public void ResetTrackTime()
    {
        trackTime = 0;
    }

    public int GetTrackTime()
    {
        return trackTime;
    }

    public int GetTriggerTime()
    {
        return triggerTime;
    }

    public void ResetTriggerTime()
    {
        triggerTime = 0;
    }

    public void ResetMouseSpeed()
    {
        highestMouseSpeed = 0;
    }

    public float GetMouseSpeed()
    {
        return highestMouseSpeed;
    }



}
