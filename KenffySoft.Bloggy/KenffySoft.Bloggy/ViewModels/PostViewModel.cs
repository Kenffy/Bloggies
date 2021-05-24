using KenffySoft.Bloggy.Helpers;
using KenffySoft.Bloggy.Models;
using KenffySoft.Bloggy.Services;
using KenffySoft.Bloggy.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KenffySoft.Bloggy.ViewModels
{
    public class PostViewModel : BaseViewModel
    {
        private int pageNumber = 0;
        private readonly int pageSize = 10;
        private readonly string MemberId;
        private PostDetail selectedPost;
        private ObservableCollection<PostDetail> Posts;
        private ObservableCollection<PostDetail> postList;
        public Command RefreshCommand { get; }
        public Command AddCommand { get; }
        public Command EditCommand { get; }
        public Command DeleteCommand { get; }
        public Command LoadMoreCommand { get; }
        public Command<object> SelectedCommand { get; }

        public PostViewModel(string Id = null)
        {
            selectedPost = new PostDetail();
            Posts = new ObservableCollection<PostDetail>();
            postList = new ObservableCollection<PostDetail>();
            RefreshCommand = new Command(OnRefresh);
            AddCommand = new Command(OnCreatePost);
            EditCommand = new Command<object>(OnEditPost);
            DeleteCommand = new Command<object>(OnDeletePost);
            SelectedCommand = new Command<object>(OnSelected);
            LoadMoreCommand = new Command(LoadMorePosts);
            MemberId = Id;
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

        private async void OnSelected(object obj)
        {
            if (SelectedPost == null)
                return;

            //MessagingCenter.Subscribe<PostDetailViewModel, bool>(this, "UpsertPostStatus", OnRefreshPost);
            await Shell.Current.GoToAsync($"{nameof(PostDetailPage)}?{nameof(PostDetailViewModel.PostId)}={SelectedPost.Id}");
            //MessagingCenter.Unsubscribe<PostDetailViewModel>(this, "UpsertPostStatus");
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
                Posts.Clear();
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
            Posts.Clear();
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
                    Posts.Clear();
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
            if (Posts.Count > 0 && PostList.Count < Posts.Count)
            {
                pageNumber++;
                var tempPosts = Posts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                foreach (var post in tempPosts)
                {
                    PostList.Add(post);
                }
            }
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
                Posts = await BloggyServices.GetAllPostsByIdAsync(MemberId);

                if (Posts.Count == 0)
                    return;

                pageNumber++;
                var tempPosts = Posts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                foreach (var post in tempPosts)
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
