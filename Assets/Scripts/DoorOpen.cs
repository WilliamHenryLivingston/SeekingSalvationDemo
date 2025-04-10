using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public Animator newanimator;
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Unlock()
    {
        Debug.Log("The door is now open.");
        newanimator.SetTrigger("OpenDoor");
    }
}
