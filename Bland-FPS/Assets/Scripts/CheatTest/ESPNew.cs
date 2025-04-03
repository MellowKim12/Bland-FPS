using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;

public class ESPNew : MonoBehaviour
{
    GameObject targetPlayer;
    GameObject ownPlayer;

    Camera cam;

    private Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
    private void Awake()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                ownPlayer = player;
                cam = ownPlayer.GetComponentInChildren<Camera>();
            }
            else
                targetPlayer = player;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnGUI()
    {
        DrawCrossHair();
    }

    private void DrawCrossHair()
    {


        // get own camera


        Vector3 target = targetPlayer.transform.position;
        Vector3 w2s = cam.WorldToScreenPoint(target);

        Debug.Log("original coordnates " + w2s);

        w2s.y = center.y - w2s.y + 200.0f;

        Debug.Log("translated coordnates " + w2s);

        if (w2s.z < 0) return;

        RDRRendererNew.DrawBox(new Vector2(w2s.x - 2.0f, w2s.y - 10.0f), new Vector2(10, 10));

    }


}

