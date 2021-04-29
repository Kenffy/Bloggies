using KenffySoft.Bloggy.Helpers;
using KenffySoft.Bloggy.Models;
using KenffySoft.Bloggy.Services;
using KenffySoft.Bloggy.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KenffySoft.Bloggy.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private string settingsTitle;
        private Member currentUser;
        public Command EditProfileCommand { get; }
        public Command ResetPasswordCommand { get; }
        public Command AboutCommand { get; }
        public Command LogOutCommand { get; }
        public Command DisplayImageCommand { get; }

        public SettingsViewModel()
        {
            currentUser = new Member();
            AboutCommand = new Command(OnAbout);
            LogOutCommand = new Command(OnLogOut);
            EditProfileCommand = new Command(OnEditProfile);
            DisplayImageCommand = new Command(OnDisplayImage);
            ResetPasswordCommand = new Command(OnResetPassword);
            LoadUserInfos();
        }

        public Member CurrentUser
        {
            get => currentUser;
            set => SetProperty(ref currentUser, value);
        }

        public string SettingsTitle
        {
            get => settingsTitle;
            set => SetProperty(ref settingsTitle, value);
        }

        private async void OnLogOut(object obj)
        {
            #region Bloggy FireBase
            if (BloggyConstant.CheckConnectivity() == false)
            {
                var msg = "No internet connection. Please check and try again";
                await Application.Current.MainPage.DisplayAlert("Connection Error", msg, "Cancel");
                return;
            }
            Preferences.Remove("BloggyToken");
            Preferences.Remove("Bloggy_Email");
            Preferences.Remove("Bloggy_Password");
            Application.Current.MainPage = new NavigationPage(new LoginPage());

            #endregion
        }

        private async void OnAbout(object obj)
        {
            await Shell.Current.Navigation.PushAsync(new AboutPage() { BindingContext = new AboutViewModel()});
        }

        private async void OnResetPassword(object obj)
        {
            await Shell.Current.Navigation.PushAsync(new ForgotPasswordPage());
        }

        private async void OnEditProfile(object obj)
        {
            if (CurrentUser == null) return;

            var editprofile = new EditProfilePage();
            var editprofilevm = new EditProfileViewModel(CurrentUser);
            editprofile.BindingContext = editprofilevm;
            editprofilevm.RefreshSettingsPage += OnRefreshSettingsPage;
            await Shell.Current.Navigation.PushAsync(editprofile);
        }

        private void OnRefreshSettingsPage(object sender, EventArgs e)
        {
            LoadUserInfos();
        }

        private async void OnDisplayImage(object obj)
        {
            if (string.IsNullOrEmpty(CurrentUser.ProfileImage))
                return;

            var imagepage = new ImagePage { BindingContext = new ImageViewModel(CurrentUser.ProfileImage) };
            await Shell.Current.Navigation.PushModalAsync(imagepage);
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
                SettingsTitle = "Settings " + CurrentUser.Name;
                //if (string.IsNullOrEmpty(CurrentUser.ProfileImage))
                //{
                //    CurrentUser.ProfileImage = "defaultprofile.png";
                //}
            }
            catch (Exception ex)
            {
                var msg = "An error occured, Something went wrong";
                Console.WriteLine(ex.Message);
                await Application.Current.MainPage.DisplayAlert("Error", msg, "Cancel");
                return;
            }
        }

        private async void ResetPassword(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new ForgotPasswordPage());
        }
    }
}
