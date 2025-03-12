using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum PickupType {Health, SpeedBoost, Score}

    public PickupType pickupType;

    [SerializeField] int value = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>(); 

            if (player != null)
            {
                ApplyEffect(player);

                Destroy(gameObject);
            }
        }
    }

    private void ApplyEffect(PlayerController player)
    {
        switch (pickupType)
        {

            case PickupType.Health:
                player.IncreaseHealth(value); break;

            case PickupType.SpeedBoost: 
                player.IncreaseSpeed(value, 5f); break;

            case PickupType.Score:
                player.IncreaseScore(value); break;

        }
    }

}
