using KenffySoft.Bloggy.Helpers;
using KenffySoft.Bloggy.Models;
using KenffySoft.Bloggy.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace KenffySoft.Bloggy.ViewModels
{
    public class FollowersViewModel : BaseViewModel
    {
        private int pageNumber = 0;
        private int pageSize = 20;

        private Models.Bloggy selectedMember;
        private ObservableCollection<Models.Bloggy> followersCollection;

        public FollowersViewModel()
        {
            selectedMember = new Models.Bloggy();
            followersCollection = new ObservableCollection<Models.Bloggy>();
            Init();
        }

        public Models.Bloggy SelectedMember
        {
            get => selectedMember;
            set => SetProperty(ref selectedMember, value);
        }
        public ObservableCollection<Models.Bloggy> FollowersCollection
        {
            get => followersCollection;
            set => SetProperty(ref followersCollection, value);
        }

        private async void Init()
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
                var members = await BloggyServices.GetFollowersAsync(pageNumber, pageSize);
                foreach (var member in members)
                {
                    FollowersCollection.Add(member);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Oops!!!", "Something went wrong!", "Ok");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
