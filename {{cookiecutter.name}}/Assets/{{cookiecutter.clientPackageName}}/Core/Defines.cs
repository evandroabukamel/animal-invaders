using Environment;
using Pitaya;
using UnityEngine;

namespace Core
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

        public static PitayaLogLevel PitayaLogLevel =
            (PitayaLogLevel)PlayerPrefs.GetInt(PitayaLogLevelKey, (int)PitayaLogLevel.Disable);

        // Enables the local or stag environment depending on how this app is running. 
#if UNITY_EDITOR
        static EnvironmentData AssignedEnvironmentData => EnvironmentData.Local;
#else
        static EnvironmentData AssignedEnvironmentData => EnvironmentData.Stag;
#endif
        // Notice: Prod is not enabled here by default, please change this file to consider the prod option.
        // Remember to check out the cookiecutter.json file for setting prod URL and port values.

        public static EnvironmentData EnvironmentData => EnvironmentData.ReadEnv(AssignedEnvironmentData);
    }
}
