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
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 45,
                BackgroundColor = Color.CadetBlue,
                BorderColor = Color.CadetBlue,
                BorderWidth = 2,
                CornerRadius = 10,
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

        void LoginButton_Clicked(object sender, EventArgs e)
        {
            B2CAuthenticationService.Instance.OnTokenFailed += Instance_OnTokenFailed;
            B2CAuthenticationService.Instance.OnAuthenticationFailed += Instance_OnAuthenticationFailed;
            B2CAuthenticationService.Instance.OnApiCallGraphFailed += Instance_OnApiCallGraphFailed;
            B2CAuthenticationService.Instance.OnSignInSuccessed += Instance_OnSignInSuccessed;
            B2CAuthenticationService.Instance.OnSignOutSuccessed += Instance_OnSignOutSuccessed;

            B2CAuthenticationService.Instance.CreatePublicClient(true);
            B2CAuthenticationService.Instance.AcquireTokenAsync().ConfigureAwait(false);
        }

        private void Instance_OnSignInSuccessed(object sender, string e)
        {
            LoginButton.Text = "Logn Out";
        }

        private void Instance_OnSignOutSuccessed(object sender, EventArgs e)
        {
            LoginButton.Text = "Logn In";
        }

        private async void Instance_OnApiCallGraphFailed(object sender, string e)
        {
            await DisplayAlert("API call to graph failed: ", e, "Dismiss");
        }

        private async void Instance_OnAuthenticationFailed(object sender, string e)
        {
            await DisplayAlert("Authentication failed. See exception message for details: ", e, "Dismiss");
        }

        private async void Instance_OnTokenFailed(object sender, string e)
        {
            await DisplayAlert("Acquire token interactive failed. See exception message for details: ", e, "Dismiss");
        }
    }
}