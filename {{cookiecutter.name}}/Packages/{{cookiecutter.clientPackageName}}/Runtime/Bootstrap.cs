using System.Collections.Generic;
using System.IO;
using Common;
using UnityEngine;
using Wildlife.UnityLogger;
using Logger = Wildlife.Logging.Logger;
using Pitaya;
using TFG.Modules;
using UI;
using Wildlife.Authentication;
using Wildlife.MetagameBase;

/// <summary>
/// Application entry point responsible for initializing dependencies and start the game.
/// </summary>
public class Bootstrap : MonoBehaviour
{
    private GameInitializer _gameInitializer;

    [SerializeField] private MainView mainView;

    /// <summary>
    /// Awake used as an Entry point. This is where your app will start.
    /// </summary>
    void Awake()
    {
        UnityMainThreadDispatcher.Instance().ShouldThrow = true;

        // Sets the UnityLogger into the Wildlife Logger, used on all modules.
        Logger.logger = new UnityLogger();

        // Sets the LogLevel for Pitaya interactions.
        StaticPitayaBinding.SetLogLevel(Defines.PitayaLogLevel);

        // Instantiates the initializer and creates a minimal environment for initialization.
        _gameInitializer = new GameInitializer();

        // (Optional) In case device information is available to be set, DeviceInfo might be set here.
        // Otherwise, a dummy standard device information is automatically set in shared context.
        // _initializer.environmentContext.deviceInfo = YourDeviceInfo;

        // Instantiates and initializes metagame modules for a minimal client.
        _gameInitializer.InitializeMetagameBase();
        _gameInitializer.InitializeMetagameConfiguration();
        _gameInitializer.InitializeMetagameAuthentication();

        // Sets up the MainView with the bare minimum for allowing metagame client connection events to be triggered.
        mainView.SetMetagameClient(_gameInitializer.MetagameBaseContext.Client);

        // Starts the client by connecting the client to the metagame server and logging in the current player.
        Start();
    }

    /// <summary>
    /// Connects the metagame client to the metagame server and logs in the current player.
    /// This will trigger the OnConnectedEvent delegate (OnConnected method).
    /// </summary>
    void Start()
    {
        _gameInitializer.StartGame();
    }

    /// <summary>
    /// Disconnects the metagame client from the metagame server and logs out the current player.
    /// This will trigger the OnDisconnectedEvent delegate (OnDisconnected method).
    /// </summary>
    void Stop()
    {
        _gameInitializer.StopGame();
    }

    void OnDestroy()
    {
        Stop();
    }
}