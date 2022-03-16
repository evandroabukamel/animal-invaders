using Wildlife.Authentication;
using Wildlife.MetagameModuleAbstractions;
using Services.MetagameModules.Authentication;

namespace Contexts.MetagameModules
{
    public struct AuthenticationContext
    {
        public IAuthenticationHooksBuilder HooksBuilder;
        public IMetagameAuthenticationClient Client;
        public IMetagameLoginClient LoginClient;
        public IAccountProvider AccountProvider;
        public IReadOnlyPlayer ReadOnlyPlayer;
    }
}