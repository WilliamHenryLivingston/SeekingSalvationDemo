using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private PlayerController player;
    [SerializeField] private int breadcrumbsBehindPlayer = 10;
    [SerializeField] private float minSpawnTime = 50f; // 5 minutes
    [SerializeField] private float maxSpawnTime = 75f; // 7 minutes

    private bool enemySpawned = false;

    private void Start()
    {
        float spawnDelay = Random.Range(minSpawnTime, maxSpawnTime);
        StartCoroutine(SpawnEnemyAfterDelay(spawnDelay));
    }

    private IEnumerator SpawnEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (player != null && player.breadcrumbsTrail.Count > breadcrumbsBehindPlayer)
        {
            SpawnEnemyAtLastBreadcrumbOffset(breadcrumbsBehindPlayer);
        }
        else
        {
            Debug.LogWarning("Not enough breadcrumbs yet. Waiting for more...");
            StartCoroutine(WaitUntilBreadcrumbsReady());
        }
    }

    private IEnumerator WaitUntilBreadcrumbsReady()
    {
        while (!enemySpawned)
        {
            if (player.breadcrumbsTrail.Count > breadcrumbsBehindPlayer)
            {
                SpawnEnemyAtLastBreadcrumbOffset(breadcrumbsBehindPlayer);
                break;
            }

            yield return new WaitForSeconds(5f); // Check every 5 seconds
        }
    }

    private void SpawnEnemyAtLastBreadcrumbOffset(int offsetFromEnd)
    {
        int trailCount = player.breadcrumbsTrail.Count;
        int spawnIndex = trailCount - offsetFromEnd;

        // Ensure index is valid
        if (spawnIndex < 0 || spawnIndex >= trailCount) return;

        GameObject spawnPoint = player.breadcrumbsTrail[spawnIndex];

        if (spawnPoint != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);

            TrackerEnemy tracker = enemy.GetComponent<TrackerEnemy>();
            if (tracker != null)
            {
                tracker.SetStartingIndex(spawnIndex + 1); // Follow from the next breadcrumb forward
            }

            enemySpawned = true;
            Debug.Log("Something has caught your scent!");
        }
    }
}