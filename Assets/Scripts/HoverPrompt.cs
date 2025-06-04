using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyHoverPrompt : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject uiPrompt;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private string message = "Press E to place currency";

    private GameObject currentTarget;

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, interactableLayer))
        {
            if (hit.collider.CompareTag("CurrencyDropZone"))
            {
                currentTarget = hit.collider.gameObject;
                uiPrompt.SetActive(true);
                Debug.Log("Hovering over drop zone");
                promptText.text = message;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    var dropZone = currentTarget.GetComponent<CurrencyDropZone>();
                    dropZone?.TryPlaceCurrency();
                }
                return;
            }
        }

        // Hide prompt when not hovering
        currentTarget = null;
        uiPrompt.SetActive(false);
    }
}
