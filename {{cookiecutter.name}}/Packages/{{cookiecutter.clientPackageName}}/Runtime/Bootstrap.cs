using Common;
using UnityEngine;
using Wildlife.UnityLogger;
using Logger = Wildlife.Logging.Logger;
using Pitaya;
using TFG.Modules;
using UI;
using Wildlife.MetagameBase;

/// <summary>
/// Application entry point responsible for initializing dependencies and start the game.
/// </summary>
public class Bootstrap : MonoBehaviour
{
    private IInitializer _initializer;

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
        _initializer = new Initializer();

        // (Optional) In case device information is available to be set, DeviceInfo might be set here.  
        // Otherwise, a dummy standard device information is automatically set in environment context.
        // _initializer.environmentContext.deviceInfo = YourDeviceInfo;

        // Instantiates and initializes metagame modules for a minimal client.
        _initializer.InitializeMetagameBase();
        _initializer.InitializeMetagameConfiguration();
        _initializer.InitializeMetagameAuthentication();

        // Sets up a status event handler for the metagame client concerning client connection events.
        var handler = new MetagameClientStatusEventHandler();
        _initializer.MetagameBaseContext.Client.StatusHandler = handler;
        handler.OnConnectedEvent += OnConnected;
        handler.OnFailedToConnectEvent += OnConnectionFailed;
        handler.OnDisconnectedEvent += OnDisconnected;
        handler.OnReconnectedEvent += OnReconnected;
        handler.OnKickedEvent += OnKicked;
        
        // Sets up the MainView with the bare minimum for allowing metagame client connection events to be triggered.
        mainView.SetMetagameClient(_initializer.MetagameBaseContext.Client);

        // Starts the client by connecting the client to the metagame server and logging in the current player.
        Start();
    }

    /// <summary>
    /// Connects the metagame client to the metagame server and logs in the current player.
    /// This will trigger the OnConnectedEvent delegate (OnConnected method).
    /// </summary>
    void Start()
    {
        _initializer.MetagameBaseContext.Client.Connect();
    }

    /// <summary>
    /// Disconnects the metagame client from the metagame server and logs out the current player.
    /// This will trigger the OnDisconnectedEvent delegate (OnDisconnected method).
    /// </summary>
    void Stop()
    {
        _initializer.MetagameBaseContext.Client.Disconnect();
    }

    void OnDestroy()
    {
        Stop();
    }
    
    /// <summary>
    /// Evaluates the metagame client connection to the metagame server following OnConnectedEvent delegate.
    /// This will likely attempt to log in the player through the established connection to the metagame server.
    /// This method is just an example implementation and this might be customized by the game.
    /// Therefore, feel free to edit this method as desired.
    /// </summary>
    void OnConnected(IMetagameClient client)
    {
        Logger.LogInfo("Bootstrap::Connected > Metagame Client Connected! Attempt to Login client...");

        _initializer.AuthenticationContext.LoginClient.Login(player =>
        {
            Logger.LogInfo($"Logged In Player: < AccountID: {player.Id} > logged in!");
        }, errors =>
        {
            foreach (var er in errors)
            {
                Logger.LogError($"Bootstrap::Connected > Login Error [ {er.Code}: {er.Msg} ]");
            }
        });
    }

    /// <summary>
    /// Evaluates the metagame client reconnection to the metagame server following OnReconnectedEvent delegate.
    /// This will likely attempt to log in the player through the established connection to the metagame server.
    /// This method is just an example implementation and this might be customized by the game.
    /// Therefore, feel free to edit this method as desired.
    /// </summary>
    void OnReconnected(IMetagameClient client)
    {
        Logger.LogInfo("Bootstrap::Reconnected > Metagame Client Reconnected!");
        OnConnected(client);
    }

    /// <summary>
    /// Evaluates the metagame client connection failure to the metagame server following OnConnectionFailedEvent delegate.
    /// This might attempt to recover from a connection failure by retrying connection, entering an offline mode or
    /// warning the user about connection failure.
    /// This method is just an example implementation and this might be customized by the game.
    /// Therefore, feel free to edit this method as desired.
    /// </summary>
    void OnConnectionFailed(IMetagameClient client, NetworkError error)
    {
        Logger.LogError("Bootstrap::ConnectionFailed > Metagame Client Connection Failed!");
    }

    /// <summary>
    /// Evaluates the metagame disconnection from the metagame server following OnDisconnectedEvent delegate.
    /// This might attempt to recover from a disconnection by trying a reconnection, entering an offline mode,
    /// warning the user about it or closing the app.
    /// This method is just an example implementation and this might be customized by the game.
    /// Therefore, feel free to edit this method as desired.
    /// </summary>
    void OnDisconnected(IMetagameClient client, NetworkError error)
    {
        Logger.LogInfo("Bootstrap::Disconnected > Metagame Client Disconnected!");
    }

    /// <summary>
    /// Evaluates the client player being kicked out following OnKickedEvent delegate.
    /// This method is just an example implementation and this might be customized by the game.
    /// Therefore, feel free to edit this method as desired.
    /// </summary>
    void OnKicked(IMetagameClient client)
    {
        Logger.LogInfo("Bootstrap::Kicked > Player Kicked!");
    }
}