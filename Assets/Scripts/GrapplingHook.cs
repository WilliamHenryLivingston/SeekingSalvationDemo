using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private float grappleRange = 50f;
    [SerializeField] private float pullSpeed = 20f;
    [SerializeField] private LayerMask Default;

    private bool isZipping = false;
    private bool isHanging = false;
    private Vector3 grapplePoint;

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
                isHanging = true;
                playerRb.velocity = Vector3.zero;
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