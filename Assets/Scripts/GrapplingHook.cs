using AtrillionGamesLtd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private float grappleRange = 100f;
    [SerializeField] private float pullSpeed = 20f;
    [SerializeField] private LayerMask Default;

    private bool isZipping = false;
    private bool isHanging = false;
    private Vector3 grapplePoint;

    // NEW: Reference to player movement/clamber script
    private AtrillionGamesLtd_PlayerMove playerMoveScript;

    void Start()
    {
        // Find and assign the movement script (assumes this is on the same object or the player is tagged)
        playerMoveScript = GetComponent<AtrillionGamesLtd_PlayerMove>();
        if (playerMoveScript == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                playerMoveScript = playerObj.GetComponent<AtrillionGamesLtd_PlayerMove>();
        }
    }

    void Update()
    {
        // Left Click: Fire grapple
        if (Input.GetMouseButtonDown(0)) // LMB
        {
            Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out RaycastHit hit, grappleRange, Default))
            {
                grapplePoint = hit.point;
                isZipping = true;
                isHanging = false;
                playerRb.useGravity = false;
                playerRb.velocity = Vector3.zero;
            }
        }

        // Right Click: Detach from wall
        if (Input.GetMouseButtonDown(1)) // RMB
        {
            if (isHanging)
            {
                isHanging = false;
                playerRb.useGravity = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (isZipping)
        {
            Vector3 direction = grapplePoint - playerRb.position;
            float distance = direction.magnitude;

            if (distance < 1f)
            {
                isZipping = false;
                playerRb.velocity = Vector3.zero;

                // New: Try to clamber from the grapple point
                if (playerMoveScript != null)
                {
                    bool clambered = playerMoveScript.TryClamberFromGrapple(grapplePoint);

                    if (!clambered)
                    {
                        // Fallback: stick to wall like before
                        isHanging = true;
                        playerRb.useGravity = false;
                        playerRb.velocity = Vector3.zero;
                        transform.position = grapplePoint; // Optional: if you're not already at exact point
                    }

                }
                else
                {
                    // Fallback: Hang if no clamber
                    isHanging = true;
                }

                return;
            }

            Vector3 moveStep = direction.normalized * pullSpeed * Time.fixedDeltaTime;
            playerRb.MovePosition(playerRb.position + moveStep);
        }

        if (isHanging)
        {
            playerRb.velocity = Vector3.zero;
        }
    }
}