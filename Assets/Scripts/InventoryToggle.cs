using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryUI;
    public PlayerController playerController; // Reference to PlayerController script
    private bool isInventoryOpen = false;

    [HideInInspector] public bool disableToggle = false; // <- Add this

    void Start()
    {
        inventoryUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (disableToggle) return; // <- Block input if toggle is disabled

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ToggleInventory();
        }
    }

    public void ShowInventoryUIOnly()
    {
        isInventoryOpen = true;
        inventoryUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (playerController != null) playerController.SetLookEnabled(false);
    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (playerController != null) playerController.SetLookEnabled(false);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (playerController != null) playerController.SetLookEnabled(true);
        }
    }
}
