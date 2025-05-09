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
        // Try to find the player and their breadcrumb trail
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerBreadcrumbTrail trail = player.GetComponent<PlayerBreadcrumbTrail>();
            if (trail != null)
            {
                breadcrumbTrail = trail.breadcrumbsTrail;
                initialized = true;
            }
            else
            {
                Debug.LogError("TrackerEnemy: PlayerBreadcrumbTrail not found on player.");
            }
        }
        else
        {
            Debug.LogError("TrackerEnemy: Player with tag 'Player' not found.");
        }
    }

    // Called by spawner to define where in the trail to start
    public void SetStartingIndex(int index)
    {
        currentBreadcrumbIndex = index;
    }

    private void Update()
    {
        if (!initialized || breadcrumbTrail == null || breadcrumbTrail.Count == 0 || currentBreadcrumbIndex >= breadcrumbTrail.Count)
            return;

        if (targetBreadcrumb == null)
        {
            // Skip null entries in the trail
            while (currentBreadcrumbIndex < breadcrumbTrail.Count && breadcrumbTrail[currentBreadcrumbIndex] == null)
            {
                currentBreadcrumbIndex++;
            }

            if (currentBreadcrumbIndex < breadcrumbTrail.Count)
                targetBreadcrumb = breadcrumbTrail[currentBreadcrumbIndex].transform;
        }

        if (targetBreadcrumb == null) return;

        // Move toward the current breadcrumb
        Vector3 dir = (targetBreadcrumb.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

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

    public void Despawn()
    {
        OnDespawned?.Invoke();
        Destroy(gameObject);
    }


}