using System;
using System.IO;
using Common;
using Contexts;
using Contexts.MetagameModules;
using Services.MetagameModules.Authentication;
using Pitaya;
using TFG.Modules;
using UnityEngine;
using Wildlife.Authentication;
using Wildlife.Configuration;
using Wildlife.MetagameBase;
using Wildlife.Persistency;
using JsonSerializer = Wildlife.Persistency.Json.JsonSerializer;
using Logger = Wildlife.Logging.Logger;
using ReadOnlyPlayer = Wildlife.MetagameModuleAbstractions.Player;

public interface IInitializer
{
    EnvironmentContext EnvironmentContext { get; }
    MetagameBaseContext MetagameBaseContext { get; }
    ConfigurationContext ConfigurationContext { get; }
    AuthenticationContext AuthenticationContext { get; }

    void InitializeMetagameBase();
    void InitializeMetagameConfiguration();
    void InitializeMetagameAuthentication();
}

public class Initializer : IInitializer
{
    // Environment Context.
    private EnvironmentContext _environmentContext;

    // Minimal Metagame Modules Contexts.
    private MetagameBaseContext   _metagameBaseContext;
    private ConfigurationContext  _metagameConfigurationContext;
    private AuthenticationContext _metagameAuthenticationContext;

    public Initializer()
    {
        _environmentContext = new EnvironmentContext();
            
        // (Optional) Sets your DeviceInfo into the environment context here.
        // _environmentContext.DeviceInfo = YourDeviceInfo

        _environmentContext.PitayaQueueDispatcher = MainQueueDispatcher.Create();
        _environmentContext.Logger = Logger.logger;
        _environmentContext.AnalyticsTfg = new NullTFGAnalytics();
        _environmentContext.Serializer = new JsonSerializer();

        // Retrieves the persistent data directory (Read Only) and create this directory if it does not exist yet.
        var root = Application.persistentDataPath;
        Directory.CreateDirectory(root);

        // Sets the paths to the persistent account data and the directory holding all configs (i.e. store config).
        _environmentContext.AccountDataFilePath = $"{root}/account.json";
        _environmentContext.ConfigurationDataDirectoryPath = $"{root}/configs";
        Directory.CreateDirectory(_environmentContext.ConfigurationDataDirectoryPath);

        // Create file persistence for account data based on the provided account file path.
        _environmentContext.AccountFilePersistency = new FilePersistency(_environmentContext.AccountDataFilePath, _environmentContext.Serializer);

        // Create file persistence for configuration data based on the provided configuration file path.
        _environmentContext.ConfigurationFilePersistency = new FilePersistency(_environmentContext.ConfigurationDataDirectoryPath, _environmentContext.Serializer);

        _environmentContext.ThreadDispatcher = ThreadDispatcher.defaultDispatcher;
    }

    public EnvironmentContext EnvironmentContext => _environmentContext;

    public MetagameBaseContext MetagameBaseContext => _metagameBaseContext;

    public ConfigurationContext ConfigurationContext => _metagameConfigurationContext;

    public AuthenticationContext AuthenticationContext => _metagameAuthenticationContext;

    public void InitializeMetagameBase()
    {
        _metagameBaseContext = new MetagameBaseContext();
            
        _metagameBaseContext.HookSerializer      = new HookProtobufSerializer();
        _metagameBaseContext.RequestFactory      = new MetagameRequestFactory(_environmentContext.ThreadDispatcher, _environmentContext.Logger);

        _metagameBaseContext.DeviceInfoRetriever = _environmentContext.DeviceInfo;

        var metagameConfig = new MetagameConfig
        {
            Host = Defines.EnvironmentData.MetagameUrl,
            Port = Defines.EnvironmentData.MetagamePort,
        };

        _metagameBaseContext.ClientBuilder = new MetagameClientBuilder(metagameConfig, new PitayaConfig(), _metagameBaseContext.DeviceInfoRetriever)
        {
            Dispatcher = _environmentContext.PitayaQueueDispatcher,
        };

        _metagameBaseContext.Client = (MetagameClient) _metagameBaseContext.ClientBuilder.Build();
            
        _metagameBaseContext.RequestHandler = new MetagameRequestHandler(_metagameBaseContext.Client, _metagameBaseContext.RequestFactory, _environmentContext.ThreadDispatcher, _environmentContext.Logger);
    }
        
    public void InitializeMetagameConfiguration()
    {
        _metagameConfigurationContext = new ConfigurationContext();

        _metagameConfigurationContext.HooksBuilder = new ConfigurationHooksBuilder(_metagameBaseContext.HookSerializer);
        
        var configInfos = new ConfigInfo[]
        {
            // Required configs must be added here (i.e. config for installed modules).
        };

        _metagameConfigurationContext.Client = new MetagameConfigurationClient(configInfos, _metagameBaseContext.Client, _environmentContext.ConfigurationFilePersistency, _metagameConfigurationContext.HooksBuilder);
    }
        
    public void InitializeMetagameAuthentication()
    {
        _metagameAuthenticationContext = new AuthenticationContext();
            
        _metagameAuthenticationContext.HooksBuilder    = new AuthenticationHooksBuilder(_metagameBaseContext.HookSerializer);
        _metagameAuthenticationContext.AccountProvider = new AccountProvider(_metagameAuthenticationContext.HooksBuilder);
        
        _metagameAuthenticationContext.Client      = new MetagameAuthenticationClient(_metagameBaseContext.Client, _environmentContext.AccountFilePersistency, _metagameAuthenticationContext.HooksBuilder);
        _metagameAuthenticationContext.LoginClient = new MetagameLoginClient(_metagameAuthenticationContext.Client, _metagameConfigurationContext.Client, _environmentContext.AccountFilePersistency);
            
        _metagameAuthenticationContext.ReadOnlyPlayer = _environmentContext.AccountFilePersistency.Read<ReadOnlyPlayer>();
    }
}