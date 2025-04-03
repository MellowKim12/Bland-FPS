using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TriggerBotNew : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
        bool isTargetInCrosshair = WindowCalls.IsInCross<GameObject>(targetPlayer, ownPlayer.GetComponentInChildren<Camera>());
        Triggerbot(isTargetInCrosshair);
    }

    public static void Triggerbot(bool isTargetInCrosshair, float clickDelay = 0.025f)
    {
        try
        {
            if (isTargetInCrosshair)
            {
                WindowCalls.LeftClick(clickDelay);
            }
        }
        catch { }
    }
}
