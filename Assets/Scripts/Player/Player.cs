//Copyright 2025 William Livingston
using AtrillionGamesLtd;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class Player : NetworkBehaviour
{
    public ulong ClientID {  get; private set; }
    public AtrillionGamesLtd_PlayerMove PlayerMovement { get; private set; }
    public PlayerInput Input { get; private set; }

    private void Awake()
    {
        Input = GetComponent<PlayerInput>();
        PlayerMovement = GetComponent<AtrillionGamesLtd_PlayerMove>();

        Input.enabled = false;
    }


    // Enables input for player
    public override void OnNetworkSpawn()
    {
        //Debug.Log("Player sets up: " + ClientID, this);
        Input.enabled = IsOwner;
        base.OnNetworkSpawn();
    }

    // Disables input
    public override void OnNetworkDespawn()
    {
        //Debug.Log("Player disabled: " + ClientID);
        Input.enabled = false;

        base.OnNetworkDespawn();
    }

    public void SetUpPlayerId(ulong id)
    {
        ClientID = id;
    }
}
