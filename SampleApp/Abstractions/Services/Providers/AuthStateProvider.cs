using Blazored.LocalStorage;
using CountryApp.Abstractions.Constants;
using CountryApp.Abstractions.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CountryApp.Abstractions.Services.Providers
{
    public class AuthStateProvider : AuthenticationStateProvider, IAccessTokenProvider
    {
        private readonly AppSettings _appSettings;
        private readonly ILocalStorageService _localStorage;

        public AuthStateProvider(AppSettings appSettings,
            ILocalStorageService localStorage)
        {
            _appSettings = appSettings;
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var authData = await _localStorage.GetItemAsync<Profile>(ConfigType.IdentityUserStore);
            if (authData == null || !authData.IsLoggedIn)
            {
                ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());
                return new AuthenticationState(currentUser);
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(_appSettings.JwtToken) as JwtSecurityToken;

            List<Claim> userClaims = new();
            if (jsonToken != null)
            {
                foreach (Claim claim in jsonToken.Claims)
                {
                    userClaims.Add(claim);
                }
            }
            else
            {
                Console.WriteLine("Invalid token");
            }

            ClaimsIdentity identity = new ClaimsIdentity(userClaims, "apiauth_type");
            var principal = new ClaimsPrincipal(identity);
            return await Task.FromResult(new AuthenticationState(principal));
        }

        public Task LogOutUser()
        {
            ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(currentUser)));
            return Task.CompletedTask;
        }

        public ValueTask<AccessTokenResult> RequestAccessToken()
        {
            AccessToken accessToken = new()
            {
                Value = _appSettings.JwtToken,
                Expires = DateTime.Now.AddDays(1)
            };

            InteractiveRequestOptions interaction = new() { Interaction = InteractionType.GetToken, ReturnUrl = "/", Scopes = ["api", "openid", "profile", "offline_access"] };
            AccessTokenResult accessTokenResult = new(AccessTokenResultStatus.Success, accessToken, string.Empty, interaction);
            return ValueTask.FromResult(accessTokenResult);
        }

        public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            AccessToken accessToken = new()
            {
                Value = _appSettings.JwtToken,
                Expires = DateTime.Now.AddDays(1)
            };

#pragma warning disable CS8601 // Possible null reference assignment.
            InteractiveRequestOptions interaction = new() { Interaction = InteractionType.SignIn, ReturnUrl = options.ReturnUrl, Scopes = options.Scopes };
#pragma warning restore CS8601 // Possible null reference assignment.
            AccessTokenResult accessTokenResult = new(AccessTokenResultStatus.Success, accessToken, string.Empty, interaction);
            return ValueTask.FromResult(accessTokenResult);
        }

    }
}
