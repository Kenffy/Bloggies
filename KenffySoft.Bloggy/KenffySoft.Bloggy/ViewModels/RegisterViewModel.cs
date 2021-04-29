using KenffySoft.Bloggy.Helpers;
using KenffySoft.Bloggy.Models;
using KenffySoft.Bloggy.Services;
using KenffySoft.Bloggy.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace KenffySoft.Bloggy.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        //private RegisterModel register;

        private string email;
        private string username;
        private string password;
        public Command LoginCommand { get; }
        public Command RegisterCommand { get; }
        public Command PasswordForgotCommand { get; }

        public RegisterViewModel()
        {
            //register = new RegisterModel();

            LoginCommand = new Command(OnLogin);
            RegisterCommand = new Command(OnRegister, ValidateRegister);
            this.PropertyChanged +=
                (_, __) => RegisterCommand.ChangeCanExecute();
        }

        //public RegisterModel Register
        //{
        //    get => register;
        //    set => SetProperty(ref register, value);
        //}

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        private bool ValidateRegister()
        {
            return !string.IsNullOrWhiteSpace(Email)
                && !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(Password);
        }

        private async void OnRegister()
        {
            if (BloggyConstant.CheckConnectivity() == false)
            {
                await Application.Current.MainPage.DisplayAlert("Connection Error", "No internet connection. Please check and try again", "Cancel");
                return;
            }

            try
            {
                Email = new string(Email.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
                //var Register = new RegisterModel { Email = Email, Username = Username, Password = Username };
                var Register = new RegisterModel { Email = Email, Username = Username, Password = Password };
                var result = await BloggyServices.RegisterAsync(Register);

                if (result)
                {
                    await Application.Current.MainPage.DisplayAlert("Hi", "Your account has been created", "Alright");
                    await Application.Current.MainPage.Navigation.PushModalAsync(new LoginPage());
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Register Error", "Something went wrong!", "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Register Error", "Something went wrong!", "Ok");
                Console.WriteLine(ex.Message);
            }
        }

        private void OnLogin()
        {
            Application.Current.MainPage.Navigation.PushModalAsync(new LoginPage());
        }
    }
}
