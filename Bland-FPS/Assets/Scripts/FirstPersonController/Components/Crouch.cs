using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Crouch : NetworkBehaviour
{
    public KeyCode key = KeyCode.LeftControl;

    [Header("Slow Movevment")]
    [Tooltip("Movement to slow down when crouched")]
    public PlayerMove movement;
    [Tooltip("Movement speed when courched")]
    public float movementSpeed = 2;

    [Header("Low head")]
    [Tooltip("Head to lower when crouched")]
    public Transform headToLower;
    [HideInInspector]
    public float? defaultHeadYLocalPosition;
    public float crouchYHeadPosition = 1;

    [Tooltip("Collider to lwoer when crouched")]
    public CapsuleCollider colliderToLower;
    [HideInInspector]
    public float? defaultColliderHeight;

    public bool IsCrouched { get; private set; }
    public event System.Action CrouchStart, CrouchEnd;

    private void Reset()
    {
        movement = GetComponentInParent<PlayerMove>();
        headToLower = movement.GetComponentInChildren<Camera>().transform;
        colliderToLower = movement.GetComponentInChildren<CapsuleCollider>();
    }

    private void LateUpdate()
    {
        // netcode band-aid patch
        if (!IsOwner) return;

        if (Input.GetKey(key))
        {
            if (headToLower)
            {
                if (!defaultHeadYLocalPosition.HasValue)
                {
                    defaultHeadYLocalPosition = headToLower.localPosition.y;
                }
                headToLower.localPosition = new Vector3(headToLower.localPosition.x, crouchYHeadPosition, headToLower.localPosition.z);
            }

            if (colliderToLower)
            {
                if (!defaultColliderHeight.HasValue)
                {
                    defaultColliderHeight = colliderToLower.height;
                }
                float loweringAmount;
                if (defaultHeadYLocalPosition.HasValue)
                {
                    loweringAmount = defaultHeadYLocalPosition.Value - crouchYHeadPosition;
                }
                else
                {
                    loweringAmount = defaultColliderHeight.Value * 0.5f;
                }

                colliderToLower.height = Mathf.Max(defaultColliderHeight.Value - loweringAmount, 0);
                colliderToLower.center = Vector3.up * colliderToLower.height * 0.5f;
            }

            if (!IsCrouched)
            {
                IsCrouched = true;
                SetSpeedOverrideActive(true);
                CrouchStart?.Invoke();
            }
        }
        else
        {
            if (IsCrouched)
            {
                if (headToLower)
                {
                    headToLower.localPosition = new Vector3(headToLower.localPosition.x, defaultHeadYLocalPosition.Value, headToLower.localPosition.z);
                }

                if (colliderToLower)
                {
                    colliderToLower.height = defaultColliderHeight.Value;
                    colliderToLower.center = Vector3.up * colliderToLower.height * 0.5f;
                }

                IsCrouched = false;
                SetSpeedOverrideActive(false);
                CrouchEnd?.Invoke();
            }
        }
    }

    #region Speed Override
    void SetSpeedOverrideActive(bool state)
    {
        if (!movement)
        { return; }

        if (state)
        {
            if (!movement.speedOverrides.Contains(SpeedOverride))
            {
                movement.speedOverrides.Add(SpeedOverride);
            }    
        }
        else
        {
            if (movement.speedOverrides.Contains(SpeedOverride))
            {
                movement.speedOverrides.Remove(SpeedOverride);
            }
        }
   
    }

    float SpeedOverride() => movementSpeed;
    #endregion
}
