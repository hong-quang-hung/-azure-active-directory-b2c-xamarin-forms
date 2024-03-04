using Microsoft.Identity.Client;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Azure
{
    public class MainPage : ContentPage
    {
        readonly Button LoginButton;
        readonly Grid Grid;

        public MainPage()
        {
            LoginButton = new Button()
            {
                Text = "Logn In",
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 40,
                CornerRadius = 5,
            };
            LoginButton.Clicked += LoginButton_Clicked;

            Grid = new Grid()
            {
                Padding = 10,
                ColumnSpacing = 10,
                RowSpacing = 10,
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.StartAndExpand,
            };

            Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            Grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Grid.Children.Add(LoginButton, 0, 0);
            Grid.WidthRequest = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
            Grid.HeightRequest = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;

            Title = "Azure Authentication";
            Content = Grid;

            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

        async void LoginButton_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                var userContext = await B2CAuthenticationService.Instance.SignInAsync();
                UpdateSignInState(userContext);
                UpdateUserInfo(userContext);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("AADB2C90118"))
                {
                    OnPasswordReset();
                }
                else if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                {
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
                }
            }
        }

        async void OnPasswordReset()
        {
            try
            {
                var userContext = await B2CAuthenticationService.Instance.ResetPasswordAsync();
                UpdateSignInState(userContext);
                UpdateUserInfo(userContext);
            }
            catch (Exception ex)
            {
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                {
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
                }
            }
        }

        void UpdateSignInState(UserContext userContext)
        {
            _ = userContext;
        }

        void UpdateUserInfo(UserContext userContext)
        {
            _ = userContext;
        }
    }
}