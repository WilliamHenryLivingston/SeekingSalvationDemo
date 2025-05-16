using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Vector2 lastBreadcrumbXZ;


    [Header("Breadcrumb Settings")]
    [SerializeField] private GameObject breadcrumbPrefab;
    [SerializeField] private float breadcrumbDistance = 10f;

    public List<GameObject> breadcrumbsTrail = new List<GameObject>();

    public Transform playerCamera;



    private bool canLook = true;

    private void Start()
    {
      
        lastBreadcrumbXZ = new Vector2(transform.position.x, transform.position.z);
    }

    private void Update()
    {
        //Debug.Log("Update() is running");

        CheckBreadcrumbSpawn();
    }


    private void CheckBreadcrumbSpawn()
    {
        //Debug.Log("CheckBreadcrumbSpawn() is running.");

        if (breadcrumbPrefab == null) return;

        Vector2 currentXZ = new Vector2(transform.position.x, transform.position.z);
        float horizontalDistance = Vector2.Distance(currentXZ, lastBreadcrumbXZ);

        //Debug.Log($"[Breadcrumb Debug] XZ Distance: {horizontalDistance:F2} | Current: {currentXZ} | Last: {lastBreadcrumbXZ}");

        if (horizontalDistance >= breadcrumbDistance)
        {
            //Debug.Log("Spawning Breadcrumb...");
            SpawnBreadcrumb();
            lastBreadcrumbXZ = currentXZ;
        }
    }
    private void SpawnBreadcrumb()
    {
        // Force Y to be a fixed high point for raycast — NOT player's Y
        Vector3 breadcrumbXZ = new Vector3(transform.position.x, 100f, transform.position.z);

        // Raycast down to find the ground
        Vector3 spawnPos = GetGroundedPosition(breadcrumbXZ);

        GameObject breadcrumb = Instantiate(breadcrumbPrefab, spawnPos, Quaternion.identity);
        breadcrumbsTrail.Add(breadcrumb);
    }
    private Vector3 GetGroundedPosition(Vector3 origin)
    {
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 200f))
        {
            return hit.point;
        }

        // Fallback if nothing is hit
        return origin;
    }




    public void SetLookEnabled(bool enabled)
    {
        canLook = enabled;
    }

}