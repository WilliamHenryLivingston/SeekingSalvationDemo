using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtrillionGamesLtd
{
    public class GravityField : MonoBehaviour
    {
        [SerializeField] private bool useGravityDirection;
        [SerializeField] private Vector3 defaultGravityDirection;
        [SerializeField] private Vector3 gravityDirection;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<AtrillionGamesLtd_PlayerMove>() != null && useGravityDirection)
            {
                other.gameObject.GetComponent<AtrillionGamesLtd_PlayerMove>().setPlayerGravityDirection(gravityDirection);
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (other.gameObject.GetComponent<AtrillionGamesLtd_PlayerMove>() != null && !useGravityDirection)
            {
                other.gameObject.GetComponent<AtrillionGamesLtd_PlayerMove>().setPlayerGravityDirection(-transform.up);
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