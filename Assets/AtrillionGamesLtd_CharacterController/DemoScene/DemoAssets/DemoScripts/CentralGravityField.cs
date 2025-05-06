using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtrillionGamesLtd
{
    public class CentralGravityField : MonoBehaviour
    {
        [SerializeField] private Vector3 defaultGravityDirection;
        [SerializeField] private Vector3 gravityDirection;

        void OnTriggerStay(Collider other)
        {
            if (other.gameObject.GetComponent<AtrillionGamesLtd_PlayerMove>() != null)
            {
                other.gameObject.GetComponent<AtrillionGamesLtd_PlayerMove>().setPlayerGravityDirection(transform.position - other.transform.position);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<AtrillionGamesLtd_PlayerMove>() != null)
            {
                other.gameObject.GetComponent<AtrillionGamesLtd_PlayerMove>().setPlayerGravityDirection(defaultGravityDirection);
            }
        }
    }
}