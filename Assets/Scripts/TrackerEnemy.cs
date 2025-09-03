using UnityEngine;
using System.Collections.Generic;
using System;

public class TrackerEnemy : MonoBehaviour
{
    [Header("Enemy Variant Settings")]
    public EnemyData enemyProfile;

    private float moveSpeed;
    private float reachDistance;

    private List<GameObject> breadcrumbTrail;
    private int currentBreadcrumbIndex = 0;
    private Transform targetBreadcrumb;
    private bool initialized = false;

    public event Action OnDespawned;

    private void Start()
    {
        if (enemyProfile != null)
        {
            moveSpeed = enemyProfile.moveSpeed;

            // Optional: change color/visual for clarity
            Renderer rend = GetComponentInChildren<Renderer>();
            if (rend != null) rend.material.color = enemyProfile.enemyColor;
        }

        // Find the player and breadcrumb trail
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

    public void SetStartingIndex(int index) => currentBreadcrumbIndex = index;

    private void Update()
    {
        if (!initialized || breadcrumbTrail == null || breadcrumbTrail.Count == 0 || currentBreadcrumbIndex >= breadcrumbTrail.Count)
            return;

        if (targetBreadcrumb == null)
        {
            while (currentBreadcrumbIndex < breadcrumbTrail.Count && breadcrumbTrail[currentBreadcrumbIndex] == null)
            {
                currentBreadcrumbIndex++;
            }

            if (currentBreadcrumbIndex < breadcrumbTrail.Count)
                targetBreadcrumb = breadcrumbTrail[currentBreadcrumbIndex].transform;
        }

        if (targetBreadcrumb == null) return;

        // Move toward breadcrumb
        Vector3 dir = (targetBreadcrumb.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetBreadcrumb.position) <= reachDistance)
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
        Debug.Log($"{enemyProfile.enemyName} caught the player!");
        GameManager.Instance.GameOver();
    }

    public void Despawn()
    {
        OnDespawned?.Invoke();
        Destroy(gameObject);
    }
}