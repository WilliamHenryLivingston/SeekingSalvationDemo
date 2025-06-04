using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyDropZone : MonoBehaviour
{
    [SerializeField] private GameObject uiPrompt; //  This makes it visible in Inspector
    [SerializeField] private string promptText = "Press E to place currency";
    [SerializeField] private int cost = 1;

    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            uiPrompt.SetActive(true);
            uiPrompt.GetComponentInChildren<Text>().text = promptText;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            uiPrompt.SetActive(false);
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

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PlayerInventory inventory = FindObjectOfType<PlayerInventory>();

            if (inventory != null && inventory.currency >= cost)
            {
                inventory.currency -= cost;
                Debug.Log("Currency placed!");
                uiPrompt.SetActive(false);
            }
            else
            {
                Debug.Log("Not enough currency.");
            }
        }
    }
}