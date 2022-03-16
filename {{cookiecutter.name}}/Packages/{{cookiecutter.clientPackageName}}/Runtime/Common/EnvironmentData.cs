using System;
using UnityEngine;

namespace Common
{
    public static class PlayerPrefsKeys
    {
        /// <summary>
        /// Key to store the metagame connection port into PlayerPrefs.
        /// </summary>
        public const string MetagamePort = "metagame_port";
        /// <summary>
        /// Key to store the metagame connection url into PlayerPrefs.
        /// </summary>
        public const string MetagameURL = "metagame_url";
    }
    
    public struct EnvironmentData
    {
        public string MetagameUrl;
        public int MetagamePort;

        public static readonly EnvironmentData Local = new EnvironmentData
        {
            MetagameUrl = "{{cookiecutter.localUrl}}",
            MetagamePort = Int32.Parse("{{cookiecutter.localPort}}")
        };
        
        public static readonly EnvironmentData Stag = new EnvironmentData
        {
            MetagameUrl = "{{cookiecutter.stagUrl}}",
            MetagamePort = Int32.Parse("{{cookiecutter.stagPort}}")
        };
        
        public static readonly EnvironmentData Prod = new EnvironmentData
        {
            MetagameUrl = "{{cookiecutter.prodUrl}}",
            MetagamePort = Int32.Parse("{{cookiecutter.prodPort}}")
        };

        public static EnvironmentData ReadEnv(EnvironmentData fallback = default)
        {
            return new EnvironmentData
            {
                MetagameUrl = PlayerPrefs.GetString(PlayerPrefsKeys.MetagameURL, fallback.MetagameUrl),
                MetagamePort = PlayerPrefs.GetInt(PlayerPrefsKeys.MetagamePort, fallback.MetagamePort),
            };
        }

        public static void SaveEnv(EnvironmentData data)
        {
            PlayerPrefs.SetString(PlayerPrefsKeys.MetagameURL, data.MetagameUrl);
            PlayerPrefs.SetInt(PlayerPrefsKeys.MetagamePort, data.MetagamePort);
        }
    }
}
