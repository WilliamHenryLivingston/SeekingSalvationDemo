using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add kill logic here
            Debug.Log("Player hit by spear!");
            Destroy(other.gameObject); // or call a player death method
        }

        // Destroy spear after hit
        Destroy(gameObject);
    }
}
