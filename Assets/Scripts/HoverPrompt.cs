using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyHoverPrompt : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TMP_Text promptText; // You can keep this if you want a shared prompt text object

    private CurrencyDropZone lastZone = null;

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, interactableLayer))
        {
            if (hit.collider.CompareTag("CurrencyDropZone"))
            {
                CurrencyDropZone zone = hit.collider.GetComponent<CurrencyDropZone>();
                if (zone != null)
                {
                    // Show this zone's prompt
                    zone.ShowPrompt(true);

                    // Hide the previous one if different
                    if (lastZone != null && lastZone != zone)
                    {
                        lastZone.ShowPrompt(false);
                    }

                    lastZone = zone;

                    // Update shared prompt text if you want, otherwise each drop zone sets its own text
                    if (promptText != null)
                    {
                        promptText.text = "Press E to place currency";
                    }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        zone.TryPlaceCurrency();
                    }

                    return;
                }
            }
        }

        // No hit or not a drop zone — hide last prompt
        if (lastZone != null)
        {
            lastZone.ShowPrompt(false);
            lastZone = null;
        }
    }
}