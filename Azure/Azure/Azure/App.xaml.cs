using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Azure
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Register();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        void Register()
        {
            DependencyService.Register<B2CAuthenticationService>();
        }
    }
}
