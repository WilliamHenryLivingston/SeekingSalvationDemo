using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtrillionGamesLtd
{
    public class GravityTrigger : MonoBehaviour
    {
        [SerializeField] private Vector3 gravityDirection;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<AtrillionGamesLtd_PlayerMove>() != null)
            {
                other.gameObject.GetComponent<AtrillionGamesLtd_PlayerMove>().setPlayerGravityDirection(gravityDirection);
            }
        }
    }
}
