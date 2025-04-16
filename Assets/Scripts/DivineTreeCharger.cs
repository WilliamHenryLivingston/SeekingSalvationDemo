using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivineTreeCharger : MonoBehaviour
{
    public GameObject hiddenObject; // Assign in Inspector
    public int chargesNeeded = 5;

    private int currentCharges = 0;
    private bool playerInRange = false;
    private GameObject player;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryChargeTree();
        }
    }

    void TryChargeTree()
    {
        var inventory = PlayerInventory.Instance;
        if (inventory.HasItem("Energy"))
        {
            inventory.RemoveItemByName("Energy");
            currentCharges++;

            Debug.Log($"Divine Tree charged! ({currentCharges}/{chargesNeeded})");

            if (currentCharges == chargesNeeded)
            {
                ActivateTree();
            }
        }
        else
        {
            Debug.Log("You need energy to power the tree.");
        }
    }

    void ActivateTree()
    {
        Debug.Log("Divine Tree fully powered!");
        if (hiddenObject != null)
            hiddenObject.SetActive(true);
    }
}
