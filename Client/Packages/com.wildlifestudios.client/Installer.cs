using System;
using System.IO;
using System.Linq;
using Demo;
using Demo.Client;
using Demo.Common;
using Demo.Config;
using Demo.Core;
using Demo.Source;
using Demo.UI;
using Demo.Source.Common.ServiceCollection;
using Demo.Source.Core.Content;
using Demo.Source.Core.DailyLoginRewards;
using Demo.Source.Core.Friends;
using Demo.Source.Core.Wallet;
using Demo.Source.Core.LootBoxes;
using Demo.Source.Core.PlayerProfile;
using Demo.Source.Core.RewardsTrack;
using Demo.Source.UI;
using CustomCriteriaFieldFetcher = Demo.Source.Core.RemoteActions.CriteriaFieldFetcher;
using CustomActionExecutor = Demo.Source.Core.RemoteActions.ActionExecutor;
using Demo.Source.UI.MainMenu;
using Demo.Source.UI.Services;
using Demo.Source.UI.Services.DailyLoginRewards;
using Demo.Source.UI.Services.Friends;
using Demo.Source.UI.Services.Friends.Actions;
using Demo.Source.UI.Services.Friends.FriendshipTags;
using Demo.Source.UI.Services.Friends.PlayerFriends;
using Demo.Source.UI.Services.Friends.Requests;
using Demo.Source.UI.Services.LootBoxes;
using Demo.Source.UI.Services.PlayerProfile;
using Demo.Source.UI.Services.RewardsTrack;
using Demo.Source.UI.Services.RewardsTrack.Milestone;
using Demo.Source.UI.Services.RewardsTrack.Track;
using Demo.Source.UI.Services.RewardsTrack.TrackSet;
using Demo.Source.ViewControllers.Services;
using Demo.SRDebugger;
using Demo.UI.MainMenu;
using Demo.UI.MainMenu.ServiceGroup;
using Demo.UI.Services.Store;
using Demo.ViewControllers;
using Demo.Zenject.Installers;
using Packages.com.wildlifestudios.Runtime;
using Pitaya;
using Services.Authentication;
using Services.Inventory;
using Services.LiveEvents;
using Services.PlayerSupport;
using Services.Mailbox.Source;
using Services.Mediation.Source;
using TFG.Modules;
using TFG.Modules.Interface;
using Services.PTAAS.Source;
using Services.Store;
using Services.Topaz.Source;
using Services.UI.Services;
using Services.UI.Services.Mediation;
using Services.UI.Services.RemoteActions;
using Services.ViewControllers.Services;
using UnityEngine;
using Wildlife.Authentication;
using Wildlife.Configuration;
using Wildlife.Configuration.Interface;
using Wildlife.DailyLoginRewards.Cache;
using Wildlife.DailyLoginRewards.Config;
using Wildlife.DailyLoginRewards.Converters;
using Wildlife.DailyLoginRewards.Provider;
using Wildlife.Friends;
using Wildlife.Friends.Builders;
using Wildlife.Friends.Caches;
using Wildlife.Friends.Encoders;
using Wildlife.Friends.Providers;
using Wildlife.Friends.Retrievers;
using DailyLoginRewardsModuleBuilder = Wildlife.DailyLoginRewards.ModuleBuilder;
using Wildlife.Inventory;
using Wildlife.PlayerProfile;
using Wildlife.MetagameBase;
using Wildlife.MetagameBase.DeviceInfo;
using Wildlife.Persistency;
using Wildlife.Store.Fakes;
using Wildlife.Store.Handlers;
using Wildlife.Store.Providers;
using Wildlife.Wallet;
using Wildlife.LootBoxes;
using Wildlife.LootBoxes.Cache;
using Wildlife.PlayerProfile.Cache;
using Wildlife.PlayerProfile.Models;
using Wildlife.LootBoxes.Models;
using Wildlife.MetagameModuleAbstractions;
using Zenject;
using Config = Services.Configuration.Source.Dependencies.Config;
using Analytics = Demo.Analytics;
using DeviceInfoRetriever = Demo.DeviceInfoRetriever;
using IAnalytics = Wildlife.MetagameBase.DeviceInfo.IAnalytics;
using ILogger = Wildlife.Logging.ILogger;
using JsonSerializer = Wildlife.Persistency.Json.JsonSerializer;
using Logger = Wildlife.Logging.Logger;
using Wildlife.Metagame.Abstractions.Inventory;
using Wildlife.Inventory.Adapters;
using Wildlife.Metagame.Abstractions.Wallet;
using Wildlife.Wallet.Adapters;
using Wildlife.Metagame.Abstractions.Content;
using Wildlife.RewardsTrack.Builders;
using Wildlife.RewardsTrack.Clients;
using Wildlife.RewardsTrack.Config;
using Player = Wildlife.MetagameModuleAbstractions.Player;

