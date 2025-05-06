using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResetter : MonoBehaviour
{
    public void ResetGame()
    {
        Time.timeScale = 1f; // In case you paused the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}