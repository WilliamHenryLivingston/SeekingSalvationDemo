using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtrillionGamesLtd
{
    public class MovingPlatform : MonoBehaviour
    {

        [SerializeField] private Vector3 moveDirection;
        [SerializeField] private float moveSpeed;
        [SerializeField] public Vector3 rotateAxis;
        [SerializeField] public Vector3 rotateSpeed;
        [SerializeField] public AtrillionGamesLtd_PlayerMove player;
        [SerializeField] private float stepInMovement;
        
        void FixedUpdate()
        {
            transform.position += moveDirection * Time.fixedDeltaTime * moveSpeed * Mathf.Sin(stepInMovement*moveSpeed);
            Vector3 newRotateSpeed = new Vector3(rotateAxis.x*rotateSpeed.x,rotateAxis.y*rotateSpeed.y,rotateAxis.z*rotateSpeed.z) * Time.fixedDeltaTime;
            transform.Rotate(newRotateSpeed, Space.Self);
            if(player){
                player.setPlayerGravityDirection(transform.position - player.transform.position);
            }
            stepInMovement += Time.fixedDeltaTime;
            //transform.Rotate(xAngle, yAngle, zAngle, Space.World);
        }
    }
}