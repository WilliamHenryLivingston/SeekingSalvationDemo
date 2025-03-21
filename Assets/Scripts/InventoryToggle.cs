using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryUI;
    public PlayerController playerController; //Reference to PlayerController script
    private bool isInventoryOpen = false;

    void Start()
    {
        inventoryUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;


    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.LeftShift)) //when player presses shift
        {
            ToggleInventory();//Show/hide Inventory
        }

    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen; 
        inventoryUI.SetActive(isInventoryOpen);

        if (isInventoryOpen )
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (playerController != null) playerController.SetLookEnabled(false);//Disable camera movement
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (playerController != null) playerController.SetLookEnabled(true);//Enable camera movement
        }
    }
}
