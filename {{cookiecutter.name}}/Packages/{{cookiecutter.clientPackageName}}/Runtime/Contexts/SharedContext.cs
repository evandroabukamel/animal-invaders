using System;
using Pitaya;
using TFG.Modules.Interface;
using Wildlife.MetagameBase.DeviceInfo;
using Wildlife.Persistency;
using IAnalytics = Wildlife.MetagameBase.DeviceInfo.IAnalytics;
using ILogger = Wildlife.Logging.ILogger;

namespace Contexts
{
    public struct SharedContext
    {
        public IPitayaQueueDispatcher PitayaQueueDispatcher;
        public ILogger Logger;
        public IThreadDispatcher ThreadDispatcher;
        public TFG.Modules.IAnalytics AnalyticsTfg;
        public string AccountDataFilePath;
        public string ConfigurationDataDirectoryPath;
        public ISerializer Serializer;
        public IFilePersistency AccountFilePersistency;
        public IFilePersistency ConfigurationFilePersistency;
        public DeviceInfo DeviceInfo;
    }
}