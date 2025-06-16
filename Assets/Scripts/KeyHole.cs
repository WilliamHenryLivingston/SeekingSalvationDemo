using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Keyhole : MonoBehaviour
{
    [SerializeField] private GameObject promptUI;

    public void ShowPrompt(bool show)
    {
        Debug.Log("Prompt UI: " + (promptUI ? "Found" : "Missing") + ", show = " + show);

        if (promptUI != null)
        {
            promptUI.SetActive(show);
            var text = promptUI.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.text = "Insert Keyshard Here";
            }
        }
    }

    public void TryInsertShard()
    {
        var inventory = PlayerInventory.Instance;

        if (inventory.HasItem("KeyShard"))
        {
            inventory.RemoveItemByName("KeyShard");

            Debug.Log("Inserted shard!");

            // You could also track how many shards here or notify a door system
            // You can expand this later to call KeyholeInteraction.UnlockDoor()
        }
        else
        {
            Debug.Log("No key shards in inventory.");
        }
    }
}