using System;
using System.Collections.Generic;
using System.IO;
using Common;
using Contexts;
using Contexts.MetagameModules;
using Pitaya;
using TFG.Modules;
using UnityEngine;
using Wildlife.Authentication;
using Wildlife.Configuration;
using Wildlife.MetagameBase;
using Wildlife.MetagameBase.DeviceInfo;
using Wildlife.Persistency;
using JsonSerializer = Wildlife.Persistency.Json.JsonSerializer;
using Logger = Wildlife.Logging.Logger;
using ReadOnlyPlayer = Wildlife.MetagameModuleAbstractions.Player;

public class GameInitializer
{
    // Shared Context.
    private SharedContext _sharedContext;

    // Minimal Metagame Modules Contexts.
    private MetagameBaseContext   _metagameBaseContext;
    private ConfigurationContext  _metagameConfigurationContext;
    private AuthenticationContext _metagameAuthenticationContext;

    public GameInitializer()
    {
        _sharedContext = new SharedContext();

        // Sets default dummy DeviceInfo into the shared context here.
        _sharedContext.DeviceInfo = new DeviceInfo()
        {
            FIU = Guid.NewGuid().ToString(),
            BundleId = "com.bundle.example",
            DeviceType = "DeviceType",
            OSVersion = "1.0",
            Region = "Region",
            Language = "Language",
            AdId = "AdId",
        };

        _sharedContext.PitayaQueueDispatcher = MainQueueDispatcher.Create();
        _sharedContext.Logger = Logger.logger;
        _sharedContext.AnalyticsTfg = new NullTFGAnalytics();
        _sharedContext.Serializer = new JsonSerializer();

        // Retrieves the persistent data directory (Read Only) and create this directory if it does not exist yet.
        var root = Application.persistentDataPath;
        Directory.CreateDirectory(root);

        // Sets the paths to the persistent account data and the directory holding all configs (i.e. store config).
        _sharedContext.AccountDataFilePath = $"{root}/account.json";
        _sharedContext.ConfigurationDataDirectoryPath = $"{root}/configs";
        Directory.CreateDirectory(_sharedContext.ConfigurationDataDirectoryPath);

        // Create file persistence for account data based on the provided account file path.
        _sharedContext.AccountFilePersistency = new FilePersistency(_sharedContext.AccountDataFilePath, _sharedContext.Serializer);

        // Create file persistence for configuration data based on the provided configuration file path.
        _sharedContext.ConfigurationFilePersistency = new FilePersistency(_sharedContext.ConfigurationDataDirectoryPath, _sharedContext.Serializer);

        _sharedContext.ThreadDispatcher = ThreadDispatcher.defaultDispatcher;
    }

    public SharedContext SharedContext => _sharedContext;

    public MetagameBaseContext MetagameBaseContext => _metagameBaseContext;

    public ConfigurationContext ConfigurationContext => _metagameConfigurationContext;

    public AuthenticationContext AuthenticationContext => _metagameAuthenticationContext;

    public void InitializeMetagameBase()
    {
        _metagameBaseContext = new MetagameBaseContext();

        _metagameBaseContext.HookSerializer = new HookProtobufSerializer();
        _metagameBaseContext.RequestFactory = new MetagameRequestFactory(_sharedContext.ThreadDispatcher, _sharedContext.Logger);

        _metagameBaseContext.DeviceInfoRetriever = _sharedContext.DeviceInfo;

        var metagameConfig = new MetagameConfig
        {
            Host = Defines.EnvironmentData.MetagameUrl,
            Port = Defines.EnvironmentData.MetagamePort,
        };

        _metagameBaseContext.ClientBuilder = new MetagameClientBuilder(metagameConfig, new PitayaConfig(), _metagameBaseContext.DeviceInfoRetriever)
        {
            Dispatcher = _sharedContext.PitayaQueueDispatcher,
        };

        _metagameBaseContext.Client = (MetagameClient) _metagameBaseContext.ClientBuilder.Build();

        _metagameBaseContext.RequestHandler = new MetagameRequestHandler(
            _metagameBaseContext.Client,
            _metagameBaseContext.RequestFactory,
            _sharedContext.ThreadDispatcher,
            _sharedContext.Logger);

        // Sets up a status event handler for the metagame client concerning client connection events.
        var handler = new MetagameClientStatusEventHandler();
        _metagameBaseContext.Client.StatusHandler = handler;
        handler.OnConnectedEvent += OnConnected;
        handler.OnFailedToConnectEvent += OnConnectionFailed;
        handler.OnDisconnectedEvent += OnDisconnected;
        handler.OnReconnectedEvent += OnReconnected;
        handler.OnKickedEvent += OnKicked;
    }

    public void InitializeMetagameConfiguration()
    {
        _metagameConfigurationContext = new ConfigurationContext();

        _metagameConfigurationContext.HooksBuilder = new ConfigurationHooksBuilder(_metagameBaseContext.HookSerializer);

        var configInfos = new ConfigInfo[]
        {
            // Required configs must be added here (i.e. config for installed modules).
        };

        _metagameConfigurationContext.Client = new MetagameConfigurationClient(
            configInfos,
            _metagameBaseContext.Client,
            _sharedContext.ConfigurationFilePersistency,
            _metagameConfigurationContext.HooksBuilder);
    }

