using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryUI;

    // Replace this with your actual look-control component if needed
    public MonoBehaviour lookControlScript;
    private bool isInventoryOpen = false;

    [HideInInspector] public bool disableToggle = false;

    void Start()
    {
        inventoryUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Debug.Log("Update is being called");
        if (disableToggle) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q key pressed");
            ToggleInventory();
        }
    }

    public void ShowInventoryUIOnly()
    {
        isInventoryOpen = true;
        inventoryUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SetLookEnabled(false);
    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            //Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SetLookEnabled(false);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            SetLookEnabled(true);
        }
    }

    private void SetLookEnabled(bool enabled)
    {
        if (lookControlScript == null) return;

        // Dynamically call SetLookEnabled(bool) if the component has it
        var method = lookControlScript.GetType().GetMethod("SetLookEnabled");
        if (method != null)
        {
            method.Invoke(lookControlScript, new object[] { enabled });
        }
        else
        {
            Debug.LogWarning("Look control script does not contain 'SetLookEnabled(bool)' method.");
        }
    }
}