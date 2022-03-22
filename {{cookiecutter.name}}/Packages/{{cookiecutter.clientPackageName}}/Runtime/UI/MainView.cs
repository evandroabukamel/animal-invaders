using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Wildlife.MetagameBase;

namespace UI
{
    public class MainView : MonoBehaviour
    {
        private IMetagameClient _metagameClient;

        [SerializeField] private Text projectNameTitle;

        private void Awake()
        {
            projectNameTitle.text = "Minimal Client";
        }

        public void SetMetagameClient(IMetagameClient metagameClient)
        {
            _metagameClient = metagameClient;
        }

        public void Reauthenticate()
        {
            if (_metagameClient?.IsConnected() == true)
            {
                _metagameClient?.Reconnect();
                return;
            }

            _metagameClient?.Connect();
        }
    }
}