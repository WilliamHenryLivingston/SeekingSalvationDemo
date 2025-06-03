using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Vector3 worldPointA;
    private Vector3 worldPointB;
    private Vector3 targetPosition;
    private bool movingToB = true;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("MovingPlatform: PointA or PointB not assigned.");
            enabled = false;
            return;
        }

        // Cache the global positions
        worldPointA = pointA.position;
        worldPointB = pointB.position;

        targetPosition = worldPointB;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            movingToB = !movingToB;
            targetPosition = movingToB ? worldPointB : worldPointA;
        }
    }
}

