using KenffySoft.Bloggy.Helpers;
using KenffySoft.Bloggy.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KenffySoft.Bloggy.ViewModels
{
    public class ProfileViewModel
    {
        public Command LogoutCommand { get; }
        public Command AboutCommand { get; }
        public ProfileViewModel()
        {
            LogoutCommand = new Command(OnLogOut);
            AboutCommand = new Command(OnAbout);
        }

        private async void OnAbout(object obj)
        {
            await Shell.Current.Navigation.PushAsync(new AboutPage());
            //await Shell.Current.GoToAsync(nameof(AboutPage));
        }

        private async void OnLogOut()
        {
            #region PoketBlog FireBase
            if (BloggyConstant.CheckConnectivity() == false)
            {
                await Application.Current.MainPage.DisplayAlert("Logout Error", "No internet connection. Please check and try again", "Cancel");
                return;
            }

            Preferences.Set("BloggyToken", string.Empty);
            Preferences.Set("Bloggy_Email", string.Empty);
            Preferences.Set("Bloggy_Password", string.Empty);
            Application.Current.MainPage = new NavigationPage(new LoginPage());

            #endregion
        }
    }
}
