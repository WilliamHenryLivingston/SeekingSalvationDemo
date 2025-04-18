using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyholeInteraction : MonoBehaviour
{
    public DoorOpen door; 
    public int requiredShards = 2;
    private int insertedShards = 0;
    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryInsertShard();
        }
    }

    void TryInsertShard()
    {
        var inventory = PlayerInventory.Instance;

        if (inventory.HasItem("KeyShard"))
        {
            inventory.RemoveItemByName("KeyShard");
            insertedShards++;

            Debug.Log($"Inserted shard {insertedShards}/{requiredShards}");

            if (insertedShards == requiredShards)
            {
                UnlockDoor();
            }
        }
        else
        {
            Debug.Log("No more key shards in inventory.");
        }
    }

    void UnlockDoor()
    {
        Debug.Log("Door unlocked!");
        door.Unlock();
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}