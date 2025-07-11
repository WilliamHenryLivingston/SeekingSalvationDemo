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

    private AtrillionGamesLtd_PlayerMove playerMoveScript;
    private PlayerClamber playerClamber; //  Reference to clamber script

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            playerMoveScript = playerObj.GetComponent<AtrillionGamesLtd_PlayerMove>();
            playerClamber = playerObj.GetComponent<PlayerClamber>(); //  Fix: Now this works
        }
        else
        {
            Debug.LogWarning("Player object not found. Make sure it is tagged 'Player'.");
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

                //  Enable clambering during grapple
                if (playerClamber != null)
                    playerClamber.isGrappling = true;
            }
        }

        // Right Click: Detach from wall
        if (Input.GetMouseButtonDown(1)) // RMB
        {
            if (isHanging)
            {
                isHanging = false;
                playerRb.useGravity = true;

                //  Disable clambering when hanging ends
                if (playerClamber != null)
                    playerClamber.isGrappling = false;
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

                if (playerMoveScript != null)
                {
                    bool clambered = playerMoveScript.TryClamberFromGrapple(grapplePoint);

                    if (!clambered)
                    {
                        isHanging = true;
                        playerRb.useGravity = false;
                        playerRb.velocity = Vector3.zero;
                        transform.position = grapplePoint;
                    }
                }
                else
                {
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

    private void OnCollisionEnter(Collision collision)
    {
        if (isZipping)
        {
            CancelGrapple();
        }
    }

    private void CancelGrapple()
    {
        isZipping = false;
        isHanging = false;
        playerRb.useGravity = true;
        playerRb.velocity = Vector3.zero;

        //  Disable clambering when grapple is canceled
        if (playerClamber != null)
            playerClamber.isGrappling = false;

        // Optional: Add feedback here, e.g., sound or effects
    }
}
