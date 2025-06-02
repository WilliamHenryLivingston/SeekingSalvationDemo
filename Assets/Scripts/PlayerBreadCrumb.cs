using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBreadcrumbTrail : MonoBehaviour
{
    [Header("Breadcrumb Settings")]
    [SerializeField] private GameObject breadcrumbPrefab;
    [SerializeField] private float breadcrumbDistance = 10f;
    [SerializeField] private LayerMask breadcrumbLayer;
    public AudioClip spawnSound;

    public List<GameObject> breadcrumbsTrail = new List<GameObject>();

    public Transform playerCamera;

    private Vector3 lastBreadcrumbPosition;
    private bool canLook = true;

    private void Start()
    {
        lastBreadcrumbPosition = transform.position;

        // Play sound on start if available
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null && spawnSound != null)
        {
            audio.PlayOneShot(spawnSound);
        }

        // Spawn first breadcrumb immediately
        Vector3 spawnPos = GetGroundedPosition(transform.position);
        SpawnBreadcrumb(spawnPos);
    }

    private void Update()
    {
        CheckBreadcrumbSpawn();
    }

    private void CheckBreadcrumbSpawn()
    {
        if (breadcrumbPrefab == null) return;

        float distanceTraveled = Vector3.Distance(transform.position, lastBreadcrumbPosition);
        if (distanceTraveled >= breadcrumbDistance)
        {
            Vector3 spawnPos = GetGroundedPosition(transform.position);
            SpawnBreadcrumb(spawnPos);
            lastBreadcrumbPosition = transform.position;
        }
    }

    private void SpawnBreadcrumb(Vector3 spawnPos)
    {
        GameObject breadcrumb = Instantiate(breadcrumbPrefab, spawnPos, Quaternion.identity);
        breadcrumbsTrail.Add(breadcrumb);
    }

    private Vector3 GetGroundedPosition(Vector3 origin)
    {
        // Slight vertical offset to help raycast avoid terrain clipping
        origin += Vector3.up * 5f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 10f))
        {
            return hit.point;
        }

        // Fallback if ground not found
        return origin;
    }

    public void SetLookEnabled(bool enabled)
    {
        canLook = enabled;
    }
}