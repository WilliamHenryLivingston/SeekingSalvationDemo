//Copyright 2025 William Livingston
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Keyhole : MonoBehaviour
{
    [Header("UI & Animation")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openTriggerName = "Open";
    [SerializeField] private int requiredShards = 3;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource voiceSource; // The speaker
    [SerializeField] private AudioClip doorOpenVoiceLine; // The actual voice file

    private int insertedShards = 0;
    private bool doorOpened = false;

    // ... (ShowPrompt and TryInsertShard stay the same) ...

    public void ShowPrompt(bool show)
    {
        if (promptUI != null && !doorOpened)
        {
            promptUI.SetActive(show);
            var text = promptUI.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = "Insert Keyshard Here";
        }
    }

    public void TryInsertShard()
    {
        if (doorOpened) return;

        var inventory = PlayerInventory.Instance;
        if (inventory == null) return;

        if (inventory.HasItem("KeyShard"))
        {
            inventory.RemoveItemByName("KeyShard");
            insertedShards++;
            Debug.Log($"Shard inserted! ({insertedShards}/{requiredShards})");

            if (insertedShards >= requiredShards)
            {
                OpenDoor();
            }
        }
    }

    private void OpenDoor()
    {
        doorOpened = true;

        if (promptUI != null)
            promptUI.SetActive(false);

        // 1. Trigger Animation
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(openTriggerName);
            Debug.Log("All shards inserted. Door is opening.");
        }

        // 2. Trigger Voice Line
        if (voiceSource != null && doorOpenVoiceLine != null)
        {
            voiceSource.PlayOneShot(doorOpenVoiceLine);
        }
        else
        {
            Debug.LogWarning("Keyhole script is missing the AudioSource or AudioClip!");
        }
    }
}