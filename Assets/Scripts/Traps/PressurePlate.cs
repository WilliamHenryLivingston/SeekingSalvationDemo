//Copyright 2025 William Livingston
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public SpearShooter spearShooter;
    public float delayBeforeFire = 0.3f; // Delay in seconds

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return; // Prevent double-triggering
        if (other.CompareTag("Player"))
        {
            triggered = true;
            Invoke(nameof(FireSpear), delayBeforeFire);
        }
    }

    private void FireSpear()
    {
        spearShooter?.FireSpear();
    }
}