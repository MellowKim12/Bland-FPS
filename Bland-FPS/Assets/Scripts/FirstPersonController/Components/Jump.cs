using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    Rigidbody rb;
    public float jumpStrength = 2;
    public event System.Action Jumped;

    [SerializeField, Tooltip("Prevents jumping when in mid-air")]
    GroundCheck groundCheck;

    private void Reset()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    void LateUpdate()
    {
        if (Input.GetButtonDown("Jump") && (!groundCheck || groundCheck.isGrounded))
        {
            rb.AddForce(Vector3.up * 100 * jumpStrength);
            Jumped?.Invoke();
        }
    }
}
