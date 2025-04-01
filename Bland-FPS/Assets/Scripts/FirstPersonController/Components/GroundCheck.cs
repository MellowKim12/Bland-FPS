using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Tooltip("Maximum distance from the ground")]
    public float distanceThreshold = .15f;

    [Tooltip("Wheteher this transform is gorudned now")]
    public bool isGrounded = true;

    public event System.Action Grounded;

    private Animator animator;


    const float OriginOffset = .001f;
    Vector3 RaycastOrigin => transform.position + Vector3.up * OriginOffset;
    float RaycastDistance => distanceThreshold + OriginOffset;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    private void LateUpdate()
    {
        // netcode band-aid patch
        bool isGroundedNow = Physics.Raycast(RaycastOrigin, Vector3.down, distanceThreshold * 2);

        if (isGroundedNow && !isGrounded)
        {
            Grounded?.Invoke();
        }

        // Update isGrounded
        isGrounded = isGroundedNow;
        animator.SetBool("FreeFall", isGrounded);

        if (isGrounded)
        {
            animator.SetBool("Jump", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawLine(RaycastOrigin, RaycastOrigin + Vector3.down * RaycastDistance, isGrounded ? Color.white : Color.red);
    }

}
