//Copyright 2025 William Livingston
using AtrillionGamesLtd;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    public ulong ClientID {  get; private set; }
    public AtrillionGamesLtd_PlayerMove PlayerMovement { get; private set; }
    public PlayerInput Input { get; private set; }


    [field: SerializeField] public AudioListener AudioSource { get; private set; }
    [field: SerializeField] public Camera PlayerCamera { get; private set; }

    private void Awake()
    {
        Input = GetComponent<PlayerInput>();
        PlayerMovement = GetComponent<AtrillionGamesLtd_PlayerMove>();

        Input.enabled = false;
        AudioSource.enabled = false;
        PlayerMovement.enabled = false;
        PlayerCamera.enabled = false;
    }

    // Enables input for player
    public override void OnNetworkSpawn()
    {
        Debug.Log("Player sets up: " + ClientID, this);
        Input.enabled = IsOwner;
        AudioSource.enabled = IsOwner;
        PlayerMovement.enabled = IsOwner;
        PlayerCamera.enabled = IsOwner;
        base.OnNetworkSpawn();
    }

    // Disables input
    public override void OnNetworkDespawn()
    {
        Debug.Log("Player disabled: " + ClientID);
        Input.enabled = false;
        AudioSource.enabled = false;
        PlayerMovement.enabled = false;
        PlayerCamera.enabled = false;

        base.OnNetworkDespawn();
    }

    public void SetUpPlayerId(ulong id)
    {
        ClientID = id;
    }
}
