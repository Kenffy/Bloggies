using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace KenffySoft.Bloggy.ViewModels
{
    public class ImageViewModel : BaseViewModel
    {
        private string imageUrl;

        public Command BackImageCommand { get; }

        public ImageViewModel(string image)
        {
            imageUrl = image;
            BackImageCommand = new Command(OnBackImage);
        }

        private async void OnBackImage(object obj)
        {
            await Shell.Current.Navigation.PopModalAsync();
            //await Shell.Current.GoToAsync("..");
        }

        public string ImageUrl
        {
            get => imageUrl;
            set => SetProperty(ref imageUrl, value);
        }
    }
}
