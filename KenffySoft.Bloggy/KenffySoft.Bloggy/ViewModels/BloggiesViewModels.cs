using KenffySoft.Bloggy.Helpers;
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
    public class BloggiesViewModels : BaseViewModel
    {
        private int pageNumber = 0;
        private int pageSize = 6;

        private double collectionViewOpacity = 1.0;
        private Models.Bloggy selectedBloggy;
        private ObservableCollection<Models.Bloggy> bloggies;

        public Command FollowCommand { get; }
        public Command RefreshCommand { get; }
        public Command LoadMoreCommand { get; }

        public Command ConnectionsCommand { get; }
        public Command<object> SelectedCommand { get; }

        public BloggiesViewModels()
        {
            selectedBloggy = new Models.Bloggy();
            bloggies = new ObservableCollection<Models.Bloggy>();
            RefreshCommand = new Command(Refresh);
            LoadMoreCommand = new Command(LoadMore);
            ConnectionsCommand = new Command(OnConnections);
            FollowCommand = new Command<object>(OnFollowBloggy);
            SelectedCommand = new Command<object>(OnSelected);
            LoadAllBloggies();
        }


        public Models.Bloggy SelectedBloggy
        {
            get => selectedBloggy;
            set => SetProperty(ref selectedBloggy, value);
        }
        public ObservableCollection<Models.Bloggy> Bloggies
        {
            get => bloggies;
            set
            {
                bloggies = value;
                OnPropertyChanged("Bloggies");
            }
            //set => SetProperty(ref bloggies, value);
        }

        public double CollectionViewOpacity
        {
            get => collectionViewOpacity;
            set => SetProperty(ref collectionViewOpacity, value);
        }

        private async void LoadAllBloggies()
        {
            if (BloggyConstant.CheckConnectivity() == false)
            {
                string msg = "No internet connection. Please check and try again";
                await Application.Current.MainPage.DisplayAlert("Connection Error", msg, "Cancel");
                return;
            }

            
            try
            {
                pageNumber++;
                var result = await BloggyServices.GetBloggiesAsync(pageNumber, pageSize);
                foreach(var bloggy in result)
                {
                    Bloggies.Add(bloggy);
                }
                //Bloggies = new ObservableCollection<Models.Bloggy>(result);
            }
            catch(Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Oops!!!", "Something went wrong!", "Ok");
                Console.WriteLine(ex.Message);
            }            
        }

        private async void OnFollowBloggy(object obj)
        {
            if (BloggyConstant.CheckConnectivity() == false)
            {
                string msg = "No internet connection. Please check and try again";
                await Application.Current.MainPage.DisplayAlert("Connection Error", msg, "Cancel");
                return;
            }

            try
            {
                var bloggy = obj as Models.Bloggy;

                if (bloggy.IsFollowActive == false)
                    return;
                bloggy.FollowStatus = BloggyConstant.FollowerStatus;
                bloggy.IsFollowActive = false;
                Bloggies[Bloggies.IndexOf(bloggy)] = bloggy;
                await BloggyServices.FollowBloggyAsync(bloggy.Id);
                

                // TO DO: to be uptimized
                //LoadAllBloggies();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Oops!!!", "Something went wrong!", "Ok");
                Console.WriteLine(ex.Message);
            }
        }

        private async void Refresh(object obj)
        {
            IsBusy = true;
            CollectionViewOpacity = 0;
            await Task.Delay(2000);
            pageNumber = 0;
            Bloggies.Clear();
            LoadAllBloggies();
            IsBusy = false;
            CollectionViewOpacity = 1.0;
        }

        private void LoadMore(object obj)
        {
            //IsBusy = true;
            LoadAllBloggies();
            //IsBusy = false;
        }

        private async void OnSelected(object obj)
        {
            var previous = SelectedBloggy;
            SelectedBloggy = null;

            if (previous == null)
                return;

            //await Shell.Current.GoToAsync(nameof(ProfilePage));
            //await Shell.Current.Navigation.PushAsync(new ProfilePage());
            var profileview = new AboutPage() { BindingContext = new AboutViewModel(previous.Id) };
            await Shell.Current.Navigation.PushAsync(profileview);

            //TO DO: Perform this function
            // Add your code here...

            //var bloggy = obj as Models.Bloggy;
            //SelectedBloggy = null;
            //if (bloggy == null)
            //    return;

            //SelectedBloggy = null;
        }

        //private async Task Refresh()
        //{
        //    IsBusy = true;

        //    await Task.Delay(2000);

        //    Bloggies.Clear();
        //    LoadAllBloggies();

        //    IsBusy = false;
        //}

        

        private async void OnConnections(object obj)
        {
            await Shell.Current.GoToAsync(nameof(ConnectionsPage));

            // This will pop the current page off the navigation stack
            //await Shell.Current.GoToAsync("..");
        }
    }
}
