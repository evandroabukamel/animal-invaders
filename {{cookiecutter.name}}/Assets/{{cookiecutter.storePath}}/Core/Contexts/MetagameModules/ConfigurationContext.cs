using Wildlife.Configuration;
using Wildlife.Configuration.Interface;

namespace Core.Contexts.MetagameModules
{
    ///<summary>
    ///A set of configuration resources related to the configuration module which includes a hook builder and a metagame client.
    ///</summary>
    public struct ConfigurationContext
    {
        ///<summary>
        ///The hooks builder for creating a configuration hook.
        ///</summary>
        public IConfigurationHooksBuilder HooksBuilder;
        ///<summary>
        ///The metagame configuration client for retrieving existing configs associated with a given account identifier.
        ///</summary>
        public IMetagameConfigurationClient Client;
    }
}