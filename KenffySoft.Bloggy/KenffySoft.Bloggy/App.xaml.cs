using KenffySoft.Bloggy.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KenffySoft.Bloggy
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(Preferences.Get("BloggyToken", string.Empty)))
            {
                MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                MainPage = new AppShell();
            }
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
    }
}