    public void InitializeMetagameAuthentication()
    {
        _metagameAuthenticationContext = new AuthenticationContext();

        _metagameAuthenticationContext.HooksBuilder    = new AuthenticationHooksBuilder(_metagameBaseContext.HookSerializer);

        _metagameAuthenticationContext.Client      = new MetagameAuthenticationClient(_metagameBaseContext.Client, _sharedContext.AccountFilePersistency, _metagameAuthenticationContext.HooksBuilder);
        _metagameAuthenticationContext.LoginClient = new MetagameLoginClient(_metagameAuthenticationContext.Client, _metagameConfigurationContext.Client, _sharedContext.AccountFilePersistency);

        _metagameAuthenticationContext.ReadOnlyPlayer = _sharedContext.AccountFilePersistency.Read<ReadOnlyPlayer>();
    }

    /// <summary>
    /// Starts the game. Connects the metagame client to the metagame server and logs in the current player.
    /// This will trigger the OnConnectedEvent delegate (OnConnected method).
    /// </summary>
    public void StartGame()
    {
        MetagameBaseContext.Client.Connect();
    }

    /// <summary>
    /// Stops the game. Disconnects the metagame client from the metagame server and logs out the current player.
    /// This will trigger the OnDisconnectedEvent delegate (OnDisconnected method).
    /// </summary>
    public void StopGame()
    {
        MetagameBaseContext.Client.Disconnect();
        MetagameBaseContext.Client.Destroy();
    }

    async void RequestPlayerAccount(IMetagameClient metagameClient)
    {
        Player readPlayer = _sharedContext.AccountFilePersistency.Read<Player>();
        if (readPlayer != null)
        {
            var (player, error) = await _metagameAuthenticationContext.Client.Authenticate();
            if(error == null) {
                Logger.LogInfo($"Authenticated Player: < AccountID: {player.Id} > logged in!");
            }
            else
            {
                Logger.LogError($"Bootstrap::Connected > Authentication Login Error [ {error.Code}: {error.Msg} ]");
                metagameClient.ForceDisconnect();
            }
        }
        else
        {
            var (player, error) = await _metagameAuthenticationContext.Client.Create();
            if(error == null)
            {
                Logger.LogInfo($"New Player: < AccountID: {player.Id} > logged in!");
            }
            else
            {
                Logger.LogError($"Bootstrap::Connected > Player Creation Login Error [ {error.Code}: {error.Msg} ]");
                metagameClient.ForceDisconnect();
            }
        }
    }

    /// <summary>
    /// Evaluates the metagame client connection to the metagame server following OnConnectedEvent delegate.
    /// This will likely attempt to log in the player through the established connection to the metagame server.
    /// This method is just an example implementation and this might be customized by the game.
    /// Therefore, feel free to edit this method as desired.
    /// </summary>
    public virtual void OnConnected(IMetagameClient client)
    {
        Logger.LogInfo("Bootstrap::Connected > Metagame Client Connected! Attempt to Login client...");

        RequestPlayerAccount(client);
        /*_initializer.AuthenticationContext.LoginClient.Login(player =>
        {
            Logger.LogInfo($"Logged In Player: < AccountID: {player.Id} > logged in!");
        }, errors =>
        {
            foreach (var er in errors)
            {
                Logger.LogError($"Bootstrap::Connected > Login Error [ {er.Code}: {er.Msg} ]");
            }
            client.Disconnect();
        });*/
    }

    /// <summary>
    /// Evaluates the metagame client reconnection to the metagame server following OnReconnectedEvent delegate.
    /// This will likely attempt to log in the player through the established connection to the metagame server.
    /// This method is just an example implementation and this might be customized by the game.
    /// Therefore, feel free to edit this method as desired.
    /// </summary>
    public virtual void OnReconnected(IMetagameClient client)
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
    public virtual void OnConnectionFailed(IMetagameClient client, Pitaya.NetworkError error)
    {
        var fields = new Dictionary<string, object>();
        if (error != null)
        {
            fields.Add(error.Error, error);
        }

        Logger.WithFields(fields).LogError("Bootstrap::ConnectionFailed > Metagame Client Connection Failed!");
    }

    /// <summary>
    /// Evaluates the metagame disconnection from the metagame server following OnDisconnectedEvent delegate.
    /// This might attempt to recover from a disconnection by trying a reconnection, entering an offline mode,
    /// warning the user about it or closing the app.
    /// This method is just an example implementation and this might be customized by the game.
    /// Therefore, feel free to edit this method as desired.
    /// </summary>
    public virtual void OnDisconnected(IMetagameClient client, Pitaya.NetworkError error)
    {
        var fields = new Dictionary<string, object>();
        if (error != null)
        {
            fields.Add(error.Error, error);
        }

        Logger.WithFields(fields).LogInfo("Bootstrap::Disconnected > Metagame Client Disconnected!");
    }

    /// <summary>
    /// Evaluates the client player being kicked out following OnKickedEvent delegate.
    /// This method is just an example implementation and this might be customized by the game.
    /// Therefore, feel free to edit this method as desired.
    /// </summary>
    public virtual void OnKicked(IMetagameClient client)
    {
        Logger.LogInfo("Bootstrap::Kicked > Player Kicked!");
    }
}