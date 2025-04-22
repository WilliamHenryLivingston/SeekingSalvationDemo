using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private PlayerController player;
    [SerializeField] private int breadcrumbIndexToSpawnAt = 10;
    [SerializeField] private float minSpawnTime = 50f;//300f; // 5 minutes
    [SerializeField] private float maxSpawnTime = 75f;//420f; // 7 minutes

    private bool enemySpawned = false;

    private void Start()
    {
        float spawnDelay = Random.Range(minSpawnTime, maxSpawnTime);
        StartCoroutine(SpawnEnemyAfterDelay(spawnDelay));
    }

    private IEnumerator SpawnEnemyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (player != null && player.breadcrumbsTrail.Count > breadcrumbIndexToSpawnAt)
        {
            GameObject spawnPoint = player.breadcrumbsTrail[breadcrumbIndexToSpawnAt];

            if (spawnPoint != null)
            {
                Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
                enemySpawned = true;
                Debug.Log("Something has found your trail" + breadcrumbIndexToSpawnAt);
            }
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
            if (player.breadcrumbsTrail.Count > breadcrumbIndexToSpawnAt)
            {
                GameObject spawnPoint = player.breadcrumbsTrail[breadcrumbIndexToSpawnAt];
                Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
                enemySpawned = true;
                Debug.Log("Enemy spawned at breadcrumb " + breadcrumbIndexToSpawnAt + " after waiting.");
                break;
            }

            yield return new WaitForSeconds(5f); // Check every 5 seconds
        }
    }
}