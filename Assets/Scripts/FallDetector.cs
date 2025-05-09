using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDetector : MonoBehaviour
{
    public float fallThreshold = -20f;

    void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            Debug.Log("Player fell off the map!");
            GameManager.Instance.GameOver();
            Destroy(this);
        }
    }
}