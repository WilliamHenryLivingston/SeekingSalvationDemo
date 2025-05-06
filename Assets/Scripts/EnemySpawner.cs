using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private PlayerBreadcrumbTrail player; // Updated reference
    [SerializeField] private int breadcrumbsBehindPlayer = 10;
    [SerializeField] private float minSpawnTime = 50f;
    [SerializeField] private float maxSpawnTime = 75f;

    private GameObject currentEnemy;

    private void Start()
    {
        // Automatically find the player and its breadcrumb trail script
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<PlayerBreadcrumbTrail>();
        }

        if (player == null)
        {
            Debug.LogError("EnemySpawner: PlayerBreadcrumbTrail not found on player!");
            return;
        }

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

        // Wait until enough breadcrumbs are available
        while (player.breadcrumbsTrail.Count <= breadcrumbsBehindPlayer)
        {
            yield return new WaitForSeconds(5f);
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