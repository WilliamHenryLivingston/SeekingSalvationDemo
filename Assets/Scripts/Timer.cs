using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public float timeRemaining = 60f; // Set starting time in seconds
    public TextMeshProUGUI timerText; // Reference to the timer text
    public GameObject loseScreen; // Reference to the lose screen

    private bool timerIsRunning = false;

    [SerializeField] private GameObject gameOverUI;
    public InventoryToggle inventoryToggle;

    private void Start()
    {
        timerIsRunning = true;
        loseScreen.SetActive(false); // Hide lose screen at start
        UpdateTimerUI(); // Initialize display
    }

    public void ResetTimer()
    {
        timeRemaining = 600f; // Reset to full time (or make this configurable)
        UpdateTimerUI();
        Debug.Log("Timer reset by Divine Tree.");
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerUI();
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                EndGame();
            }
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    void EndGame()
    {
        Time.timeScale = 0f;

        if (inventoryToggle != null)
        {
            inventoryToggle.disableToggle = true;
            inventoryToggle.ShowInventoryUIOnly();
        }

        gameOverUI.SetActive(true);
    }
}