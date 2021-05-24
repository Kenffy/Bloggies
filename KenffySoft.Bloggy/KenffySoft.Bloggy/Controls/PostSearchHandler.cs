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
    public class PostSearchHandler : SearchHandler
    {
        public ObservableCollection<PostDetail> PostItemsSource { get; set; }
        public Type SelectedItemNavigationTarget { get; set; }

        public PostSearchHandler()
        {
            PostItemsSource = new ObservableCollection<PostDetail>();
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
                PostItemsSource = await BloggyServices.GetPostsAsync();
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
                ItemsSource = PostItemsSource
                    .Where(p => p.Title.ToLower().Contains(newValue.ToLower()) ||
                                p.Body.ToLower().Contains(newValue.ToLower()))
                    .ToList();
            }
        }

        protected override async void OnItemSelected(object item)
        {
            base.OnItemSelected(item);
            await Task.Delay(1000);

            var post = item as PostDetail;
            await Shell.Current.GoToAsync($"{nameof(PostDetailPage)}?{nameof(PostDetailViewModel.PostId)}={post.Id}");
        }
    }
}
