using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator newanimator;
    private bool hasOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasOpened) return;

        if (other.CompareTag("Player"))
        {
            Unlock();
        }
    }

    public void Unlock()
    {
        hasOpened = true;

        if (newanimator != null)
        {
            newanimator.SetTrigger("OpenDoor");
            Debug.Log("The door is now open.");
        }
        else
        {
            Debug.LogWarning("No animator assigned to the door.");
        }
    }
}