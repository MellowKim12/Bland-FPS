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

    private Vector2 center = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
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

        w2s.x *= Screen.width / (float)cam.pixelWidth;
        w2s.y *= Screen.height / (float) cam.pixelHeight;


        //Debug.Log("original coordnates " + w2s);

        w2s.y = center.y * 2 - w2s.y;

        //Debug.Log("translated coordnates " + w2s);

        if (w2s.z < 0) return;

        RDRRendererNew.DrawBox(new Vector2(w2s.x, w2s.y), new Vector2(10, 10));

    }


}

