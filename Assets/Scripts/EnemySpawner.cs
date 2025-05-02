using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private PlayerController player;
    [SerializeField] private int breadcrumbsBehindPlayer = 10;
    [SerializeField] private float minSpawnTime = 50f; // 5 minutes
    [SerializeField] private float maxSpawnTime = 75f; // 7 minutes

    private GameObject currentEnemy;

    private void Start()
    {
        ScheduleNextSpawn();
    }

    private void ScheduleNextSpawn()
    {
        float delay = Random.Range(minSpawnTime, maxSpawnTime);
        StartCoroutine(SpawnEnemyAfterDelay(delay));
    }

    private IEnumerator SpawnEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        while (player == null || player.breadcrumbsTrail.Count <= breadcrumbsBehindPlayer)
        {
            yield return new WaitForSeconds(5f); // Wait for more breadcrumbs
        }

        SpawnEnemyAtLastBreadcrumbOffset(breadcrumbsBehindPlayer);
    }

    private void SpawnEnemyAtLastBreadcrumbOffset(int offsetFromEnd)
    {
        int trailCount = player.breadcrumbsTrail.Count;
        int spawnIndex = trailCount - offsetFromEnd;

        if (spawnIndex < 0 || spawnIndex >= trailCount) return;

        GameObject spawnPoint = player.breadcrumbsTrail[spawnIndex];

        if (spawnPoint != null)
        {
            currentEnemy = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);

            TrackerEnemy tracker = currentEnemy.GetComponent<TrackerEnemy>();
            if (tracker != null)
            {
                tracker.SetStartingIndex(spawnIndex + 1);
                tracker.OnDespawned += HandleEnemyDespawned;
            }

            Debug.Log("Something has caught your scent!");
        }
    }

    private void HandleEnemyDespawned()
    {
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
            currentEnemy = null;
        }

        ScheduleNextSpawn();
    }
}