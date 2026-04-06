//Copyright 2025 William Livingston 
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
    private bool isActivated = false; // New: Prevents re-triggering
    private PlayerInventory playerInventory;

    public DivineTreeCharger nextTree;
    public Animator treeAnimator;

    [Header("Shrine Reference")]
    public ShrineTable shrine;

    [Header("Audio Settings")]
    public AudioSource voiceSource;
    public AudioClip shiningWayVoiceLine;

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
        // Don't run the logic if out of range, missing shrine, or already activated
        if (!playerInRange || shrine == null || isActivated) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            int offeredEnergy = shrine.OfferItems();
            if (offeredEnergy <= 0) return;

            currentCharges += offeredEnergy;
            Debug.Log($"Tree received {offeredEnergy} spiritual energy. Progress: {currentCharges}/{chargesNeeded}");

            if (currentCharges >= chargesNeeded)
            {
                ActivateTree();
            }
        }
    }

    void ActivateTree()
    {
        isActivated = true; // Mark as done so it doesn't trigger again
        Debug.Log("Divine Tree fully powered! The Shining Way is revealed.");

        // 1. Reveal the Shining Way
        if (hiddenObject != null)
        {
            hiddenObject.SetActive(true);
        }

        // 2. Play Animations
        if (treeAnimator != null)
            treeAnimator.SetTrigger("Grow");

        if (nextTree != null && nextTree.treeAnimator != null)
            nextTree.treeAnimator.SetTrigger("Grow");

        // 3. Trigger the Voice Line
        if (voiceSource != null && shiningWayVoiceLine != null)
        {
            voiceSource.PlayOneShot(shiningWayVoiceLine);
        }
        else
        {
            Debug.LogWarning("Audio missing on DivineTreeCharger!");
        }
    }
}