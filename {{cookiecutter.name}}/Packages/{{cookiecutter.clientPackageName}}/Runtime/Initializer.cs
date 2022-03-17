using System;
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

public interface IInitializer
{
    SharedContext SharedContext { get; }
    MetagameBaseContext MetagameBaseContext { get; }
    ConfigurationContext ConfigurationContext { get; }
    AuthenticationContext AuthenticationContext { get; }

    void InitializeMetagameBase();
    void InitializeMetagameConfiguration();
    void InitializeMetagameAuthentication();
}

public class Initializer : IInitializer
{
    // Shared Context.
    private SharedContext _sharedContext;

    // Minimal Metagame Modules Contexts.
    private MetagameBaseContext   _metagameBaseContext;
    private ConfigurationContext  _metagameConfigurationContext;
    private AuthenticationContext _metagameAuthenticationContext;

    public Initializer()
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
            
        _metagameBaseContext.HookSerializer      = new HookProtobufSerializer();
        _metagameBaseContext.RequestFactory      = new MetagameRequestFactory(_sharedContext.ThreadDispatcher, _sharedContext.Logger);

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
}