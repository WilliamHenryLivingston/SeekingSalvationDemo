using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyDropZone : MonoBehaviour
{
    [SerializeField] private GameObject uiPrompt; // Reference to the worldspace UI prompt
    [SerializeField] private string promptText = "Press E to place currency";
    [SerializeField] private int cost = 1;

    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowPrompt(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            ShowPrompt(false);
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PlayerInventory inventory = FindObjectOfType<PlayerInventory>();

            if (inventory != null && inventory.currency >= cost)
            {
                inventory.currency -= cost;
                Debug.Log("Currency placed!");
                ShowPrompt(false);
            }
            else
            {
                Debug.Log("Not enough currency.");
            }
        }
    }

    // Called by raycast hover script to toggle the prompt
    public void ShowPrompt(bool show)
    {
        if (uiPrompt != null)
        {
            uiPrompt.SetActive(show);
            if (show)
            {
                Text text = uiPrompt.GetComponentInChildren<Text>();
                if (text != null)
                {
                    text.text = promptText;
                }
            }
        }
    }

    public void TryPlaceCurrency()
    {
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        if (inventory != null && inventory.coins >= 1)
        {
            inventory.coins -= 1;
            Debug.Log("Currency placed!");
            // Trigger upgrade or visual effect here
        }
        else
        {
            Debug.Log("Not enough currency.");
        }
    }
}