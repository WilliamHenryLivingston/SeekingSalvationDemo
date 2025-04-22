using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TrackerEnemy : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float reachDistance = 1f;

    private List<GameObject> breadcrumbTrail;
    private int currentBreadcrumbIndex = 0;

    private Transform targetBreadcrumb;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerController playerController = player.GetComponent<PlayerController>();

        breadcrumbTrail = playerController.breadcrumbsTrail;
    }

    private void Update()
    {
        if (breadcrumbTrail == null || breadcrumbTrail.Count == 0)
            return;

        // Get next valid target
        if (targetBreadcrumb == null && currentBreadcrumbIndex < breadcrumbTrail.Count)
        {
            targetBreadcrumb = breadcrumbTrail[currentBreadcrumbIndex]?.transform;
        }

        if (targetBreadcrumb == null) return;

        // Move toward breadcrumb
        Vector3 dir = (targetBreadcrumb.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        // Reached current breadcrumb
        float dist = Vector3.Distance(transform.position, targetBreadcrumb.position);
        if (dist <= reachDistance)
        {
            currentBreadcrumbIndex++;
            targetBreadcrumb = null;
        }
    }
}