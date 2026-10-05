//Copyright 2025 William Livingston
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
    [SerializeField] private LayerMask grappleLayers;
    [SerializeField] private GameObject grappleReticle;

    private bool isZipping = false;
    private bool isAiming = false;
    private Vector3 grapplePoint;

    private AtrillionGamesLtd_PlayerMove playerMoveScript;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            playerMoveScript = playerObj.GetComponent<AtrillionGamesLtd_PlayerMove>();
        }
        else
        {
            Debug.LogWarning("Player object not found. Make sure it is tagged 'Player'.");
        }

        if (grappleReticle != null)
            grappleReticle.SetActive(false);
    }

    void Update()
    {

        // Right-click hold to aim
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
        }
       


        if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
            if (grappleReticle != null)
                grappleReticle.SetActive(false);
        }

        if (isAiming)
        {
            Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out RaycastHit hit, grappleRange, grappleLayers))
            {
                if (grappleReticle != null)
                {
                    grappleReticle.SetActive(true);
                    grappleReticle.transform.position = hit.point;
                }

                if (Input.GetMouseButtonDown(0)) // Left click to fire
                {
                    FireGrapple(hit.point);
                }
            }
            else
            {
                if (grappleReticle != null)
                    grappleReticle.SetActive(false);
            }
        }

        // Cancel grapple manually (optional)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelGrapple();
        }

        if (isZipping)
        {
            GrappleMove();
        }
    }

    void FireGrapple(Vector3 point)
    {
        grapplePoint = point;
        isZipping = true;
        playerRb.useGravity = false;
        playerRb.velocity = Vector3.zero;

        if (playerMoveScript != null)
            playerMoveScript.isGrappling = true;
    }

    void GrappleMove()
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
                    playerRb.useGravity = false;
                    playerRb.velocity = Vector3.zero;
                    transform.position = grapplePoint;
                }
            }

            return;
        }

        Vector3 moveStep = direction.normalized * pullSpeed * Time.deltaTime;
        playerRb.MovePosition(playerRb.position + moveStep);
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
        isAiming = false;
        playerRb.useGravity = true;
        playerRb.velocity = Vector3.zero;

        if (grappleReticle != null)
            grappleReticle.SetActive(false);

        if (playerMoveScript != null)
            playerMoveScript.isGrappling = false;
    }
}