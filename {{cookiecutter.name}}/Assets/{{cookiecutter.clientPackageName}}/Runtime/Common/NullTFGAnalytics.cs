using System;
using System.Collections.Generic;
using Google.Protobuf;
using TFG.Modules.AnalyticsModule;
using Wildlife.MetagameBase.DeviceInfo;

namespace Common
{
    public class NullTFGAnalytics : TFG.Modules.IAnalytics
    {
        public bool Initialized { get; }
        
        public bool IsEnabled { get; }
        
        public string AdId { get; }
        
        public string ActivationDate { get; }
        
        public bool IsFirstSession { get; }
        
        public bool IsReinstall { get; }
        
        public int SessionCount { get; }
        
        public string FirstInstallId { get; }
        
        public string FirstInstallAppVersion { get; }
        
        public DateTime LastSession { get; }
        
        public void AddTrackingConsentListener(Action<bool> consentBlock)
        {
            // @todo Empty method to be implemented for analytics.
        }
        
        public void Initialize()
        {
            // @todo Empty method to be implemented for analytics.
        }

        public void SendEvent(string eventId, Dictionary<string, string> parameters = null, bool oneTime = false)
        {
            // @todo Empty method to be implemented for analytics.
        }
        
        public void SendEvent(IMessage message, bool oneTime = false)
        {
            // @todo Empty method to be implemented for analytics.
        }
        
        public void SetHeaderModifier(string key, object value)
        {
            // @todo Empty method to be implemented for analytics.
        }
        
        public void AddLogger(IAnalyticsLogger loggerInstance)
        {
            // @todo Empty method to be implemented for analytics.
        }
        
        public void RemoveLogger(IAnalyticsLogger loggerInstance)
        {
            // @todo Empty method to be implemented for analytics.
        }
        
        [Obsolete("Use SendEvent() instead")]
        public void LogEvent(string eventId, Dictionary<string, object> parameters = null, bool oneTime = false)
        {
            // @todo Empty method to be implemented for analytics.
        }
    }
}
