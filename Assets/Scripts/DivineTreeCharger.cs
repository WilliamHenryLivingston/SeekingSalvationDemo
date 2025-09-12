//Copyright 2025 William Livingston
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivineTreeCharger : MonoBehaviour
{
    public GameObject hiddenObject; // Assign in Inspector
    public int chargesNeeded = 300;

    private int currentCharges = 0;
    private bool playerInRange = false;
    private PlayerInventory playerInventory; // reference to inventory

    public DivineTreeCharger nextTree;
    public Animator treeAnimator;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInventory>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInventory = null;
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
        // Look for a shrine in range
        ShrineTable shrine = FindObjectOfType<ShrineTable>();
        if (shrine == null)
        {
            Debug.Log("No shrine found near tree.");
            return;
        }

        int spiritualEnergy = shrine.CollectStoredValue();
        if (spiritualEnergy <= 0)
        {
            Debug.Log("Nothing has been placed on the shrine.");
            return;
        }

        currentCharges += spiritualEnergy;

        Debug.Log($"Offered {spiritualEnergy} spiritual energy. Tree progress: {currentCharges}/{chargesNeeded}");

        if (currentCharges >= chargesNeeded)
        {
            ActivateTree();
        }
    }

    void ActivateTree()
    {
        Debug.Log("Divine Tree fully powered!");

        // Tell the next tree to grow
        if (nextTree != null && nextTree.treeAnimator != null)
        {
            nextTree.treeAnimator.SetTrigger("Grow");
        }

        // Reveal the shining path
        if (hiddenObject != null)
            hiddenObject.SetActive(true);
    }

    IEnumerator RevealAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hiddenObject != null)
            hiddenObject.SetActive(true);
    }
}