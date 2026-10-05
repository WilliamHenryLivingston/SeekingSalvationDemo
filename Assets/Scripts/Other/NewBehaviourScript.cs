//Copyright 2025 William Livingston
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    void Start()
    {
        Debug.Log(PowerOf(2, 2));
    }
    int PowerOf(int number, int power)
    {
        int value = number;
        for(int i = 1; i < power; i++)
        {
            value = value * number;
        }
        return value;
    }
}
