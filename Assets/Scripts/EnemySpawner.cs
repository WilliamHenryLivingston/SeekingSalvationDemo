using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Variants")]
    [SerializeField] private GameObject[] enemyPrefabs;  // Array of variants

    [Header("Player Tracking")]
    [SerializeField] private PlayerBreadcrumbTrail player;
    [SerializeField] private int breadcrumbsBehindPlayer = 10;

    [Header("Spawn Timing")]
    [SerializeField] private float minSpawnTime = 50f;
    [SerializeField] private float maxSpawnTime = 75f;

    private GameObject currentEnemy;

    private void Start()
    {
        // Find the player and its breadcrumb trail script
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

        if (spawnPoint != null && enemyPrefabs.Length > 0)
        {
            GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            currentEnemy = Instantiate(enemyToSpawn, spawnPoint.transform.position, Quaternion.identity);

            TrackerEnemy tracker = currentEnemy.GetComponent<TrackerEnemy>();
            if (tracker != null)
            {
                tracker.SetStartingIndex(spawnIndex + 1);
                tracker.OnDespawned += HandleEnemyDespawned;

                // Print special message
                if (tracker.enemyProfile != null && !string.IsNullOrEmpty(tracker.enemyProfile.spawnMessage))
                {
                    Debug.Log(tracker.enemyProfile.spawnMessage);
                }
                else
                {

                }
            }
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