using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Vector2 lastBreadcrumbXZ;
    [SerializeField] private LayerMask breadcrumbLayer;


    [Header("Breadcrumb Settings")]
    [SerializeField] private GameObject breadcrumbPrefab;
    [SerializeField] private float breadcrumbDistance = 10f;

    public List<GameObject> breadcrumbsTrail = new List<GameObject>();

    public Transform playerCamera;

    public AudioClip spawnSound;

    private bool canLook = true;

    private void Start()
    {
      
        lastBreadcrumbXZ = new Vector2(transform.position.x, transform.position.z);
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null && spawnSound != null)
        {
            audio.PlayOneShot(spawnSound);
        }
    }

    private void Update()
    {
        //Debug.Log("Update() is running");

        CheckBreadcrumbSpawn();
    }




    private void CheckBreadcrumbSpawn()
    {
        if (breadcrumbPrefab == null) return;

        Vector3 origin = transform.position + Vector3.up * 0.1f; // Slight offset to avoid clipping
        Vector3 spawnPos = GetGroundedPosition(origin);

        // Compare to last breadcrumb's position
        if (breadcrumbsTrail.Count > 0)
        {
            Vector3 lastPos = breadcrumbsTrail[breadcrumbsTrail.Count - 1].transform.position;
            float distance = Vector3.Distance(new Vector3(spawnPos.x, 0, spawnPos.z), new Vector3(lastPos.x, 0, lastPos.z));

            if (distance < breadcrumbDistance)
                return; // Too close — skip spawn
        }

        SpawnBreadcrumb(spawnPos);
    }
    private void SpawnBreadcrumb(Vector3 spawnPos)
    {
        GameObject breadcrumb = Instantiate(breadcrumbPrefab, spawnPos, Quaternion.identity);
        breadcrumbsTrail.Add(breadcrumb);
    }
    private Vector3 GetGroundedPosition(Vector3 origin)
    {
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 200f))
        {
            return hit.point;
        }

        // Fallback if nothing is hit
        return origin;
    }




    public void SetLookEnabled(bool enabled)
    {
        canLook = enabled;
    }

}