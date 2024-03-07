using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Azure
{
    public class B2CAuthenticationService
    {
        public object ParentWindow { get; set; }

        public IPublicClientApplication PCA;

        public B2CState State { get; set; }

        public static B2CAuthenticationService Instance;

        public event EventHandler<string> OnTokenFailed;
        public event EventHandler<string> OnAuthenticationFailed;
        public event EventHandler<string> OnApiCallGraphFailed;
        public event EventHandler<string> OnSignInSuccessed;
        public event EventHandler OnSignOutSuccessed;

        static B2CAuthenticationService()
        {
            Instance = new B2CAuthenticationService
            {
                State = B2CState.SignIn
            };
        }

        public void CreatePublicClient(bool useBroker)
        {
            if (PCA == null)
            {
                var builder = PublicClientApplicationBuilder.Create(B2CSettings.ClientID);

                if (useBroker)
                {
                    switch (Device.RuntimePlatform)
                    {
                        case Device.Android:
                            builder = builder.WithRedirectUri(B2CSettings.BrokerRedirectUriOnAndroid);
                            break;
                        case Device.iOS:
                            builder = builder.WithIosKeychainSecurityGroup("com.microsoft.adalcache");
                            builder = builder.WithRedirectUri(B2CSettings.BrokerRedirectUriOnIOS);
                            break;
                    }

                    builder.WithBroker();
                }
                else
                {
                    builder = builder.WithRedirectUri($"msal{B2CSettings.ClientID}://auth");
                }

                PCA = builder.Build();
            }
        }

        public async Task AcquireTokenAsync()
        {
            AuthenticationResult authResult = null;
            IEnumerable<IAccount> accounts = await PCA.GetAccountsAsync();
            try
            {
                if (State == B2CState.SignIn)
                {
                    try
                    {
                        IAccount firstAccount = accounts.FirstOrDefault();
                        authResult = await PCA.AcquireTokenSilent(B2CSettings.Scopes, firstAccount)
                                              .ExecuteAsync();
                    }
                    catch (MsalUiRequiredException)
                    {
                        try
                        {
                            authResult = await PCA.AcquireTokenInteractive(B2CSettings.Scopes)
                                                 .WithAuthority($"https://login.microsoftonline.com/{B2CSettings.TenantID}/v2.0")
                                                 .WithParentActivityOrWindow(ParentWindow)
                                                 .WithUseEmbeddedWebView(true)
                                                 .ExecuteAsync();
                        }
                        catch (Exception ex)
                        {
                            OnTokenFailed?.Invoke(ParentWindow, ex.Message);
                            OnTokenFailed = null;
                        }
                    }

                    if (authResult != null)
                    {
                        var content = await GetHttpContentWithTokenAsync(authResult.AccessToken);
                        UpdateUserContent(content);
                        State = B2CState.SignOut;
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            OnSignInSuccessed?.Invoke(ParentWindow, content);
                            OnSignInSuccessed = null;
                        });
                    }
                }
                else
                {
                    while (accounts.Any())
                    {
                        await PCA.RemoveAsync(accounts.FirstOrDefault());
                        accounts = await PCA.GetAccountsAsync();
                    }

                    State = B2CState.SignIn;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        OnSignOutSuccessed?.Invoke(ParentWindow, EventArgs.Empty);
                    });
                }
            }
            catch (Exception ex)
            {
                OnAuthenticationFailed?.Invoke(ParentWindow, ex.Message);
                OnAuthenticationFailed = null;
            }
        }

        private void UpdateUserContent(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                _ = content;
            }
        }

        private async Task<string> GetHttpContentWithTokenAsync(string token)
        {
            try
            {
                //get data from API
                HttpClient client = new HttpClient();
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.SendAsync(message);
                string responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception ex)
            {
                OnApiCallGraphFailed?.Invoke(ParentWindow, ex.Message);
                OnApiCallGraphFailed = null;
                return ex.ToString();
            }
        }
    }

    public enum B2CState
    {
        SignIn = 0,
        SignOut = 1
    }
}
