//Copyright 2025 William Livingston
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public Animator newanimator;

    [Header("Audio Settings")]
    public AudioSource voiceAudioSource; // Drag your AudioSource here
    public AudioClip divineTreeVoiceLine; // Drag your .wav or .mp3 here

    public void Unlock()
    {
        Debug.Log("The door is now open.");

        // Trigger the animation
        if (newanimator != null)
        {
            newanimator.SetTrigger("OpenDoor");
        }

        // Trigger the voice line
        PlayVoiceLine();
    }

    private void PlayVoiceLine()
    {
        if (voiceAudioSource != null && divineTreeVoiceLine != null)
        {
            // PlayOneShot is great for voice lines because it won't 
            // cut off other sounds on the same source
            voiceAudioSource.PlayOneShot(divineTreeVoiceLine);
        }
        else
        {
            Debug.LogWarning("Missing AudioSource or AudioClip on DoorOpen script!");
        }
    }
}