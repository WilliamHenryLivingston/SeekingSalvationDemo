//Copyright 2025 William Livignston 
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DivineTreeCharger : MonoBehaviour
{
    [Header("Tree Settings")]
    public GameObject hiddenObject;
    public int chargesNeeded = 10;

    private int currentCharges = 0;
    private bool playerInRange = false;
    private PlayerInventory playerInventory;

    public DivineTreeCharger nextTree;
    public Animator treeAnimator;

    [Header("Shrine Reference")]
    public ShrineTable shrine; // Assign your shrine here

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
        if (!playerInRange || shrine == null) return;

        // Offer items from shrine to tree
        if (Input.GetKeyDown(KeyCode.R))
        {
            int offeredEnergy = shrine.OfferItems();
            if (offeredEnergy <= 0) return;

            currentCharges += offeredEnergy;
            Debug.Log($"Tree received {offeredEnergy} spiritual energy. Progress: {currentCharges}/{chargesNeeded}");

            if (currentCharges >= chargesNeeded)
                ActivateTree();
        }
    }

    void ActivateTree()
    {
        Debug.Log("Divine Tree fully powered!");

        if (treeAnimator != null)
            treeAnimator.SetTrigger("Grow");

        if (nextTree != null && nextTree.treeAnimator != null)
            nextTree.treeAnimator.SetTrigger("Grow");

        if (hiddenObject != null)
            hiddenObject.SetActive(true);
    }
}