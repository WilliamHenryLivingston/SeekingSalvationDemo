//Copyright 2025 William Livingston
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Spear hit the player!");
            // Trigger your game over logic here
            GameManager.Instance.GameOver(); // example call
        }
    }
}
