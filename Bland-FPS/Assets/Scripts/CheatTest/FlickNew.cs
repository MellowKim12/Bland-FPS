using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Credit to @486176652066756E21 on unknowncheats for base source code

public class FlickNew : MonoBehaviour
{
    GameObject targetPlayer;
    GameObject ownPlayer;

    private void Awake()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                ownPlayer = player;
            }
            else
                targetPlayer = player;
        }
    }

    private void Update()
    {
        object[] targets = { targetPlayer };
        Aimbot(targets, targetPlayer.GetComponentInChildren<Camera>().transform.position, ownPlayer.GetComponentInChildren<Camera>(), 1.0);
    }

    public static void Aimbot<T>(IEnumerable<T> targets, Vector3 headPos, Camera localPlayerCam, double mouseSmoothing)
    {
        try
        {
            float closestTargetDistance = float.MaxValue;
            Vector2 closestTargetScreenPosition = Vector2.zero;

            foreach (T target in targets)
            {
                Vector3 headPosition = headPos;

                Vector3 targetScreenPosition = localPlayerCam.WorldToScreenPoint(headPosition);

                if(targetScreenPosition.z > -8)
                {
                    Vector2 centerScreenPosition = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
                    float distancecToCenter = Vector2.Distance(new Vector2(targetScreenPosition.x, Screen.height - targetScreenPosition.y), centerScreenPosition);

                    float fieldOfViewRadius = 300f;

                    if (distancecToCenter < fieldOfViewRadius && distancecToCenter < closestTargetDistance)
                    {
                        closestTargetDistance = distancecToCenter;
                        closestTargetScreenPosition = new Vector2(targetScreenPosition.x, Screen.height - targetScreenPosition.y);
                    }
                }
            }

            if (closestTargetScreenPosition != Vector2.zero)
            {
                double distanceToMoveX = closestTargetScreenPosition.x - Screen.width / 2.0f;
                double distanceToMoveY = closestTargetScreenPosition.y - Screen.height / 2.0f;

                distanceToMoveX /= mouseSmoothing;
                distanceToMoveY /= mouseSmoothing;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    WindowCalls.mouse_event(0x0001, (int)distanceToMoveX, (int)distanceToMoveY, 0, 0);
                }
            }


        }
        catch { }
    }


}
