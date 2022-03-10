using Pitaya;
using UnityEngine;

namespace Demo.Common
{
    /// <summary>
    /// This class defines common configurations to be used based on the environment that your App is running.
    /// </summary>
    public static class Defines
    {
        /// <summary>
        /// Key to store the Pitaya Log Level into PlayerPrefs.
        /// </summary>
        public const string PitayaLogLevelKey = "pitaya_log_level";

        public const string SamplePlayerSupportHubUrl = "https://hub-stag.wildlifestudios.com/games/5ce493bc-6ddb-4ac7-bcf9-5d522df0df92/services/angel/findAndUtils";
        
        public static PitayaLogLevel pitayaLogLevel = (PitayaLogLevel)PlayerPrefs.GetInt(PitayaLogLevelKey, (int)PitayaLogLevel.Disable);

#if UNITY_EDITOR
        static EnvironmentData _environmentData => EnvironmentData.Local;
#else
        static EnvironmentData _environmentData => EnvironmentData.Stag;
#endif
        
        public static EnvironmentData EnvironmentData => EnvironmentData.ReadEnv(_environmentData);
    }
}
