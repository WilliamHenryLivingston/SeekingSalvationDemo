using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    public float tileSize = 1.01845f;
    public Vector3 spawnCenter = Vector3.zero;

    [System.Serializable]
    public class HexTile
    {
        public GameObject item;
        public float probability;
    }

    public List<HexTile> hexTiles;

    [Range(2, 100)]
    public int gridSize = 10;

    public float overlapCheckRadius = 0.6f; // Adjust based on tile scale

    private void Start()
    {
        GenerateGrid();
    }

    private GameObject PickWeightedRandom(List<HexTile> hexTiles)
    {
        float totalProbability = hexTiles.Sum(t => t.probability);
        float randomValue = Random.value * totalProbability;
        float cumulative = 0f;

        foreach (var tile in hexTiles)
        {
            cumulative += tile.probability;
            if (randomValue < cumulative)
                return tile.item;
        }

        return hexTiles.Last().item;
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            float offsetX = x * tileSize * Mathf.Cos(Mathf.Deg2Rad * 30);
            float offsetZ = (x % 2 == 0) ? 0 : tileSize / 2;

            for (int y = 0; y < gridSize; y++)
            {
                Vector3 spawnPos = spawnCenter + new Vector3(offsetX, 0f, offsetZ);

                if (IsOverlappingCheckpoint(spawnPos))
                {
                   // Debug.Log($"Skipped spawning tile at {spawnPos} due to Checkpoint.");
                    //offsetZ += tileSize;
                   // continue;
                }

                GameObject tilePrefab = PickWeightedRandom(hexTiles);
                GameObject tile = Instantiate(tilePrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 6) * 60f, 0), transform);

                offsetZ += tileSize;
            }
        }
    }

    private bool IsOverlappingCheckpoint(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, overlapCheckRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Checkpoint"))
            {
                return true;
            }
        }
        return false;
    }
}