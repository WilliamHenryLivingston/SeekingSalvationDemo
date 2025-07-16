using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KeyholdInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask keyholeLayer;

    private Keyhole currentKeyhole;


    void Update()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, keyholeLayer))
        {
            currentKeyhole = hit.collider.GetComponent<Keyhole>();

            if (currentKeyhole != null)
                currentKeyhole.ShowPrompt(true);

            if (Input.GetKeyDown(KeyCode.E))
                currentKeyhole?.TryInsertShard();
        }
        else
        {
            if (currentKeyhole != null)
            {
                currentKeyhole.ShowPrompt(false);
                currentKeyhole = null;
            }
        }
    }
}