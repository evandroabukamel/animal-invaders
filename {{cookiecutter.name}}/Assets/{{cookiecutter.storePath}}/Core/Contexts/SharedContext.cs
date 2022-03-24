using Pitaya;
using TFG.Modules.Interface;
using Wildlife.MetagameBase.DeviceInfo;
using Wildlife.Persistency;
using ILogger = Wildlife.Logging.ILogger;

namespace Core.Contexts
{
    ///<summary>
    ///A set of shared resources that might be used throughout the whole project, by all modules and other code contexts.
    ///</summary>
    public struct SharedContext
    {
        ///<summary>
        ///The dispatcher for Pitaya. 
        ///</summary>
        public IPitayaQueueDispatcher PitayaQueueDispatcher;
        ///<summary>
        ///The logger that must be used for logging info, errors, warnings and debug messages. 
        ///</summary>
        public ILogger Logger;
        ///<summary>
        ///The thread dispatcher for the game. 
        ///</summary>
        public IThreadDispatcher ThreadDispatcher;
        ///<summary>
        ///The analytics for tracking game metrics. 
        ///</summary>
        public TFG.Modules.IAnalytics AnalyticsTfg;
        ///<summary>
        ///The path to the persistence account data file. 
        ///</summary>
        public string AccountDataFilePath;
        ///<summary>
        ///The path to the directory storing persistence configuration data. 
        ///</summary>
        public string ConfigurationDataDirectoryPath;
        ///<summary>
        ///The serializer for serializing and deserializing objects (persistence-related). 
        ///</summary>
        public ISerializer Serializer;
        ///<summary>
        ///The file persistence that stores data related to the account file. 
        ///</summary>
        public IFilePersistency AccountFilePersistency;
        ///<summary>
        ///The file persistence that stores data related to the configuration file. 
        ///</summary>
        public IFilePersistency ConfigurationFilePersistency;
        ///<summary>
        ///The device information that includes data about the device running the game. 
        ///</summary>
        public DeviceInfo DeviceInfo;
    }
}