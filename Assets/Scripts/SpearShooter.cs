using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearShooter : MonoBehaviour
{
    public GameObject spearPrefab;
    public Transform firePoint;
    public float spearSpeed = 25f;

    public void FireSpear()
    {
        GameObject spear = Instantiate(spearPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = spear.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * spearSpeed;
        }
    }
}
