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
    public class ProfileViewModel : BaseViewModel
    {
        private string number = "+49 000000000";
        private string email = "test@gmail.com";
        private string version = "Version 1.0.1";

        public string MemberId;
        public string followStatus;

        private Member currentUser;
        public Command OnPostsCommand { get; }
        public Command OnFollowersCommand { get; }
        public Command OnFollowingCommand { get; }
        public Command DisplayImageCommand { get; }
        public Command WebsiteCommand { get; }
        public Command OnMessageCommand { get; }
        public Command OnSMSCommand { get; }
        public Command OnCallCommand { get; }
        public Command FollowCommand { get; }

        public ProfileViewModel(string memberId = null)
        {
            Title = "About";
            followStatus = BloggyConstant.FollowStatus;
            MemberId = memberId;
            currentUser = new Member();
            OnPostsCommand = new Command(OnPosts);
            OnFollowersCommand = new Command(OnFollowers);
            OnFollowingCommand = new Command(OnFollowing);
            DisplayImageCommand = new Command(OnDisplayImage);
            WebsiteCommand = new Command(OnWebsite);
            OnMessageCommand = new Command(OnMessage);
            OnSMSCommand = new Command(OnSMS);
            OnCallCommand = new Command(OnCall);
            FollowCommand = new Command(OnFollowBloggy);
            LoadUserInfos(MemberId);
        }


        private async void OnWebsite(object obj)
        {
            await Browser.OpenAsync("https://aka.ms/xamarin-quickstart");
            //var msg = "Currently not available!!!";
            //await Application.Current.MainPage.DisplayAlert("Rate the Application", msg, "Cancel");
            //return;
        }


        public Member CurrentUser
        {
            get => currentUser;
            set => SetProperty(ref currentUser, value);
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

        public string Version
        {
            get => version;
            set => SetProperty(ref version, value);
        }

        public string FollowStatus
        {
            get => followStatus;
            set => SetProperty(ref followStatus, value);
        }

        private async void OnPosts(object obj)
        {
            var detail = new PostsPage() { BindingContext = new PostViewModel(MemberId) };
            await Shell.Current.Navigation.PushAsync(detail); //new BloggyDetailPage(CurrentUser, 0)
        }

        private async void OnFollowers(object obj)
        {
            var detail = new FollowersPage() { BindingContext = new FollowersViewModel(MemberId) };
            await Shell.Current.Navigation.PushAsync(detail); //new BloggyDetailPage(CurrentUser, 1)
        }

        private async void OnFollowing(object obj)
        {
            var detail = new FollowingPage() { BindingContext = new FollowingViewModel(MemberId) };
            await Shell.Current.Navigation.PushAsync(detail); //new BloggyDetailPage(CurrentUser, 1)
        }

        private async void OnDisplayImage(object obj)
        {
            if (string.IsNullOrEmpty(CurrentUser.ProfileImage))
                return;

            var imagepage = new ImagePage { BindingContext = new ImageViewModel(CurrentUser.ProfileImage) };
            await Shell.Current.Navigation.PushModalAsync(imagepage);
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

        private async void OnFollowBloggy()
        {
            if (BloggyConstant.CheckConnectivity() == false)
            {
                string msg = "No internet connection. Please check and try again";
                await Application.Current.MainPage.DisplayAlert("Connection Error", msg, "Cancel");
                return;
            }

            try
            {

                if (FollowStatus == BloggyConstant.FollowStatus)
                {
                    await BloggyServices.FollowBloggyAsync(CurrentUser.Id);
                    //TO DO unfollow
                    FollowStatus = BloggyConstant.FollowerStatus;
                }
                else if(FollowStatus == BloggyConstant.UnFollowStatus)
                {
                    // TO DO: Unfollow
                }
                
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Oops!!!", "Something went wrong!", "Ok");
                Console.WriteLine(ex.Message);
            }
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

                CurrentUser = await BloggyServices.GetMemberByIdAsync(memberId);

                if (CurrentUser.Followers.Contains(App.AuthUserId))
                {
                    FollowStatus = BloggyConstant.UnFollowStatus;
                }
                else
                {
                    FollowStatus = BloggyConstant.FollowStatus;
                }
                    

                if (CurrentUser != null)
                {
                    MemberId = CurrentUser.Id;
                    Title = CurrentUser.Name;
                    //Title = "About " + CurrentUser.Name;
                    Version = CurrentUser.Name + ": Version 1.0.0";

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
    }
}
