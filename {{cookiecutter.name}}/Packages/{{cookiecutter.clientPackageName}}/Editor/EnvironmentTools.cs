#if UNITY_EDITOR
using UnityEditor;

public class EnvironmentTools
{
    const string kLocalTool = "Tools/Environment/Local";
    const string kStagTool = "Tools/Environment/Stag";
    const string kProdTool = "Tools/Environment/Prod";

    [MenuItem(kLocalTool, true)]
    static bool UseLocalEnvValidate()
    {
        return CheckEnvs();
    }

    [MenuItem(kLocalTool)]
    static void UseLocalEnv()
    {
        SetEnv(EnvironmentData.Local);
    }

    [MenuItem(kStagTool, true)]
    static bool UseStagEnvValidate()
    {
        return CheckEnvs();
    }

    [MenuItem(kStagTool)]
    static void UseStagEnv()
    {
        SetEnv(EnvironmentData.Stag);
    }

    [MenuItem(kProdTool, true)]
    static bool UseProdEnvValidate()
    {
        return CheckEnvs();
    }

    [MenuItem(kProdTool)]
    static void UseProdEnv()
    {
        SetEnv(EnvironmentData.Prod);
    }
        
    static bool CheckEnvs()
    {
        Menu.SetChecked(kLocalTool, Defines.EnvironmentData.MetagameUrl == EnvironmentData.Local.MetagameUrl);
        Menu.SetChecked(kStagTool, Defines.EnvironmentData.MetagameUrl == EnvironmentData.Stag.MetagameUrl);
        Menu.SetChecked(kProdTool, Defines.EnvironmentData.MetagameUrl == EnvironmentData.Prod.MetagameUrl
                                   && EnvironmentData.Prod.MetagameUrl != "your.prod.url.here");
        return true;
    }
        
    static void SetEnv(EnvironmentData environment)
    {
        EnvironmentData.SaveEnv(environment);
    }
}
#endif
