using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KenffySoft.Bloggy.ViewModels
{
    public class ProfileOthersViewModel : BaseViewModel
    {
        private bool isDarkThemeActive;
        public Command PrivacyPolicyCommand { get; }
        public Command TermsOfUseCommand { get; }
        public Command WebsiteCommand { get; }
        public Command RateAppCommand { get; }
        public ProfileOthersViewModel()
        {
            isDarkThemeActive = false;
            PrivacyPolicyCommand = new Command(OnPrivacyPolicy);
            TermsOfUseCommand = new Command(OnTermsOfUse);
            WebsiteCommand = new Command(OnWebsite);
            RateAppCommand = new Command(OnRateApp);
        }

        public bool IsDarkThemeActive
        {
            get => isDarkThemeActive;
            set
            {
                isDarkThemeActive = value;
            }
        }

        private async void OnRateApp(object obj)
        {
            var msg = "Currently not available";
            await Application.Current.MainPage.DisplayAlert("Rate the Application", msg, "Cancel");
            return;
        }

        private async void OnWebsite(object obj)
        {
            await Browser.OpenAsync("https://www.bloggies.com");
        }

        private async void OnTermsOfUse(object obj)
        {
            var msg = "Currently not available!!!";
            await Application.Current.MainPage.DisplayAlert("Terms of use", msg, "Cancel");
            return;
        }

        private async void OnPrivacyPolicy(object obj)
        {
            var msg = "Currently not available!!!";
            await Application.Current.MainPage.DisplayAlert("Privacy Policy", msg, "Cancel");
            return;
        }
    }
}
