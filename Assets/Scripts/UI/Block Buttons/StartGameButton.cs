using System;
using Blocks.Common;
using Unity.Properties;
using UnityEngine.UIElements;
using UnityEngine;

[UxmlElement]
public partial class StartGameButton : Button
{
    private const string _startGameButtonText = "Start Game";
    private string _sessionType;
    private DataBinding _dataBinding;
    private StartGameViewModel _viewModel;

    [CreateProperty, UxmlAttribute]
    public string SessionType
    {
        get => _sessionType;
        set
        {
            if (_sessionType == value)
            {
                return;
            }

            _sessionType = value;
            if (panel != null)
            {
                UpdateBindings();
            }
        }
    }

    public StartGameButton()
    {
        text = _startGameButtonText;

        // Binds button
        AddToClassList(BlocksTheme.Button);
        _dataBinding = new DataBinding() { dataSourcePath = new PropertyPath(nameof(StartGameViewModel.CanStartGame)), bindingMode = BindingMode.ToTarget };
        SetBinding(new BindingId(nameof(enabledSelf)), _dataBinding);
        clicked += StartGame;

        RegisterCallback<AttachToPanelEvent>(_ => UpdateBindings());
        RegisterCallback<DetachFromPanelEvent>(_ => CleanupBindings());
    }

    // Calls for view model to start game
    void StartGame()
    {
        _ = _viewModel.StartGame(_sessionType);
    }

    private void UpdateBindings()
    {
        // Creates new View Model for button
        CleanupBindings();
        _viewModel = new StartGameViewModel(_sessionType);
        _dataBinding.dataSource = _viewModel;
    }

    void CleanupBindings()
    {
        if (_dataBinding.dataSource is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _viewModel = null;
        _dataBinding.dataSource = null;
    }
}
