using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBreadcrumbTrail : MonoBehaviour
{
    [Header("Breadcrumb Settings")]
    [SerializeField] private GameObject breadcrumbPrefab;
    [SerializeField] private float breadcrumbDistance = 10f;

    public List<GameObject> breadcrumbsTrail = new List<GameObject>();

    private Vector3 lastBreadcrumbPosition;

    private void Start()
    {
        lastBreadcrumbPosition = transform.position;
    }

    private void Update()
    {
        CheckBreadcrumbSpawn();
    }

    private void CheckBreadcrumbSpawn()
    {
        if (breadcrumbPrefab == null) return;

        float distanceTraveled = Vector3.Distance(transform.position, lastBreadcrumbPosition);

        if (distanceTraveled >= breadcrumbDistance)
        {
            SpawnBreadcrumb();
            lastBreadcrumbPosition = transform.position;
        }
    }

    private void SpawnBreadcrumb()
    {
        Vector3 spawnPos = GetGroundedPosition(transform.position);
        GameObject breadcrumb = Instantiate(breadcrumbPrefab, spawnPos, Quaternion.identity);
        breadcrumbsTrail.Add(breadcrumb);
    }

    /// <summary>
    /// Casts a ray downward to place the breadcrumb on the ground.
    /// </summary>
    private Vector3 GetGroundedPosition(Vector3 origin)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin + Vector3.up * 5f, Vector3.down, out hit, 10f))
        {
            return hit.point;
        }

        // Fallback if nothing is hit
        return origin;
    }
}
