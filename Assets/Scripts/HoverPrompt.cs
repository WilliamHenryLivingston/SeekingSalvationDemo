using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionHoverPrompt : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TMP_Text promptText;

    private MonoBehaviour lastInteractable = null;

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Handle Currency
            if (hitObject.CompareTag("CurrencyDropZone"))
            {
                var zone = hitObject.GetComponent<CurrencyDropZone>();
                if (zone != null)
                {
                    zone.ShowPrompt(true);
                    if (lastInteractable != null && lastInteractable != zone)
                        (lastInteractable as CurrencyDropZone)?.ShowPrompt(false);

                    lastInteractable = zone;

                    if (promptText != null)
                        promptText.text = "Press E to place currency";

                    if (Input.GetKeyDown(KeyCode.E))
                        zone.TryPlaceCurrency();

                    return;
                }
            }

            // Handle Keyhole
            if (hitObject.CompareTag("Keyhole"))
            {
                var keyhole = hitObject.GetComponent<Keyhole>();
                if (keyhole != null)
                {
                    keyhole.ShowPrompt(true);
                    if (lastInteractable != null && lastInteractable != keyhole)
                        (lastInteractable as Keyhole)?.ShowPrompt(false);

                    lastInteractable = keyhole;

                    if (promptText != null)
                        promptText.text = "Insert Keyshard Here";

                    if (Input.GetKeyDown(KeyCode.E))
                        keyhole.TryInsertShard();

                    return;
                }
            }
        }

        // Hide the previous prompt if no hit
        if (lastInteractable != null)
        {
            if (lastInteractable is CurrencyDropZone currency)
                currency.ShowPrompt(false);
            else if (lastInteractable is Keyhole keyhole)
                keyhole.ShowPrompt(false);

            lastInteractable = null;
        }
    }
}