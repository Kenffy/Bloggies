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
    public class HomeViewModel : BaseViewModel
    {
        private int pageNumber = 0;
        private readonly int pageSize = 5;
        private bool isSearchEnable;
        private bool isCancelSearchEnable;

        private PostDetail selectedPost;
        private ObservableCollection<Category> categories;
        private ObservableCollection<PostDetail> Posts;
        private ObservableCollection<PostDetail> postCollection;

        public bool IsSearchEnable
        {
            get => isSearchEnable;
            set => SetProperty(ref isSearchEnable, value);
        }

        public bool IsCancelSearchEnable
        {
            get => isCancelSearchEnable;
            set => SetProperty(ref isCancelSearchEnable, value);
        }

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

        public ObservableCollection<Category> Categories
        {
            get => categories;
            set => SetProperty(ref categories, value);
        }

        public Command RefreshCommand { get; }
        public Command LoadMoreCommand { get; }
        public Command NotificationCommand { get; }
        public Command<object> SelectedCommand { get; }
        public Command OnSearchCommand { get; }
        public Command OnCancelSearchCommand { get; }
        public Command FilterCommand { get; }
        public HomeViewModel()
        {
            isSearchEnable = false;
            isCancelSearchEnable = true;
            selectedPost = new PostDetail();
            Posts = new ObservableCollection<PostDetail>();
            postCollection = new ObservableCollection<PostDetail>();
            categories = new ObservableCollection<Category>();
            RefreshCommand = new Command(Refresh);
            LoadMoreCommand = new Command(LoadMore);
            SelectedCommand = new Command<object>(OnSelected);
            NotificationCommand = new Command(OnNotifications);
            OnSearchCommand = new Command(OnSearch);
            FilterCommand = new Command(OnFilter);
            OnCancelSearchCommand = new Command(OnCancelSearch);
            Init();
        }

        private async void OnFilter(object obj)
        {
            if (BloggyConstant.CheckConnectivity() == false)
            {
                string msg = "No internet connection. Please check and try again";
                await Application.Current.MainPage.DisplayAlert("Connection Error", msg, "Cancel");
                return;
            }

            try
            {
                var category = obj as Category;

                if (category == null)
                    return;

                if(category.Name == "More...")
                {
                    string msg = "Filter will come soon: " + category.Name + " selected.";
                    await Application.Current.MainPage.DisplayAlert("Filter Information", msg, "Cancel");
                }
                else if (category.Name == "All")
                {
                    string msg = "Filter will come soon: " + category.Name + " selected.";
                    await Application.Current.MainPage.DisplayAlert("Filter Information", msg, "Cancel");
                }
                else
                {
                    string msg = "Filter will come soon: " + category.Name + " selected.";
                    await Application.Current.MainPage.DisplayAlert("Filter Information", msg, "Cancel");
                }
                

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Oops!!!", "Something went wrong!", "Ok");
                Console.WriteLine(ex.Message);
            }
        }

        private void OnCancelSearch(object obj)
        {
            throw new NotImplementedException();
        }

        private void OnSearch(object obj)
        {
            throw new NotImplementedException();
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
            Posts.Clear();
            Categories.Clear();
            PostCollection.Clear();
            await LoadPostAsync();
            IsBusy = false;
        }

        private void LoadMore(object obj)
        {
            //await LoadPostAsync();
            if (Posts.Count > 0 && PostCollection.Count < Posts.Count)
            {
                pageNumber++;
                var tempPosts = Posts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                foreach (var post in tempPosts)
                {
                    PostCollection.Add(post);
                }
            }
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
                var tempCategories = BloggyConstant.GetCategories();
                Categories = new ObservableCollection<Category>(tempCategories.OrderBy(c => c.Name));

                Posts = await BloggyServices.GetAllPostsAsync();

                if (Posts.Count == 0)
                    return;

                pageNumber++;
                var tempPosts = Posts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                foreach (var post in tempPosts)
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
                Posts.Clear();
                Categories.Clear();
                PostCollection.Clear();
                await LoadPostAsync();
            }
        }

        private async void OnNotifications(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NotificationsPage));
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
