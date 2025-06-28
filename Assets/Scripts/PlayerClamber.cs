using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClamber : MonoBehaviour
{
    [Header("Clamber Settings")]
    public float chestHeight = 1.2f;
    public float wallCheckDistance = 0.5f;
    public float ledgeCheckHeight = 1f;
    public float maxLedgeSlope = 45f;
    public float playerHeightOffset = 1f;
    public LayerMask wallLayer;
    public LayerMask groundLayer;

    private bool isClambering = false;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!IsGrounded() && !isClambering)
        {
            if (CheckForLedge(out Vector3 ledgePos))
            {
                StartCoroutine(ClamberToLedge(ledgePos));
            }
        }
    }

    bool CheckForLedge(out Vector3 ledgePoint)
    {
        ledgePoint = Vector3.zero;
        Vector3 origin = transform.position + Vector3.up * chestHeight;

        if (Physics.Raycast(origin, transform.forward, out RaycastHit wallHit, wallCheckDistance, wallLayer))
        {
            Vector3 ledgeOrigin = wallHit.point + Vector3.up * ledgeCheckHeight;
            if (Physics.Raycast(ledgeOrigin, Vector3.down, out RaycastHit ledgeHit, ledgeCheckHeight * 2f, groundLayer))
            {
                if (Vector3.Angle(ledgeHit.normal, Vector3.up) <= maxLedgeSlope)
                {
                    ledgePoint = ledgeHit.point;
                    return true;
                }
            }
        }

        return false;
    }

    IEnumerator ClamberToLedge(Vector3 targetPosition)
    {
        isClambering = true;
        characterController.enabled = false;

        Vector3 startPos = transform.position;
        Vector3 endPos = targetPosition + Vector3.up * playerHeightOffset;

        float t = 0f;
        float duration = 0.4f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, t / duration);
            yield return null;
        }

        transform.position = endPos;
        characterController.enabled = true;
        isClambering = false;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayer);
    }
}