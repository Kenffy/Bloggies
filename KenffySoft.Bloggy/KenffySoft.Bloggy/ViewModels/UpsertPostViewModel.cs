using ImageToArray;
using KenffySoft.Bloggy.Helpers;
using KenffySoft.Bloggy.Models;
using KenffySoft.Bloggy.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace KenffySoft.Bloggy.ViewModels
{
    public class UpsertPostViewModel : BaseViewModel
    {
        private bool isNewPost;
        private bool isPublicPost;
        private string postTitle;
        private string postBody;
        private Image postImage;
        private MediaFile file;
        private PostDetail currentPost;
        public Command MediaCommand { get; }
        public Command CameraCommand { get; }
        public Command SaveCommand { get; }

        public UpsertPostViewModel()
        {
            Title = "Create Post";
            isNewPost = true;
            isPublicPost = true;
            postImage = new Image();
            currentPost = new PostDetail();
            MediaCommand = new Command(OnMedia);
            CameraCommand = new Command(OnCamera);
            SaveCommand = new Command(OnSavePost, ValidatePost);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }

        public UpsertPostViewModel(PostDetail post)
        {
            Title = "Update Post";
            isNewPost = false;
            postImage = new Image();
            currentPost = post;
            MediaCommand = new Command(OnMedia);
            CameraCommand = new Command(OnCamera);
            SaveCommand = new Command(OnSavePost, ValidatePost);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();

            Init();
        }

        private void Init()
        {
            if (CurrentPost == null || string.IsNullOrEmpty(CurrentPost.Id.ToString()))
                return;

            PostTitle = CurrentPost.Title;
            PostBody = CurrentPost.Body;

            if (!string.IsNullOrEmpty(CurrentPost.PostImage))
            {
                PostImage.Source = ImageSource.FromUri(new Uri(CurrentPost.PostImage));
            }
        }

        private bool ValidatePost(object arg)
        {
            return !string.IsNullOrWhiteSpace(PostTitle)
                && !string.IsNullOrWhiteSpace(PostBody);
        }

        public bool IsNewPost
        {
            get => isNewPost;
            set => SetProperty(ref isNewPost, value);
        }

        public bool IsPublicPost
        {
            get => isPublicPost;
            set => SetProperty(ref isPublicPost, value);
        }

        public string PostTitle
        {
            get => postTitle;
            set => SetProperty(ref postTitle, value);
        }

        public string PostBody
        {
            get => postBody;
            set => SetProperty(ref postBody, value);
        }

        public Image PostImage
        {
            get => postImage;
            set => SetProperty(ref postImage, value);
        }

        public PostDetail CurrentPost
        {
            get => currentPost;
            set => SetProperty(ref currentPost, value);
        }

        private async void OnSavePost(object obj)
        {
            CurrentPost.Title = PostTitle;
            CurrentPost.Body = PostBody;

            if (BloggyConstant.CheckConnectivity() == false)
            {
                var msg = "No internet connection. Please check and try again";
                await Application.Current.MainPage.DisplayAlert("Connection Error", msg, "Cancel");
                return;
            }

            try
            {
                if (CurrentPost != null && IsNewPost == true)
                {
                    // New post
                    CurrentPost.Id = Guid.NewGuid();
                    CurrentPost.IsPublicPost = IsPublicPost;
                    await BloggyServices.CreatePostAsync(CurrentPost);
                    PostTitle = "";
                    PostBody = "";
                    PostImage = new Image();
                    MessagingCenter.Send(this, "UpsertPostStatus", true);
                    var msg = "Post successfully added.";
                    await Application.Current.MainPage.DisplayAlert("Create Post", msg, "Alright");
                    await Shell.Current.Navigation.PopAsync();
                }
                else
                {
                    // Edit post
                    await BloggyServices.UpdatePostAsync(CurrentPost);
                    MessagingCenter.Send(this, "UpsertPostStatus", true);
                    var msg = "Post successfully updated.";
                    await Application.Current.MainPage.DisplayAlert("Update Post", msg, "Alright");
                    await Shell.Current.Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                var msg = "An error occured, Something went wrong";
                Console.WriteLine(ex.Message);
                MessagingCenter.Send(this, "UpsertPostStatus", false);
                await Application.Current.MainPage.DisplayAlert("Error", msg, "Cancel");
                return;
            }
        }

        private async void OnCamera(object obj)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = "test.jpg"
            });

            PostImage.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });

            CurrentPost.ImageArray = FromFile.ToArray(file.GetStream());
        }

        private async void OnMedia(object obj)
        {
            string action = await Application.Current.MainPage.DisplayActionSheet("Media", "Cancel", null, "Camera", "Gallery");

            await CrossMedia.Current.Initialize();

            if (action == "Camera")
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await Application.Current.MainPage.DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                    return;
                }

                file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                    Directory = "Sample",
                    Name = "test.jpg"
                });
            }
            else if (action == "Gallery")
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await Application.Current.MainPage.DisplayAlert("ERROR", "Pick Photo is NOT supported", "OK");
                    return;
                }

                file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium
                });
            }
            else
            {
                // Do nothing
            }

            if (file == null)
            {
                return;
            }

            PostImage.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });

            CurrentPost.ImageArray = FromFile.ToArray(file.GetStream());
        }

    }
}
