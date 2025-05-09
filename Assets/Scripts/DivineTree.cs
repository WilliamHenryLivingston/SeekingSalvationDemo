using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivineTree : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameTimer timer = FindObjectOfType<GameTimer>();
            if (timer != null)
            {
                timer.ResetTimer();
            }
        }

        if (other.CompareTag("Player"))
        {
            // Despawn the enemy
            GameObject enemy = GameObject.FindWithTag("Enemy");
            if (enemy != null)
            {
                enemy.GetComponent<TrackerEnemy>().Despawn();
                Debug.Log("Something has lost your scent");
            }
        }
    }
}