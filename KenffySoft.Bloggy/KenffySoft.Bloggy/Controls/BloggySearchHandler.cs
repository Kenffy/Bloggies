using KenffySoft.Bloggy.Helpers;
using KenffySoft.Bloggy.Models;
using KenffySoft.Bloggy.Services;
using KenffySoft.Bloggy.ViewModels;
using KenffySoft.Bloggy.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KenffySoft.Bloggy.Controls
{
    public class BloggySearchHandler : SearchHandler
    {
        public ObservableCollection<Models.Bloggy> BloggyItemsSource { get; set; }
        public Type SelectedItemNavigationTarget { get; set; }

        public BloggySearchHandler()
        {
            BloggyItemsSource = new ObservableCollection<Models.Bloggy>();
            Init();
        }

        private async void Init()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            CheckInternetConnection();
            try
            {
                BloggyItemsSource = await BloggyServices.GetAllBloggiesAsync();
                //BloggyCollection = await BloggyServices.GetAllBloggiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Application.Current.MainPage.DisplayAlert("Error", "An error occured, Something went wrong", "Cancel");
                return;
            }
        }

        private async void CheckInternetConnection()
        {
            if (BloggyConstant.CheckConnectivity() == false)
            {
                var msg = "No internet connection. Please check and try again";
                await Application.Current.MainPage.DisplayAlert("Connection Error", msg, "Cancel");
                return;
            }
        }

        protected override void OnQueryChanged(string oldValue, string newValue)
        {
            base.OnQueryChanged(oldValue, newValue);

            if (string.IsNullOrWhiteSpace(newValue))
            {
                ItemsSource = null;
            }
            else
            {
                ItemsSource = BloggyItemsSource
                    .Where(p => p.Name.ToLower().Contains(newValue.ToLower()))
                    .ToList();
            }
        }

        protected override async void OnItemSelected(object item)
        {
            base.OnItemSelected(item);
            await Task.Delay(1000);

            var bloggy = item as Models.Bloggy;
            //await Shell.Current.GoToAsync($"{nameof(PostDetailPage)}?{nameof(PostDetailViewModel.PostId)}={post.Id}");

            var profileview = new ProfilePage() { BindingContext = new ProfileViewModel(bloggy.Id) };
            await Shell.Current.Navigation.PushAsync(profileview);
        }
    }
}
