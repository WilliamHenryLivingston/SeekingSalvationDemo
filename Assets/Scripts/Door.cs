using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Animator not found on Door!", this);    }

    public void Unlock()
    {
        Debug.Log("The door is now open.");
        animator.SetTrigger("Open"); // Make sure this trigger exists in your Animator
    }
}