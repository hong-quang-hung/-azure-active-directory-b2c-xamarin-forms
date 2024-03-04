using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Azure
{
    public class B2CAuthenticationService
    {
        private readonly IPublicClientApplication PCA;

        private static readonly Lazy<B2CAuthenticationService> lazy = new Lazy<B2CAuthenticationService>(() => new B2CAuthenticationService());

        public static B2CAuthenticationService Instance { get { return lazy.Value; } }

        private B2CAuthenticationService()
        {

            var builder = PublicClientApplicationBuilder.Create(B2CSettings.ClientID)
                .WithB2CAuthority(B2CSettings.AuthoritySignInSignUp)
                .WithIosKeychainSecurityGroup(B2CSettings.IOSKeyChainGroup)
                .WithRedirectUri($"msal{B2CSettings.ClientID}://auth");

            var windowLocatorService = DependencyService.Get<IParentWindowLocatorService>();

            if (windowLocatorService != null)
            {
                builder = builder.WithParentActivityOrWindow(() => windowLocatorService?.GetCurrentParentWindow());
            }

            PCA = builder.Build();
        }

        public async Task<UserContext> SignInAsync()
        {
            UserContext newContext;
            try
            {
                newContext = await AcquireTokenSilent();
            }
            catch (MsalUiRequiredException)
            {
                newContext = await SignInInteractively();
            }
            return newContext;
        }

        private async Task<UserContext> AcquireTokenSilent()
        {
            IEnumerable<IAccount> accounts = await PCA.GetAccountsAsync();
            AuthenticationResult authResult = await PCA.AcquireTokenSilent(B2CSettings.Scopes, GetAccountByPolicy(accounts, B2CSettings.PolicySignUpSignIn))
               .WithB2CAuthority(B2CSettings.AuthoritySignInSignUp)
               .ExecuteAsync();

            var newContext = UpdateUserInfo(authResult);
            return newContext;
        }

        public async Task<UserContext> ResetPasswordAsync()
        {
            AuthenticationResult authResult = await PCA.AcquireTokenInteractive(B2CSettings.Scopes)
                .WithPrompt(Prompt.NoPrompt)
                .WithAuthority(B2CSettings.AuthorityPasswordReset)
                .ExecuteAsync();

            var userContext = UpdateUserInfo(authResult);

            return userContext;
        }

        public async Task<UserContext> EditProfileAsync()
        {
            IEnumerable<IAccount> accounts = await PCA.GetAccountsAsync();

            AuthenticationResult authResult = await PCA.AcquireTokenInteractive(B2CSettings.Scopes)
                .WithAccount(GetAccountByPolicy(accounts, B2CSettings.PolicyEditProfile))
                .WithPrompt(Prompt.NoPrompt)
                .WithAuthority(B2CSettings.AuthorityEditProfile)
                .ExecuteAsync();

            var userContext = UpdateUserInfo(authResult);

            return userContext;
        }

        private async Task<UserContext> SignInInteractively()
        {
            AuthenticationResult authResult = await PCA.AcquireTokenInteractive(B2CSettings.Scopes)
                .ExecuteAsync();

            var newContext = UpdateUserInfo(authResult);
            return newContext;
        }

        public async Task<UserContext> SignOutAsync()
        {

            IEnumerable<IAccount> accounts = await PCA.GetAccountsAsync();
            while (accounts.Any())
            {
                await PCA.RemoveAsync(accounts.FirstOrDefault());
                accounts = await PCA.GetAccountsAsync();
            }

            var signedOutContext = new UserContext();
            signedOutContext.IsLoggedOn = false;
            return signedOutContext;
        }

        private IAccount GetAccountByPolicy(IEnumerable<IAccount> accounts, string policy)
        {
            foreach (var account in accounts)
            {
                string userIdentifier = account.HomeAccountId.ObjectId.Split('.')[0];
                if (userIdentifier.EndsWith(policy.ToLower())) return account;
            }

            return null;
        }

        private string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }

        public UserContext UpdateUserInfo(AuthenticationResult ar)
        {
            var newContext = new UserContext();
            newContext.IsLoggedOn = false;
            JObject user = ParseIdToken(ar.IdToken);

            newContext.AccessToken = ar.AccessToken;
            newContext.Name = user["name"]?.ToString();
            newContext.UserIdentifier = user["oid"]?.ToString();

            newContext.GivenName = user["given_name"]?.ToString();
            newContext.FamilyName = user["family_name"]?.ToString();

            newContext.StreetAddress = user["streetAddress"]?.ToString();
            newContext.City = user["city"]?.ToString();
            newContext.Province = user["state"]?.ToString();
            newContext.PostalCode = user["postalCode"]?.ToString();
            newContext.Country = user["country"]?.ToString();

            newContext.JobTitle = user["jobTitle"]?.ToString();

            if (user["emails"] is JArray emails)
            {
                newContext.EmailAddress = emails[0].ToString();
            }
            newContext.IsLoggedOn = true;

            return newContext;
        }

        JObject ParseIdToken(string idToken)
        {
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            return JObject.Parse(idToken);
        }
    }
}
