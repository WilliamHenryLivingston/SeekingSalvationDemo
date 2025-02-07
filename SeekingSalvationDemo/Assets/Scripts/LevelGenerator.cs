using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public float tileSize = 1.01845f; //original number = 101.845
    public Vector3 spawnCenter = Vector3.zero;

    [System.Serializable]
    public class HexTile
    {
        public GameObject item;
        public float probability;
    }

    public List<HexTile> hexTiles;

    [Range(2, 10)]
    public int gridSize = 100;

    private void Start()
    {
        GenerateGrid();
    }

    private GameObject PickWeightedRandom(List<HexTile> hexTiles)
    {
        float totalProbability = 0f;
        foreach (var tile in hexTiles)
        {
            totalProbability += tile.probability;
        }

        float randomValue = Random.value * totalProbability;
        float cumulativeProbability = 0f;

        foreach (var tile in hexTiles)
        {
            cumulativeProbability += tile.probability;
            if (randomValue < cumulativeProbability)
            {
                return tile.item;
            }
        }

        return hexTiles[hexTiles.Count - 1].item; // Default to the last tile if none selected
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            float offsetX = x * tileSize * Mathf.Cos(Mathf.Deg2Rad * 30);
            float offsetY = (x % 2 == 0) ? 0 : tileSize / 2;

            for (int y = 0; y < gridSize; y++)
            {
                GameObject tilePrefab = PickWeightedRandom(hexTiles);
                GameObject tile = Instantiate(tilePrefab, transform);

                tile.transform.position = spawnCenter + new Vector3(offsetX, 0, offsetY);
                float rotationAngle = Mathf.Floor(Random.Range(0f, 6f)) * 60f;
                tile.transform.rotation = Quaternion.Euler(0, rotationAngle, 0);

                offsetY += tileSize;
            }
        }
    }
}