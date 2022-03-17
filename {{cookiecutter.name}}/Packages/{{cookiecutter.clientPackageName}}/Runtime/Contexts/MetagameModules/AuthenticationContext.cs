using Wildlife.Authentication;
using Wildlife.MetagameModuleAbstractions;

namespace Contexts.MetagameModules
{
    ///<summary>
    ///A set of authentication resources related to the authentication module which includes a hook builder, a player,
    /// an account provider, a metagame client and a login client.
    ///</summary>
    public struct AuthenticationContext
    {
        ///<summary>
        ///The hook builder for building authentication hooks related to the account creation and authentication.
        ///</summary>
        public IAuthenticationHooksBuilder HooksBuilder;
        ///<summary>
        ///The metagame authentication responsible for authenticating and creating a Player account.
        ///</summary>
        public IMetagameAuthenticationClient Client;
        ///<summary>
        ///The metagame login client responsible for logging in an existing Player account.
        ///</summary>
        public IMetagameLoginClient LoginClient;
        ///<summary>
        ///The read-only data of the player that is currently authenticated and possibly logged in.
        ///</summary>
        public IReadOnlyPlayer ReadOnlyPlayer;
    }
}