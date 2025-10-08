//Copyright 2025 William Livingston
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgeInteraction : MonoBehaviour
{
    // The name of the required item for this interaction
    private const string REQUIRED_ITEM_NAME = "HedgeCutter";

    // Reference to the player's action script (or just their Transform)
    private Transform playerTransform;
    private bool playerIsNearby = false;

    // A flag to check if the hedge has already been cut
    private bool isCut = false;

    // Visuals/Audio (Optional)
    public GameObject cutParticlesPrefab;
    public AudioClip cutSound;

    // --- PLAYER DETECTION (Trigger Collider) ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            playerIsNearby = true;
            Debug.Log("Player near hedge. Press 'E' to cut.");
            // TODO: Show UI prompt here
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = null;
            playerIsNearby = false;
            // TODO: Hide UI prompt here
        }
    }

    // --- INTERACTION LOGIC ---
    private void Update()
    {
        // 1. Check if the player is nearby AND the hedge hasn't been cut yet
        if (playerIsNearby && !isCut)
        {
            // 2. Check for the 'E' input
            if (Input.GetKeyDown(KeyCode.E))
            {
                AttemptCutHedge();
            }
        }
    }

    private void AttemptCutHedge()
    {
        // 3. Use the PlayerInventory instance to check for the item
        if (PlayerInventory.Instance != null && PlayerInventory.Instance.HasItem(REQUIRED_ITEM_NAME))
        {
            // SUCCESS! 

            // 4. Consume the Hedgecutter
            bool removed = PlayerInventory.Instance.RemoveItemByName(REQUIRED_ITEM_NAME);

            if (removed)
            {
                CutHedge(); // Proceed to cut and despawn
            }
            else
            {
                Debug.LogError("Hedgecutter found in inventory but failed to remove it!");
            }
        }
        else
        {
            Debug.Log("You need a Hedgecutter to pass!");
            // TODO: Flash an on-screen message
        }
    }

    private void CutHedge()
    {
        isCut = true; // Prevents re-cutting

        // Play visual/audio effects
        if (cutParticlesPrefab != null)
        {
            Instantiate(cutParticlesPrefab, transform.position, Quaternion.identity);
        }
        if (cutSound != null)
        {
            AudioSource.PlayClipAtPoint(cutSound, transform.position);
        }

        // Despawn the hedge
        Destroy(gameObject);
        Debug.Log("Hedge cut! Path is clear.");
        // TODO: Hide UI prompt here immediately before destruction
    }
}