using System.Collections.Generic;
using UnityEngine.Scripting;
using Wildlife.Authentication;
using Wildlife.MetagameBase.Hooks;
using Protos.V1;
using System.Linq;
using Player = Wildlife.MetagameModuleAbstractions.Player;

namespace Services.MetagameModules.Authentication
{
    public interface IAccountProvider
    {
        Player GetAccount();
        IEnumerable<string> GetCustomPayload();
    }

    public class AccountProvider : IAccountProvider
    {
        public const string ExampleNameArg = "Player123";

        private Player _player;
        private List<string> _customPayload;

        public AccountProvider(IAuthenticationHooksBuilder authHooksBuilder)
        {
            authHooksBuilder.AddCreateArgHook(new NoArgsHook(this, nameof(CreateArgsHook)));
            authHooksBuilder.AddAuthenticateArgHook(new SingleArgHook<Player>(this, nameof(AuthenticateArgsHook)));

            authHooksBuilder.AddAuthenticateHook(new Hook<Player>(this, nameof(OnAuthenticateResponse)));
            authHooksBuilder.AddCreateHook(new Hook<Player>(this, nameof(OnCreateResponse)));
        }

        public Player GetAccount()
        {
            return _player;
        }

        public IEnumerable<string> GetCustomPayload()
        {
            return _customPayload;
        }

        [Preserve]
        public CreateAccountAdditionalArgs CreateArgsHook()
        {
            return new CreateAccountAdditionalArgs()
            {
                Name = ExampleNameArg,
                TutorialStep = 12,
                ChosenServerRegion = "us",
            };
        }

        [Preserve]
        public AuthenticateAdditionalArgs AuthenticateArgsHook(Player player)
        {
            return new AuthenticateAdditionalArgs()
            {
                ChosenServerRegion = "eu",
            };
        }

        [Preserve]
        public void OnAuthenticateResponse(Player player, AuthenticateAdditionalPayload additionalPayload)
        {
            _player = player;
            _customPayload = additionalPayload.CustomItemIds.ToList();
        }

        [Preserve]
        public void OnCreateResponse(Player player, CreateAccountAdditionalPayload additionalPayload)
        {
            _player = player;
            _customPayload = additionalPayload.CustomItemIds.ToList();
        }
    }
}
