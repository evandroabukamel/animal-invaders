using System;
using UnityEngine;

namespace Core
{
    public static class PlayerPrefsKeys
    {
        /// <summary>
        /// Key to store the Pitaya Log Level into PlayerPrefs.
        /// </summary>
        public const string PitayaLogLevelKey = "pitaya_log_level";
        /// <summary>
        /// Key to store the metagame connection port into PlayerPrefs.
        /// </summary>
        public const string MetagamePort = "metagame_port";
        /// <summary>
        /// Key to store the metagame connection url into PlayerPrefs.
        /// </summary>
        public const string MetagameURL = "metagame_url";
    }
}
