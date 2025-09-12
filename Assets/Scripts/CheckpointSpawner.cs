//Copyright 2025 William Livingston
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Tiles"))
        {

            Destroy(other.gameObject);
        }
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
