using UnityEngine;
using UnityEngine.UI;
using Wildlife.MetagameBase;

namespace Core.UI
{
    public class MainView : MonoBehaviour
    {
        private IMetagameClient _metagameClient;
          
        [SerializeField] private Text projectNameTitle;
        [SerializeField] private Text statusMessage;
        
        private void Awake()
        {
            projectNameTitle.text = "Minimal Client";
        }

        private void Update()
        {
            UpdateStatusMessage();
        }

        public void SetMetagameClient(IMetagameClient metagameClient)
        {
            _metagameClient = metagameClient;
        }

        private void UpdateStatusMessage()
        {
            if (_metagameClient.IsConnected())
            {
                statusMessage.text = "Connected!";
            }
            else
            {
                statusMessage.text = "Disconnected!";
            }
        }

        public void Disconnect()
        { 
            _metagameClient?.Disconnect();
        }
        
        public void Authenticate()
        {
            _metagameClient?.Connect();
        }
    }
}