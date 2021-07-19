using KenffySoft.Bloggy.Helpers;
using KenffySoft.Bloggy.Models;
using KenffySoft.Bloggy.Services;
using KenffySoft.Bloggy.Views;
using KenffySoft.Bloggy.Views.Users;
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
        private string number = "+49 000000000";
        private string email = "test@gmail.com";
        private Member currentUser;
        public Command EditProfileCommand { get; }
        public Command ResetPasswordCommand { get; }
        public Command AboutCommand { get; }
        public Command LogOutCommand { get; }
        public Command DisplayImageCommand { get; }
        public Command OnMessageCommand { get; }
        public Command OnSMSCommand { get; }
        public Command OnCallCommand { get; }
        public Command WebsiteCommand { get; }

        public Command OnPostsCommand { get; }
        public Command OnFollowersCommand { get; }
        public Command OnFollowingCommand { get; }

        public SettingsViewModel()
        {
            currentUser = new Member();
            AboutCommand = new Command(OnAbout);
            LogOutCommand = new Command(OnLogOut);
            EditProfileCommand = new Command(OnEditProfile);
            DisplayImageCommand = new Command(OnDisplayImage);
            ResetPasswordCommand = new Command(OnResetPassword);
            OnMessageCommand = new Command(OnMessage);
            OnSMSCommand = new Command(OnSMS);
            OnCallCommand = new Command(OnCall);
            WebsiteCommand = new Command(OnWebsite);
            OnPostsCommand = new Command(OnPosts);
            OnFollowersCommand = new Command(OnFollowers);
            OnFollowingCommand = new Command(OnFollowing);
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

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string Number
        {
            get => number;
            set => SetProperty(ref number, value);
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

        private async void LoadUserInfos(string memberId = null)
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

                if (CurrentUser != null)
                {
                    //Title = CurrentUser.Name;
                    Title = "Settings " + CurrentUser.Name;

                    if (!string.IsNullOrEmpty(CurrentUser.PhoneNumber))
                    {
                        Number = CurrentUser.PhoneNumber;
                    }

                    if (!string.IsNullOrEmpty(CurrentUser.Email))
                    {
                        Email = CurrentUser.Email;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = "An error occured, Something went wrong";
                Console.WriteLine(ex.Message);
                await Application.Current.MainPage.DisplayAlert("Error", msg, "Cancel");
                return;
            }
        }

        private void OnMessage(object obj)
        {
            var message = new EmailMessage("", "", Email);
            Xamarin.Essentials.Email.ComposeAsync(message);
        }

        private void OnSMS(object obj)
        {
            var message = new SmsMessage("", Number);
            Sms.ComposeAsync(message);
        }

        private void OnCall(object obj)
        {
            PhoneDialer.Open(Number);
        }

        private async void OnWebsite(object obj)
        {
            await Browser.OpenAsync("https://aka.ms/xamarin-quickstart");
            //var msg = "Currently not available!!!";
            //await Application.Current.MainPage.DisplayAlert("Rate the Application", msg, "Cancel");
            //return;
        }

        private async void OnPosts(object obj)
        {
            var detail = new PostsPage() { BindingContext = new PostViewModel(CurrentUser.Id) };
            await Shell.Current.Navigation.PushAsync(detail); //new BloggyDetailPage(CurrentUser, 0)
        }

        private async void OnFollowers(object obj)
        {
            var detail = new FollowersPage() { BindingContext = new FollowersViewModel(CurrentUser.Id) };
            await Shell.Current.Navigation.PushAsync(detail); //new BloggyDetailPage(CurrentUser, 1)
        }

        private async void OnFollowing(object obj)
        {
            var detail = new FollowingPage() { BindingContext = new FollowingViewModel(CurrentUser.Id) };
            await Shell.Current.Navigation.PushAsync(detail); //new BloggyDetailPage(CurrentUser, 1)
        }

        private async void ResetPassword(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new ForgotPasswordPage());
        }
    }
}
