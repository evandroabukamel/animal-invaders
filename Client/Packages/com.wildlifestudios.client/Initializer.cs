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
    /// <summary>
    /// Application entry point responsible for initializing dependencies and start the game.
    /// </summary>
    public class Initializer : MonoBehaviour
    {
        /// <summary>
        /// Awake used as an Entry point. This is where your app will start.
        /// </summary>
        void Awake()
        {
            UnityMainThreadDispatcher.Instance().ShouldThrow = true;
            
            // Set the UnityLogger into the Wildlife Logger, used on all modules.
            Logger.logger = new UnityLogger();
            
            // Set the LogLevel for Pitaya interactions.
            StaticPitayaBinding.SetLogLevel(Defines.pitayaLogLevel);
            
            _installer = new Installer();
            _installer.InstallBaseServices();
            _installer.InstallMetagameBase();
            _installer.InstallMetagameConfiguration();
            _installer.InstallMetagameAuthentication();
        }
    }
}
