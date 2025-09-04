using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerEnemy : MonoBehaviour
{
    [Header("Enemy Variant Settings")]
    public EnemyData enemyProfile;

    [Header("Overrides (optional)")]
    [SerializeField] private float moveSpeedOverride = -1f;
    [SerializeField] private float reachDistanceOverride = -1f;

    [Header("Tuning")]
    [SerializeField] private float repathInterval = 0.25f; // how often we re-validate target

    private float moveSpeed = 3.5f;
    private float reachDistance = 0.35f; // safe default > 0
    private float sqrReach;

    private List<GameObject> breadcrumbTrail;
    private int currentBreadcrumbIndex = 0;
    private Transform targetBreadcrumb;
    private bool initialized = false;

    public event Action OnDespawned;

    private void Awake()
    {
        // Read from profile with safe fallbacks
        if (enemyProfile != null)
        {
            if (enemyProfile.moveSpeed > 0f) moveSpeed = enemyProfile.moveSpeed;
            if (enemyProfile.reachDistance > 0f) reachDistance = enemyProfile.reachDistance;

            // Visual identifier per-variant (optional)
            var rend = GetComponentInChildren<Renderer>();
            if (rend != null) rend.material.color = enemyProfile.enemyColor;
        }

        // Apply overrides if provided
        if (moveSpeedOverride > 0f) moveSpeed = moveSpeedOverride;
        if (reachDistanceOverride > 0f) reachDistance = reachDistanceOverride;

        sqrReach = reachDistance * reachDistance;
    }

    private void Start()
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("TrackerEnemy: Player with tag 'Player' not found.");
            return;
        }

        var trail = player.GetComponent<PlayerBreadcrumbTrail>();
        if (trail == null)
        {
            Debug.LogError("TrackerEnemy: PlayerBreadcrumbTrail not found on player.");
            return;
        }

        breadcrumbTrail = trail.breadcrumbsTrail;
        ClampStartingIndex();
        initialized = true;

        AcquireNextTarget();
        StartCoroutine(RepathLoop());
    }

    public void SetStartingIndex(int index)
    {
        currentBreadcrumbIndex = index;
        ClampStartingIndex();
    }

    private void ClampStartingIndex()
    {
        if (breadcrumbTrail == null) return;
        currentBreadcrumbIndex = Mathf.Clamp(currentBreadcrumbIndex, 0, Mathf.Max(0, breadcrumbTrail.Count - 1));
    }

    private IEnumerator RepathLoop()
    {
        var wait = new WaitForSeconds(repathInterval);
        while (true)
        {
            if (initialized && (targetBreadcrumb == null || currentBreadcrumbIndex >= breadcrumbTrail.Count))
                AcquireNextTarget();

            yield return wait;
        }
    }

    private void AcquireNextTarget()
    {
        if (breadcrumbTrail == null) { targetBreadcrumb = null; return; }

        // Skip consumed/missing crumbs
        while (currentBreadcrumbIndex < breadcrumbTrail.Count && breadcrumbTrail[currentBreadcrumbIndex] == null)
            currentBreadcrumbIndex++;

        if (currentBreadcrumbIndex < breadcrumbTrail.Count)
            targetBreadcrumb = breadcrumbTrail[currentBreadcrumbIndex]?.transform;
        else
            targetBreadcrumb = null;
    }

    private void Update()
    {
        if (!initialized || targetBreadcrumb == null) return;

        Vector3 toTarget = targetBreadcrumb.position - transform.position;
        float sqrDist = toTarget.sqrMagnitude;

        // Advance when within reach radius
        if (sqrDist <= sqrReach)
        {
            currentBreadcrumbIndex++;
            AcquireNextTarget();
            return;
        }

        // Move and face
        Vector3 dir = toTarget.normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), 10f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            KillPlayer();
    }

    private void KillPlayer()
    {
        string nameSafe = enemyProfile != null ? enemyProfile.enemyName : "Enemy";
        Debug.Log($"{nameSafe} caught the player!");
        GameManager.Instance.GameOver();
    }

    public void Despawn()
    {
        OnDespawned?.Invoke();
        Destroy(gameObject);
    }
}