using Core.Environment;
using Pitaya;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// This class defines common configurations to be used based on the environment that your App is running.
    /// </summary>
    public static class Defines
    {
        public static PitayaLogLevel PitayaLogLevel =
            (PitayaLogLevel)PlayerPrefs.GetInt(PlayerPrefsKeys.PitayaLogLevelKey, (int)PitayaLogLevel.Disable);

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
