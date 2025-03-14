using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum PickupType {Health, SpeedBoost, Energy}

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
                player.IncreaseSpeed(1, 5000000000000000000f); break;

            case PickupType.Energy:
                player.IncreaseScore(value); break;

        }
    }

}
