using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.Properties;
using Unity.Services.Multiplayer;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartGameViewModel : IDisposable, IDataSourceViewHashProvider, INotifyBindablePropertyChanged
{
    private const string _mainLevel = "MainLevel";
    private SessionObserver _sessionObserver;
    private ISession _session;
    private bool _canStartGame;

    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

    [CreateProperty]
    public bool CanStartGame
    {
        get => _canStartGame;
        private set
        {
            if (_canStartGame == value)
            {
                return;
            }

            _canStartGame = value;
            Notify();
        }
    }

    public StartGameViewModel(string sessionType)
    {
        _sessionObserver = new SessionObserver(sessionType);
        _sessionObserver.SessionAdded += OnSessionAdded;

        if (_sessionObserver.Session != null)
        {
            
            OnSessionAdded(_sessionObserver.Session);
        }
    }

    void OnSessionAdded(ISession newSession)
    {
        _session = newSession;
        _session.RemovedFromSession += OnSessionRemoved;
        _session.Deleted += OnSessionRemoved;

        if (_session.IsHost == true)
        {
            CanStartGame = true;
        }
    }

    void OnSessionRemoved()
    {
        CanStartGame = false;
        CleanupSession();
    }

    void CleanupSession()
    {
        _session.RemovedFromSession -= OnSessionRemoved;
        _session.Deleted -= OnSessionRemoved;
        _session = null;
    }

    // Starts the game
    public async Task StartGame(string sessionType)
    {
        // Only host can start game
        if (_session.IsHost == false)
        {
            UnityEngine.Debug.Log("User is not the host and cannot start game");
            return;
        }

        NetworkManager.Singleton.SceneManager.LoadScene(_mainLevel,LoadSceneMode.Single);
    }

    public void Dispose()
    {
        if (_sessionObserver != null)
        {
            _sessionObserver.Dispose();
            _sessionObserver = null;
        }

        if (_session != null)
        {
            CleanupSession();
        }
    }

    public long GetViewHashCode() => _canStartGame ? 1 : 0;

    void Notify([CallerMemberName] string property = null)
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
    }

}
