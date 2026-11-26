using UnityEngine;
using Unity.Netcode;
using Steamworks;
using Steamworks.Data;
using Netcode.Transports.Facepunch;
using System;

public class SteamSessionManager : MonoBehaviour
{
    public static SteamSessionManager Instance { get; private set; }

    [field: SerializeField] public uint AppID { get; private set; }
    [field: SerializeField] public int MaxMembers { get; private set; } = 4;

    private FacepunchTransport transport = null;

    public Lobby? currentLobby { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        try
        {
            Steamworks.SteamClient.Init(AppID, false);
            Debug.Log("Connected to Steam at: " + Steamworks.SteamClient.Name);
        }
        catch (Exception e)
        {
            Debug.Log("Cannot connect to steam??");
        }
    }

    private void Update()
    {
        Steamworks.SteamClient.RunCallbacks();
    }

    private void Start()
    {
        // Grabs transport
        transport = GetComponent<FacepunchTransport>();

        // Creates callbacks
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
    }

    private void OnDisable()
    {
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite -= OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated -= OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequested;


        Disconnected();

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallback;
        }

        Steamworks.SteamClient.Shutdown();
    }

    private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId id)
    {
        RoomEnter room = await lobby.Join();

        if (room != RoomEnter.Success)
        {
            Debug.Log("Failed to join lobby");
        }
        else
        {
            currentLobby = lobby;
            Debug.Log("Successfully joined lobby");
        }
    }

    private void OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId id)
    {
        Debug.Log("Lobby Created");
    }

    private void OnLobbyInvite(Friend friend, Lobby lobby)
    {
        Debug.Log("Invited friend: " + friend.Name);
    }

    private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
    {
        Debug.Log("Friend left lobby: " + friend.Name);
    }

    private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        Debug.Log("Friend joined lobby: " + friend.Name);
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            return;
        }
        StartClient(currentLobby.Value.Owner.Id);
    }

    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        if (result != Result.OK)
        {
            Debug.Log("Lobby failed to create");
            return;
        }

        lobby.SetPublic();
        lobby.SetJoinable(true);
        lobby.SetGameServer(lobby.Owner.Id);
    }

    public async void StartHost()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.StartHost();
        currentLobby = await SteamMatchmaking.CreateLobbyAsync(MaxMembers);
    }

    private void OnServerStarted()
    {
        Debug.Log("Host started :)");
    }

    public void StartClient(SteamId id)
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallback;
        transport.targetSteamId = id;
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client Started");
        }
    }

    private void OnClientDisconnectedCallback(ulong obj)
    {
        throw new NotImplementedException();
    }

    private void OnClientConnectedCallback(ulong obj)
    {
        throw new NotImplementedException();
    }

    public void Disconnected()
    {
        if (!SteamClient.IsValid)
        {
            Debug.Log("Steam is already closed");
            return;
        }

        currentLobby?.Leave();

        if (NetworkManager.Singleton == null)
        {
            return;
        }

        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        }
        else
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallback;
        }
        NetworkManager.Singleton.Shutdown(true);
    }


}
