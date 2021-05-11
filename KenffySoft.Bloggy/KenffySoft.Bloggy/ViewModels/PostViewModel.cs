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
    public class PostViewModel : BaseViewModel
    {
        private int pageNumber = 0;
        private readonly int pageSize = 10;
        private PostDetail selectedPost;
        private ObservableCollection<PostDetail> postList;
        public Command RefreshCommand { get; }
        public Command AddCommand { get; }
        public Command EditCommand { get; }
        public Command DeleteCommand { get; }
        public Command LoadMoreCommand { get; }
        public Command<object> SelectedCommand { get; }

        public PostViewModel()
        {
            selectedPost = new PostDetail();
            postList = new ObservableCollection<PostDetail>();
            RefreshCommand = new Command(OnRefresh);
            AddCommand = new Command(OnCreatePost);
            EditCommand = new Command<object>(OnEditPost);
            DeleteCommand = new Command<object>(OnDeletePost);
            SelectedCommand = new Command<object>(OnSelected);
            LoadMoreCommand = new Command(LoadMorePosts);
            LoadPostAsync();
        }

        public ObservableCollection<PostDetail> PostList
        {
            get => postList;
            set => SetProperty(ref postList, value);
        }

        public PostDetail SelectedPost
        {
            get => selectedPost;
            set => SetProperty(ref selectedPost, value);
        }

        private void OnSelected(object obj)
        {
            if (SelectedPost == null)
                return;
            SelectedPost = null;
        }

        private async void OnCreatePost(object obj)
        {
            var viewmodel = new UpsertPostViewModel();
            var upsert = new UpsertPostPage { BindingContext = viewmodel };

            MessagingCenter.Subscribe<UpsertPostViewModel, bool>(this, "UpsertPostStatus", OnRefreshPost);
            await Shell.Current.Navigation.PushAsync(upsert);
            MessagingCenter.Unsubscribe<PostViewModel>(this, "UpsertPostStatus");
        }

        private void OnRefreshPost(UpsertPostViewModel arg1, bool arg2)
        {
            if (arg2 == true)
            {
                pageNumber = 0;
                PostList.Clear();
                LoadPostAsync();
            }
        }

        private async void OnEditPost(object obj)
        {
            var post = obj as PostDetail;
            if (post == null || string.IsNullOrEmpty(post.Id.ToString()))
                return;

            var viewmodel = new UpsertPostViewModel(post);
            var upsert = new UpsertPostPage { BindingContext = viewmodel };
            MessagingCenter.Subscribe<UpsertPostViewModel, bool>(this, "UpsertPostStatus", OnRefreshPost);
            await Shell.Current.Navigation.PushAsync(upsert);
            MessagingCenter.Unsubscribe<PostViewModel>(this, "UpsertPostStatus");
        }

        private void OnRefresh(object obj)
        {
            IsBusy = true;
            pageNumber = 0;
            PostList.Clear();
            LoadPostAsync();
            IsBusy = false;
        }

        private async void OnDeletePost(object obj)
        {
            var post = obj as PostDetail;
            if (post == null || string.IsNullOrEmpty(post.Id.ToString()))
                return;

            var msg = "Do you really want to delete this post?";
            var result = await Application.Current.MainPage.DisplayAlert("DELETE ALERT", msg, "Yes", "No");
            if (result == true)
            {
                try
                {
                    await BloggyServices.DeletePostAsync(post);
                    await Application.Current.MainPage.DisplayAlert("DELETE", "Post successfully deleted", "Alright");
                    pageNumber = 0;
                    PostList.Clear();
                    LoadPostAsync();
                }
                catch
                {
                    await Application.Current.MainPage.DisplayAlert("DELETE", "An error occured, Something went wrong.", "Cancel");
                }
            }
        }

        private void LoadMorePosts(object obj)
        {
            LoadPostAsync();
        }

        private async void LoadPostAsync()
        {
            if (BloggyConstant.CheckConnectivity() == false)
            {
                var msg = "No internet connection. Please check and try again";
                await Application.Current.MainPage.DisplayAlert("Connection Error", msg, "Cancel");
                return;
            }

            try
            {
                pageNumber++;
                var posts = await BloggyServices.GetAllPostsByIdAsync(pageNumber, pageSize);
                foreach (var post in posts)
                {
                    PostList.Add(post);
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
