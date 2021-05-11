using KenffySoft.Bloggy.Helpers;
using KenffySoft.Bloggy.Models;
using KenffySoft.Bloggy.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KenffySoft.Bloggy.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        //private string searchTerm;
        //private PostDetail selectedPost;
        //private ObservableCollection<PostDetail> PostItems;
        //private ObservableCollection<Models.Bloggy> BloggyItems;
        private ObservableCollection<PostDetail> postCollection;
        private ObservableCollection<Models.Bloggy> bloggyCollection;

        //public Command OnSearchCommand { get; }

        public SearchViewModel()
        {
            //selectedPost = new PostDetail();
            //PostItems = new ObservableCollection<PostDetail>();
            //BloggyItems = new ObservableCollection<Models.Bloggy>();
            postCollection = new ObservableCollection<PostDetail>();
            bloggyCollection = new ObservableCollection<Models.Bloggy>();

            //OnSearchCommand = new Command(OnSearch);
            Init();
        }

        //public string SearchTerm
        //{
        //    get => searchTerm;
        //    set => SetProperty(ref searchTerm, value);
        //}

        //public PostDetail SelectedPost
        //{
        //    get => selectedPost;
        //    set => SetProperty(ref selectedPost, value);
        //}
        public ObservableCollection<PostDetail> PostCollection
        {
            get => postCollection;
            set => SetProperty(ref postCollection, value);
        }

        public ObservableCollection<Models.Bloggy> BloggyCollection
        {
            get => bloggyCollection;
            set => SetProperty(ref bloggyCollection, value);
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
                PostCollection = await BloggyServices.GetPostsAsync();
                //BloggyCollection = await BloggyServices.GetAllBloggiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Application.Current.MainPage.DisplayAlert("Error", "An error occured, Something went wrong", "Cancel");
                return;
            }
        }

        //private async void OnSearch()
        //{
        //    CheckInternetConnection();

        //    if (string.IsNullOrEmpty(SearchTerm))
        //    {
        //        SearchTerm = string.Empty;
        //    }

        //    try
        //    {
        //        SearchTerm = SearchTerm.ToLowerInvariant();

        //        var posts = PostItems.Where(p => p.Title.ToLowerInvariant().Contains(SearchTerm) ||
        //                                              p.Body.ToLowerInvariant().Contains(SearchTerm)).ToList();

        //        var bloggies = BloggyItems.Where(p => p.Name.ToLowerInvariant().Contains(SearchTerm)).ToList();

        //        PostCollection.Clear();
        //        BloggyCollection.Clear();

        //        if (!string.IsNullOrWhiteSpace(SearchTerm))
        //        {
        //            PostCollection = new ObservableCollection<PostDetail>(posts);
        //            BloggyCollection = new ObservableCollection<Models.Bloggy>(bloggies); ;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        await Application.Current.MainPage.DisplayAlert("Error", "An error occured, Something went wrong", "Cancel");
        //        return;
        //    }
            
        //}

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
