using Demo.Common;
using Demo.Source;
using Demo.SRDebugger;
using UnityEngine;
using Wildlife.UnityLogger;
using Logger = Wildlife.Logging.Logger;
using Pitaya;
using TFG.Modules;
using Zenject;

namespace Client
{
    public class PlayerReauthenticator : IPlayerReauthenticator
    {
        Installer _installer;
        
        public PlayerReauthenticator(Installer installer)
        {
            _installer = installer;
            
            var handler = new MetagameClientStatusEventHandler();
            _installer.metagameBaseInstall.metagameClient.StatusHandler = handler;

            handler.OnConnectedEvent += Connected;
            handler.OnFailedToConnectEvent += ConnectionFailed;
            handler.OnDisconnectedEvent += Disconnected;
            handler.OnReconnectedEvent += Reconnected;
            handler.OnKickedEvent += Kicked;

            Logger.LogInfo("Connect!");
            _installer.metagameBaseInstall.metagameClient.Connect();
        }
        
        public void ReauthenticatePlayer()
        {
            _installer.metagameBaseInstall.metagameClient.Reconnect();
        }
        
        void OnDestroy()
        {
            _installer.metagameBaseInstall.metagameClient.Disconnect();
        }
        
        void Connected(IMetagameClient client)
        {
            Logger.LogInfo("Metagame Client Connected!");

            _installer.metagameBaseInstall.loginClient.Login(player =>
            {
                Logger.LogInfo($"AccountID: {player.Id}");
            }, errors =>
            {
                foreach (var er in errors)
                {
                    Logger.LogError($"Initializer::Connected > Error [ {er.Code}: {er.Msg} ]");
                }

                client.Reconnect();
            });
        }

        void Reconnected(IMetagameClient client)
        {
            Logger.LogInfo("Reconnected!");
            Connected(client);
        }

        void ConnectionFailed(IMetagameClient client, NetworkError error)
        {
            Logger.LogError("Connection Failed!");
        }

        void Disconnected(IMetagameClient client, NetworkError error)
        {
            Logger.LogInfo("Disconnected!");
            client.Reconnect();
        }

        void Kicked(IMetagameClient client)
        {
            Logger.LogInfo("Kicked!");
        }
    }
}
