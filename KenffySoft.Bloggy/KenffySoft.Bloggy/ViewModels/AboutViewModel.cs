using KenffySoft.Bloggy.Helpers;
using KenffySoft.Bloggy.Models;
using KenffySoft.Bloggy.Services;
using KenffySoft.Bloggy.Views;
using KenffySoft.Bloggy.Views.Users;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KenffySoft.Bloggy.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        private string version = "Version 1.0.1";
        private bool isDarkThemeActive;
        private Member currentUser;
        public Command PrivacyPolicyCommand { get; }
        public Command TermsOfUseCommand { get; }
        public Command WebsiteCommand { get; }
        public Command RateAppCommand { get; }


        public AboutViewModel()
        {
            Title = "About";
            isDarkThemeActive = false;
            currentUser = new Member();
            PrivacyPolicyCommand = new Command(OnPrivacyPolicy);
            TermsOfUseCommand = new Command(OnTermsOfUse);
            WebsiteCommand = new Command(OnWebsite);
            RateAppCommand = new Command(OnRateApp);

            LoadUserInfos();
        }

        private async void OnRateApp(object obj)
        {
            var msg = "Currently not available";
            await Application.Current.MainPage.DisplayAlert("Rate the Application", msg, "Cancel");
            return;
        }

        private async void OnWebsite(object obj)
        {
            await Browser.OpenAsync("https://aka.ms/xamarin-quickstart");
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

        public Member CurrentUser
        {
            get => currentUser;
            set => SetProperty(ref currentUser, value);
        }


        public string Version
        {
            get => version;
            set => SetProperty(ref version, value);
        }

        public bool IsDarkThemeActive
        {
            get => isDarkThemeActive;
            set => SetProperty(ref isDarkThemeActive, value);
        }

        private async void LoadUserInfos()
        {
            if (BloggyConstant.CheckConnectivity() == false)
            {
                var msg = "No internet connection. Please check and try again";
                await Application.Current.MainPage.DisplayAlert("Connection Error", msg, "Cancel");
                return;
            }

            try
            {
                CurrentUser = await BloggyServices.GetAuthMemberAsync();
            }
            catch (Exception ex)
            {
                var msg = "An error occured, Something went wrong";
                Console.WriteLine(ex.Message);
                await Application.Current.MainPage.DisplayAlert("Error", msg, "Cancel");
                return;
            }
        }
    }
}