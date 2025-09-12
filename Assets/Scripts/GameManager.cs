//Copyright 2025 William Livingston
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject gameOverUI;
    public InventoryToggle inventoryToggle;


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

    public void GameOver()
    {
        Time.timeScale = 0f;

        if (inventoryToggle != null)
        {
            inventoryToggle.disableToggle = true;
            inventoryToggle.ShowInventoryUIOnly();
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        gameOverUI.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}