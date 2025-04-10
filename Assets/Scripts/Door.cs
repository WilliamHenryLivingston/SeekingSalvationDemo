using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator newanimator;
    public Animator animator;
    private void Start()
    {
        

        
    }

    public void Update()
    {
        Debug.Log(newanimator == null);
    }

    public void Unlock()
    {
        Debug.Log("The door is now open.");
        newanimator.SetTrigger("OpenDoor"); 
    }
}