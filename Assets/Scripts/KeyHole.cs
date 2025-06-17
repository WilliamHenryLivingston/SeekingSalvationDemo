using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Keyhole : MonoBehaviour
{
    [SerializeField] private GameObject promptUI;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openTriggerName = "Open";
    [SerializeField] private int requiredShards = 3;

    private int insertedShards = 0;
    private bool doorOpened = false;

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

        if (inventory != null && inventory.HasItem("KeyShard"))
        {
            inventory.RemoveItemByName("KeyShard");
            insertedShards++;

            Debug.Log($"Shard inserted! ({insertedShards}/{requiredShards})");

            if (insertedShards >= requiredShards)
            {
                OpenDoor();
            }
        }
        else
        {
            Debug.Log("No key shards in inventory.");
        }
    }

    private void OpenDoor()
    {
        doorOpened = true;

        if (promptUI != null)
            promptUI.SetActive(false);

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(openTriggerName);
            Debug.Log("All shards inserted. Door is opening.");
        }
    }
}