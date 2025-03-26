using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class PlayerLook : NetworkBehaviour
{
    [SerializeField]
    Transform player;

    public float sensitivity = 2;
    public float smoothing = 1.5f;

    Vector2 velocity;
    Vector2 frameVelocity;

    private bool setCam = false;

    private void Reset()
    {
        player = GetComponentInParent<PlayerMove>().transform;
    }

    private void Awake()
    {

    }

    public void ActivateLocalCam(GameObject playerID)
    {
        /*
        Debug.Log("meow?");
        if (OwnerClientId == playerID.GetComponent<NetworkObject>().OwnerClientId && !setCam)
        {

        }
        else
            Debug.Log("Thats not your purse!"); */
    }
    public override void OnNetworkSpawn()
    {
        Debug.Log("meow?");
        if (IsOwner)
            GetComponent<Camera>().enabled = true;
        else
            GetComponent<Camera>().enabled = false;
    }

    void Update()
    {
        // netcode band-aid patch
        if (!IsOwner) return;

        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        player.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
}