namespace Client
{
    public class Installer
    {
        public BaseServicesInstall           baseServicesInstall;
        public MetagameBaseInstall           metagameBaseInstall;
        public MetagameConfigurationInstall  metagameConfigurationInstall;
        public MetagameAuthenticationInstall metagameAuthenticationInstall;

        public Installer()
        {
            baseServicesInstall           = new BaseServicesInstall();
            metagameBaseInstall           = new MetagameBaseInstall();
            metagameConfigurationInstall  = new MetagameConfigurationInstall();
            metagameAuthenticationInstall = new MetagameAuthenticationInstall();
        }
        
        public void InstallBaseServices()
        {
            baseServicesInstall = new BaseServicesInstall();
            
            baseServicesInstall.pitayaQueueDispatcher = MainQueueDispatcher.Create();
            baseServicesInstall.logger = Logger.logger;
            baseServicesInstall.analytics = new Demo.Analytics();
            baseServicesInstall.serializer = new Wildlife.Persistency.Json.JsonSerializer();

            var root = Application.persistentDataPath;

            Directory.CreateDirectory(root);

            baseServicesInstall.configuration = new Config()
            {
                AccountFilePath = $"{root}/account.json",
                ConfigurationDirectoryPath = $"{root}/configs",
            };

            var accountPersistencyPath = baseServicesInstall.configuration.AccountFilePath;
            baseServicesInstall.accountFilePersistency = new FilePersistency(accountPersistencyPath, serializer);

            var configurationPersistencyPath = baseServicesInstall.configuration.ConfigurationDirectoryPath;
            Directory.CreateDirectory(configurationPersistencyPath);
            baseServicesInstall.configurationFilePersistency = new FilePersistency(path, serializer);

            baseServicesInstall.threadDispatcher = ThreadDispatcher.defaultDispatcher;

            baseServicesInstall.playerReauthenticator = new PlayerReauthenticator(this);
        }

        public void InstallMetagameBase(DeviceInfo.DeviceInfo deviceInfo)
        {
            metagameBaseInstall = new MetagameBaseInstall();
            
            metagameBaseInstall.hookSerializer      = new HookProtobufSerializer();
            metagameBaseInstall.requestFactory      = new MetagameRequestFactory();
            metagameBaseInstall.requestHandler      = new MetagameRequestHandler();

            metagameBaseInstall.deviceInfoRetriever = deviceInfo;

            var metagameConfig = new MetagameConfig
            {
                Host = Defines.EnvironmentData.MetagameUrl,
                Port = Defines.EnvironmentData.MetagamePort,
            };

            metagameBaseInstall.clientBuilder = new MetagameClientBuilder(metagameConfig, new PitayaConfig(), metagameBaseInstall.deviceInfoRetriever)
            {
                Dispatcher = baseServicesInstaller.pitayaQueueDispatcher,
            };

            metagameBaseInstall.client = (MetagameClient) metagameBaseInstall.clientBuilder.Build();
        }
        
        public void InstallMetagameConfiguration()
        {
            metagameConfigurationInstall = new MetagameConfigurationInstall();

            metagameConfigurationInstall.hooksBuilder            = new ConfigurationHooksBuilder();
            metagameConfigurationInstall.configurationCollection = new ConfigurationCollection();
        
            var configInfos = new[]
            {
                // Required configs must be added here (i.e. config for installed modules).
            };

            metagameConfigurationInstall.client = new MetagameConfigurationClient(configInfos, metagameBaseInstall.client, baseServicesInstall.configurationFilePersistency, metagameConfigurationInstall.hooksBuilder);
        }
        
        public void InstallMetagameAuthentication()
        {
            metagameAuthenticationInstall = new MetagameAuthenticationInstall();
            
            metagameAuthenticationInstall.hooksBuilder    = new AuthenticationHooksBuilder();
            metagameAuthenticationInstall.accountProvider = new AccountProvider();
        
            metagameAuthenticationInstall.client      = new MetagameAuthenticationClient(metagameBaseInstall.client, baseServicesInstaller.accountFilePersistency, metagameAuthenticationInstall.hooksBuilder);
            metagameAuthenticationInstall.loginClient = new MetagameLoginClient(metagameAuthenticationInstall.client, metagameConfigurationInstall.client, baseServicesInstaller.accountFilePersistency);
            
            metagameAuthenticationInstall.readOnlyPlayer = baseServicesInstaller.accountFilePersistency.Read<Player>();
        }
    }
}