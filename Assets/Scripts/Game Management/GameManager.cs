//Copyright 2025 William Livingston
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject _playerPrefab;

    [SerializeField] private SpawnPoint[] _spawnPoints;

    [SerializeField] private GameObject _gameOverUI;
    public InventoryToggle _inventoryToggle;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        StartGame();
    }

    // Spawns in players based on spawn points
    private void StartGame()
    {
        if (NetworkManager.Singleton.IsHost == false)
        {
            return;
        }

        //Debug.Log("Start the game");
        IReadOnlyList<ulong> clients = NetworkManager.Singleton.ConnectedClientsIds;

        //Debug.Log("Player count: " + clients.Count);
        if (_spawnPoints.Length <  clients.Count)
        {
            Debug.Log("There are more client's than there are spawn points. Add more spawn points to spawn new players");
            return;
        }

        for (int i = 0; i < clients.Count; i++) 
        {
            GameObject player = Instantiate(_playerPrefab, _spawnPoints[i].Position, _spawnPoints[i].transform.rotation);
            Debug.Log(player.scene.name);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clients[i]);
            player.GetComponent<Player>().SetUpPlayerId(clients[i]);

            //Debug.Log("Player is created: " + clients[i], player);
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0f;

        if (_inventoryToggle != null)
        {
            _inventoryToggle.disableToggle = true;
            _inventoryToggle.ShowInventoryUIOnly();
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _gameOverUI.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}