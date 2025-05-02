using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class TrackerEnemy : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float reachDistance = 1f;

    private List<GameObject> breadcrumbTrail;
    private int currentBreadcrumbIndex = 0;
    private Transform targetBreadcrumb;
    private bool initialized = false;

    public event Action OnDespawned;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                breadcrumbTrail = playerController.breadcrumbsTrail;
                initialized = true;
            }
        }
    }

    // This method allows you to start following breadcrumbs from a specific index
    public void SetStartingIndex(int index)
    {
        currentBreadcrumbIndex = index;
    }

    private void Update()
    {
        if (!initialized || breadcrumbTrail == null || breadcrumbTrail.Count == 0 || currentBreadcrumbIndex >= breadcrumbTrail.Count)
            return;

        // Get the next valid breadcrumb to follow
        if (targetBreadcrumb == null)
        {
            targetBreadcrumb = breadcrumbTrail[currentBreadcrumbIndex]?.transform;
        }

        if (targetBreadcrumb == null) return;

        // Move toward the target breadcrumb
        Vector3 dir = (targetBreadcrumb.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        // Check if the enemy reached the breadcrumb
        float dist = Vector3.Distance(transform.position, targetBreadcrumb.position);
        if (dist <= reachDistance)
        {
            currentBreadcrumbIndex++;
            targetBreadcrumb = null;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        Debug.Log("Player caught by enemy!");
        GameManager.Instance.GameOver();
    }

    public void OnDestroy()
    {
        OnDespawned?.Invoke();
        Destroy(gameObject);
    }
}