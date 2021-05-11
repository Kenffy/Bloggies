using KenffySoft.Bloggy.Helpers;
using KenffySoft.Bloggy.Models;
using KenffySoft.Bloggy.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KenffySoft.Bloggy.ViewModels
{
    public class NotificationsViewModel : BaseViewModel
    {
        private int pageNumber = 0;
        private readonly int pageSize = 10;

        private NotificationDetail selectedNotification;
        private ObservableCollection<NotificationDetail> notificationsCollection;

        public NotificationsViewModel()
        {
            selectedNotification = new NotificationDetail();
            notificationsCollection = new ObservableCollection<NotificationDetail>();
            Init();
        }

        public NotificationDetail SelectedPost
        {
            get => selectedNotification;
            set => SetProperty(ref selectedNotification, value);
        }
        public ObservableCollection<NotificationDetail> NotificationsCollection
        {
            get => notificationsCollection;
            set => SetProperty(ref notificationsCollection, value);
        }

        private async void Init()
        {
            await LoadNotificationsAsync();
        }

        private async Task LoadNotificationsAsync()
        {
            CheckInternetConnection();
            try
            {
                pageNumber++;
                var notifications = await BloggyServices.GetNotificationsAsync(pageNumber, pageSize);
                //PostCollection.AddRange(posts);
                foreach (var noti in notifications)
                {
                    NotificationsCollection.Add(noti);
                }
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
    }
}
