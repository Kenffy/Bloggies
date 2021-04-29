using KenffySoft.Bloggy.Helpers;
using KenffySoft.Bloggy.Models;
using KenffySoft.Bloggy.Services;
using KenffySoft.Bloggy.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KenffySoft.Bloggy.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private int pageNumber = 0;
        private readonly int pageSize = 10;

        private PostDetail selectedPost;
        private ObservableCollection<PostDetail> postCollection;

        public PostDetail SelectedPost
        {
            get => selectedPost;
            set => SetProperty(ref selectedPost, value);
        }
        public ObservableCollection<PostDetail> PostCollection
        {
            get => postCollection;
            set => SetProperty(ref postCollection, value);
        }

        public Command RefreshCommand { get; }
        public Command LoadMoreCommand { get; }

        public Command<object> SelectedCommand { get; }
        public HomeViewModel()
        {
            selectedPost = new PostDetail();
            postCollection = new ObservableCollection<PostDetail>();
            RefreshCommand = new Command(Refresh);
            LoadMoreCommand = new Command(LoadMore);
            SelectedCommand = new Command<object>(OnSelected);
            Init();
        }

        private async void Init()
        {
            await LoadPostAsync();
        }

        private async void Refresh(object obj)
        {
            if (IsBusy)
                return;
            IsBusy = true;
            pageNumber = 0;
            PostCollection.Clear();
            await LoadPostAsync();
            IsBusy = false;
        }

        private async void LoadMore(object obj)
        {
            await LoadPostAsync();
        }

        private async void OnSelected(object obj)
        {
            if (SelectedPost == null)
                return;

            MessagingCenter.Subscribe<PostDetailViewModel, bool>(this, "UpsertPostStatus", OnRefreshPost);
            await Shell.Current.GoToAsync($"{nameof(PostDetailPage)}?{nameof(PostDetailViewModel.PostId)}={SelectedPost.Id}");
            MessagingCenter.Unsubscribe<PostDetailViewModel>(this, "UpsertPostStatus");
            SelectedPost = null;
        }

        private async Task LoadPostAsync()
        {
            CheckInternetConnection();
            try
            {
                pageNumber++;
                var posts = await BloggyServices.GetAllPostsAsync(pageNumber, pageSize);
                //PostCollection.AddRange(posts);
                foreach (var post in posts)
                {
                    PostCollection.Add(post);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Application.Current.MainPage.DisplayAlert("Error", "An error occured, Something went wrong", "Cancel");
                return;
            }
        }

        private async void OnRefreshPost(PostDetailViewModel arg1, bool arg2)
        {
            if (arg2 == true)
            {
                pageNumber = 0;
                PostCollection.Clear();
                await LoadPostAsync();
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
    }
}
