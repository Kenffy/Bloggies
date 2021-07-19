using Firebase.Auth;
using KenffySoft.Bloggy.Views;
using Newtonsoft.Json;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KenffySoft.Bloggy
{
    public partial class App : Application
    {
        public static string AuthUserId = "";
        public App()
        {
            InitializeComponent();
            var tokenStr = Preferences.Get("BloggyToken", string.Empty);
            
            if (string.IsNullOrEmpty(tokenStr))
            {
                MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                var token = JsonConvert.DeserializeObject<FirebaseAuthLink>(tokenStr);
                AuthUserId = token.User.LocalId;
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
