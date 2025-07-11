using System.Collections;
using UnityEngine;

public class PlayerClamber : MonoBehaviour
{
    [Header("Clamber Settings")]
    public float wallCheckDistance = 0.4f;
    public float wallCheckRadius = 0.3f;
    public float ledgeCheckHeight = 1.2f;
    public float maxLedgeSlope = 45f;
    public float playerHeightOffset = 1.1f;
    public float clamberDuration = 0.4f;
    public float maxClamberHeightThreshold = 1.5f; // Max height difference to allow clamber
    public LayerMask wallLayer;
    public LayerMask groundLayer;

    [HideInInspector] public bool isGrappling = false; // Step 1: Only clamber while grappling

    private bool isClambering = false;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Step 2: Clambering is allowed only if currently grappling
        if (!IsGrounded() && !isClambering && isGrappling)
        {
            if (CheckForLedge(out Vector3 ledgePos))
            {
                float heightDifference = ledgePos.y - transform.position.y;
                Debug.Log($"Height difference: {heightDifference}");

                // Only clamber if the ledge is ABOVE the player by a sufficient amount
                if (heightDifference <= maxClamberHeightThreshold && heightDifference >= 0.4f)
                {
                    StartCoroutine(ClamberToLedge(ledgePos));
                }
                else
                {
                    Debug.Log("Clamber canceled: Ledge not above player or too high.");
                }
            }
        }
    }

    bool CheckForLedge(out Vector3 ledgePoint)
    {
        ledgePoint = Vector3.zero;
        Vector3 origin = transform.position + Vector3.up * (characterController.height * 0.9f) + transform.forward * 0.3f;

        if (Physics.SphereCast(origin, wallCheckRadius, transform.forward, out RaycastHit wallHit, wallCheckDistance, wallLayer))
        {
            Vector3 wallHitPoint = wallHit.point;
            Vector3 upwardCheckOrigin = wallHitPoint + Vector3.up * ledgeCheckHeight;

            if (Physics.Raycast(upwardCheckOrigin, Vector3.down, out RaycastHit ledgeHit, ledgeCheckHeight * 2f, groundLayer))
            {
                if (Vector3.Angle(ledgeHit.normal, Vector3.up) <= maxLedgeSlope)
                {
                    ledgePoint = ledgeHit.point;
                    Debug.DrawLine(ledgeHit.point, ledgeHit.point + Vector3.up * 0.2f, Color.green, 1f);
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

        float topY = targetPosition.y + playerHeightOffset;
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, topY, transform.position.z);

        float t = 0f;
        while (t < clamberDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, t / clamberDuration);
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        Vector3 forward = transform.forward;
        float castHeight = characterController.height * 0.9f;
        float forwardOffset = 0.3f;

        Vector3 sphereCastOrigin = transform.position + Vector3.up * castHeight + forward * forwardOffset;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(sphereCastOrigin, forward * wallCheckDistance);
        Gizmos.DrawWireSphere(sphereCastOrigin + forward * wallCheckDistance, wallCheckRadius);

        Vector3 wallHitPoint = sphereCastOrigin + forward * wallCheckDistance;
        Vector3 upwardCheckOrigin = wallHitPoint + Vector3.up * ledgeCheckHeight;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(upwardCheckOrigin, Vector3.down * ledgeCheckHeight * 2f);
        Gizmos.DrawWireSphere(upwardCheckOrigin, 0.1f);
        Gizmos.DrawWireSphere(upwardCheckOrigin + Vector3.down * ledgeCheckHeight * 2f, 0.1f);
    }
#endif
}


