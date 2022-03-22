#if UNITY_EDITOR
using Core;
using Environment;
using UnityEditor;
using UnityEngine;
using Logger = Wildlife.Logging.Logger;

namespace Editor
{
    public class ToolbarEnvironmentSelector
    {
        const string envLocalToolPath = "Tools/Environment/Local";
        const string envStagToolPath = "Tools/Environment/Stag";
        const string envProdToolPath = "Tools/Environment/Prod";

        [MenuItem(envLocalToolPath, true)]
        static bool UseLocalEnvValidate()
        {
            return CheckEnvs();
        }

        [MenuItem(envLocalToolPath)]
        static void UseLocalEnv()
        {
            SetEnv(EnvironmentData.Local);
        }

        [MenuItem(envStagToolPath, true)]
        static bool UseStagEnvValidate()
        {
            return CheckEnvs();
        }

        [MenuItem(envStagToolPath)]
        static void UseStagEnv()
        {
            SetEnv(EnvironmentData.Stag);
        }

        [MenuItem(envProdToolPath, true)]
        static bool UseProdEnvValidate()
        {
            return CheckEnvs();
        }

        [MenuItem(envProdToolPath)]
        static void UseProdEnv()
        {
            SetEnv(EnvironmentData.Prod);
        }

        static bool CheckEnvs()
        {
            Menu.SetChecked(envLocalToolPath, Defines.EnvironmentData.MetagameUrl == EnvironmentData.Local.MetagameUrl);
            Menu.SetChecked(envStagToolPath, Defines.EnvironmentData.MetagameUrl == EnvironmentData.Stag.MetagameUrl);
            Menu.SetChecked(envProdToolPath, Defines.EnvironmentData.MetagameUrl == EnvironmentData.Prod.MetagameUrl
                                             && EnvironmentData.Prod.MetagameUrl != "your.prod.url.here");
            return true;
        }

        static void SetEnv(EnvironmentData environment)
        {
            EnvironmentData.SaveEnv(environment);
        }
    }
}
#endif
