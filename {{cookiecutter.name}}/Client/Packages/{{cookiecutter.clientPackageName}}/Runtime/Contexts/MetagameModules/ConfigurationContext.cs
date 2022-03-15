using Wildlife.Configuration;
using Wildlife.Configuration.Interface;

namespace Contexts.MetagameModules
{
    public struct ConfigurationContext
    {
        public IConfigurationHooksBuilder HooksBuilder;
        public IMetagameConfigurationClient Client;
    }
}