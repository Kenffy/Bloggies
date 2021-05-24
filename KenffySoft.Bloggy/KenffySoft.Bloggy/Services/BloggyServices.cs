using Firebase.Database;
using Firebase.Auth;
using Firebase.Database.Query;
using Firebase.Storage;
using KenffySoft.Bloggy.Helpers;
using KenffySoft.Bloggy.Models;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Essentials;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace KenffySoft.Bloggy.Services
{
    public static class BloggyServices
    {
        #region Account
        public static async Task<bool> RegisterAsync(RegisterModel model)
        {

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(BloggyConstant.WebAPIKey));
            var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password);
            await authProvider.UpdateProfileAsync(auth.FirebaseToken, model.Username, "");
            await authProvider.SendEmailVerificationAsync(auth);

            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                    new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken) });

            Member member = new Member
            {
                Id = auth.User.LocalId,
                Name = model.Username,
                Description = "Hello Bloggy",
                Avatar = model.Username.ToUpper().FirstOrDefault().ToString(),
                AvatarColor = BloggyColor.GetRandomHexColor(),
                Email = model.Email,
                ProfileImage = "",
                ProfileImageName = "",
                Role = BloggyConstant.UserRole,
                PhoneNumber = "",
                Followers = ",",
                Following = ","
            };

            await client.Child("Members").PostAsync(member);
            return (auth.FirebaseToken != null);
        }
        public static async Task<bool> LoginAsync(LoginModel model)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(BloggyConstant.WebAPIKey));
            var auth = await authProvider.SignInWithEmailAndPasswordAsync(model.Email, model.Password);
            var content = await auth.GetFreshAuthAsync();

            var serializedcontent = JsonConvert.SerializeObject(content);
            Preferences.Set("BloggyToken", serializedcontent);

            return (auth.FirebaseToken != null);
        }

        public static async Task<Member> GetAuthMemberAsync()
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });


               var member = (await client.Child("Members")
                            .OnceAsync<Member>()).FirstOrDefault
                            (a => a.Object.Id == token.User.LocalId);

            if (string.IsNullOrEmpty(member.Object.ProfileImage)) 
            {
                member.Object.ProfileImage = "defaultprofile.png";
            }

            return member.Object;
        }

        public static async Task<Member> GetMemberByIdAsync(string Id)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var member = (await client.Child("Members")
            .OnceAsync<Member>()).FirstOrDefault
            (a => a.Object.Id == Id);

            if (string.IsNullOrEmpty(member.Object.ProfileImage))
            {
                member.Object.ProfileImage = "defaultprofile.png";
            }

            return member.Object;
        }

        public static async Task<string> GetMemberNameByIdAsync(string Id)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var member = (await client.Child("Members")
            .OnceAsync<Member>()).FirstOrDefault
            (a => a.Object.Id == Id);

            return member.Object.Name;
        }

        public static async Task<string> GetMemberProfileImageByIdAsync(string Id)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var member = (await client.Child("Members")
            .OnceAsync<Member>()).FirstOrDefault
            (a => a.Object.Id == Id);

            if (string.IsNullOrEmpty(member.Object.ProfileImage))
            {
                member.Object.ProfileImage = "defaultprofile.png";
            }

            return member.Object.ProfileImage;
        }

        public static async Task<string> GetMemberAvatarByIdAsync(string Id)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var member = (await client.Child("Members")
            .OnceAsync<Member>()).FirstOrDefault
            (a => a.Object.Id == Id);

            return member.Object.Avatar;
        }

        public static async Task<string> GetMemberAvatarColorByIdAsync(string Id)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var member = (await client.Child("Members")
            .OnceAsync<Member>()).FirstOrDefault
            (a => a.Object.Id == Id);

            return member.Object.AvatarColor;
        }

        public static async Task<ObservableCollection<Models.Bloggy>> GetAllBloggiesAsync()
        {
            var authMember = await GetAuthMemberAsync();

            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                    new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var members = (await client
                  .Child("Members")
                  .OnceAsync<Member>()).Select(item => new Models.Bloggy
                  {
                      Id = item.Object.Id,
                      Name = item.Object.Name,
                      Avatar = item.Object.Avatar,
                      AvatarColor = item.Object.AvatarColor,
                      Email = item.Object.Email,
                      Description = item.Object.Description,
                      ProfileImage = string.IsNullOrEmpty(item.Object.ProfileImage) ? "defaultprofile.png" : item.Object.ProfileImage,
                      Followers = item.Object.Followers,
                      NumFollowers = item.Object.NumFollowers
                  }).Where(m => m.Email != authMember.Email).ToList();

            return new ObservableCollection<Models.Bloggy>(members);
        }
        public static async Task<ObservableCollection<Models.Bloggy>> GetBloggiesAsync(int pageNumber, int pageSize)
        {
            var authMember = await GetAuthMemberAsync();

            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                    new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var members = (await client
                  .Child("Members")
                  .OnceAsync<Member>()).Select(item => new Models.Bloggy
                  {
                      Id = item.Object.Id,
                      Name = item.Object.Name,
                      Avatar = item.Object.Avatar,
                      AvatarColor = item.Object.AvatarColor,
                      Email = item.Object.Email,
                      Description = item.Object.Description,
                      ProfileImage = string.IsNullOrEmpty(item.Object.ProfileImage) ? "defaultprofile.png" : item.Object.ProfileImage,
                      Followers = item.Object.Followers,
                      NumFollowers = item.Object.NumFollowers
                  }).Where(m => m.Email != authMember.Email).ToList();

            var bloggies = new List<Models.Bloggy>();

            foreach(var blog in members)
            {
                if (!authMember.Following.Contains(blog.Id))
                {
                    bloggies.Add(blog);
                }
            }

            var filterBloggies = bloggies.Skip((pageNumber - 1) * pageSize).Take(pageSize).OrderByDescending(m => m.Name).ToList();
            return new ObservableCollection<Models.Bloggy>(filterBloggies);
        }
        public static async Task<ObservableCollection<Models.Bloggy>> GetFollowersAsync(string memberId, int pageNumber, int pageSize)
        {
            var member = new Member();

            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                    new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            if(string.IsNullOrEmpty(memberId))
            {
                member = await GetAuthMemberAsync();
            }
            else
            {
                member = (await client.Child("Members")
                .OnceAsync<Member>()).FirstOrDefault
                (a => a.Object.Id == memberId).Object;
            }

            var followers = member.Followers.Split(',');
            followers = followers.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            var bloggies = new List<Models.Bloggy>();

            var members = (await client
                  .Child("Members")
                  .OnceAsync<Member>()).Select(item => new Models.Bloggy
                  {
                      Id = item.Object.Id,
                      Name = item.Object.Name,
                      Avatar = item.Object.Avatar,
                      AvatarColor = item.Object.AvatarColor,
                      Email = item.Object.Email,
                      Description = item.Object.Description,
                      ProfileImage = string.IsNullOrEmpty(item.Object.ProfileImage) ? "defaultprofile.png" : item.Object.ProfileImage,
                      Followers = item.Object.Followers,
                      NumFollowers = item.Object.NumFollowers
                  }).Where(m => m.Email != member.Email).ToList();

            foreach (var follower in followers)
            {
                var blog = members.Where(m => m.Id == follower).FirstOrDefault();
                if (blog != null)
                {
                    bloggies.Add(blog);
                }
            }

            var filterBloggies = bloggies.Skip((pageNumber - 1) * pageSize).Take(pageSize).OrderByDescending(m => m.Name).ToList();
            return new ObservableCollection<Models.Bloggy>(filterBloggies);
        }
        public static async Task<ObservableCollection<Models.Bloggy>> GetFollowingAsync(string memberId, int pageNumber, int pageSize)
        {
            var member = new Member();

            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                    new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            if(string.IsNullOrEmpty(memberId))
            {
                member = await GetAuthMemberAsync();
            }
            else
            {
                member = (await client.Child("Members")
                .OnceAsync<Member>()).FirstOrDefault
                (a => a.Object.Id == memberId).Object;
            }

            var followings = member.Following.Split(',');
            followings = followings.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            var bloggies = new List<Models.Bloggy>();

            var members = (await client
                  .Child("Members")
                  .OnceAsync<Member>()).Select(item => new Models.Bloggy
                  {
                      Id = item.Object.Id,
                      Name = item.Object.Name,
                      Avatar = item.Object.Avatar,
                      AvatarColor = item.Object.AvatarColor,
                      Email = item.Object.Email,
                      Description = item.Object.Description,
                      ProfileImage = string.IsNullOrEmpty(item.Object.ProfileImage) ? "defaultprofile.png" : item.Object.ProfileImage,
                      Followers = item.Object.Followers,
                      NumFollowers = item.Object.NumFollowers
                  }).Where(m => m.Email != member.Email).ToList();

            foreach (var following in followings)
            {
                var blog = members.Where(m => m.Id == following).FirstOrDefault();
                if (blog != null)
                {
                    bloggies.Add(blog);
                }
            }

            var filterBloggies = bloggies.Skip((pageNumber - 1) * pageSize).Take(pageSize).OrderByDescending(m => m.Name).ToList();
            return new ObservableCollection<Models.Bloggy>(filterBloggies);
        }
        public static async Task DeleteMemberAsync(string memberId)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                    new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var member = (await client.Child("Members")
                .OnceAsync<Member>()).FirstOrDefault
                (a => a.Object.Id == memberId);

            await client.Child("Members").Child(member.Key).DeleteAsync();
        }
        public static async Task UpdateMemberAsync(Member model, byte[] ImageArray)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var cancelToken = new CancellationTokenSource();
            var firebaseStorage = new FirebaseStorage(BloggyConstant.UploadConnString,
                new FirebaseStorageOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken), ThrowOnCancel = true });

            var toUpdateMember = (await client.Child("Members")
            .OnceAsync<Member>()).FirstOrDefault
            (a => a.Object.Id == model.Id);

            var user = toUpdateMember.Object;
            user.Name = model.Name;
            user.Description = model.Description;
            user.PhoneNumber = model.PhoneNumber;
            user.Avatar = model.Name.ToUpper().FirstOrDefault().ToString();

            if (ImageArray != null)
            {
                var filename = Guid.NewGuid() + ".png";
                Stream fs = new MemoryStream(ImageArray);
                if (!string.IsNullOrEmpty(user.ProfileImageName))
                {
                    await firebaseStorage.Child("images/profiles").Child(user.ProfileImageName).DeleteAsync();
                }
                var task = await firebaseStorage.Child("images/profiles").Child(filename).PutAsync(fs, cancelToken.Token);
                user.ProfileImage = await firebaseStorage.Child("images/profiles").Child(filename).GetDownloadUrlAsync();
                user.ProfileImageName = filename;
            }

            await client.Child("Members").Child(toUpdateMember.Key).PutAsync(user);
        }
        public static async Task FollowBloggyAsync(string bloggyId)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                    new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var bloggy = (await client.Child("Members")
                .OnceAsync<Member>()).FirstOrDefault
                (a => a.Object.Id == bloggyId);
            var updatedBloggy = bloggy.Object;

            // Check if already follow
            if (bloggy.Object.Followers.Contains(token.User.LocalId))
                return;

            var authMember = (await client.Child("Members")
                .OnceAsync<Member>()).FirstOrDefault
                (a => a.Object.Id == token.User.LocalId);
            var updatedAuthMember = authMember.Object;


            // Add current user to Followers bloggy
            updatedBloggy.Followers += updatedAuthMember.Id + ",";
            updatedBloggy.NumFollowers += 1;

            // Add bloggy to Following current user
            updatedAuthMember.Following += updatedBloggy.Id + ",";
            updatedAuthMember.NumFollowing += 1;

            // update count followers / following for both.
            await client.Child("Members").Child(authMember.Key).PutAsync(updatedAuthMember);
            await client.Child("Members").Child(bloggy.Key).PutAsync(updatedBloggy);

            var msg = updatedAuthMember.Name + " follow " + updatedBloggy.Name + "'s blog.";
            await NotifyFollowers(token.User.LocalId, updatedBloggy.Id, msg, BloggyConstant.FollowBlog);
        }

        #endregion

        #region posts
        public static async Task CreatePostAsync(PostDetail model)
        {

            FirebaseAuthLink token = await GetRefreshLink();
            var cancelToken = new CancellationTokenSource();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var firebaseStorage = new FirebaseStorage(BloggyConstant.UploadConnString,
                new FirebaseStorageOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken), ThrowOnCancel = true });

            var post = new Post
            {
                Id = model.Id,
                Title = model.Title,
                Body = model.Body,
                CreatedAt = DateTime.Now,
                Likes = ",",
                PostImage = "",
                PostImageName = "",
                MemberId = token.User.LocalId,
                IsPublicPost = model.IsPublicPost
            };

            if (model.ImageArray != null)
            {
                var filename = Guid.NewGuid() + ".png";
                Stream fs = new MemoryStream(model.ImageArray);
                if (!string.IsNullOrEmpty(post.PostImageName))
                {
                    await firebaseStorage.Child("images/posts").Child(post.PostImageName).DeleteAsync();
                }
                var task = await firebaseStorage.Child("images/posts").Child(filename).PutAsync(fs, cancelToken.Token);
                post.PostImage = await firebaseStorage.Child("images/posts").Child(filename).GetDownloadUrlAsync();
                post.PostImageName = filename;
            }
            await client.Child("Posts").PostAsync(post);

            var toUpdateUser = (await client.Child("Members")
                    .OnceAsync<Member>()).FirstOrDefault
                    (a => a.Object.Id == token.User.LocalId);

            var user = toUpdateUser.Object;
            user.NumPosts += 1;

            await client.Child("Members").Child(toUpdateUser.Key).PutAsync(user);

            var msg = user.Name + " added a new post.";
            await NotifyFollowers(token.User.LocalId, post.Id.ToString(), msg, BloggyConstant.NewPost);
        }

        public static async Task NotifyFollowers(string senderId, string itemId, string message, string target)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var user = (await client.Child("Members")
                    .OnceAsync<Member>()).FirstOrDefault
                    (a => a.Object.Id == token.User.LocalId).Object;

            var followers = user.Followers.Split(',');
            followers = followers.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            var notification = new Notification()
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = "",
                Message = message,
                Target = target,
                Link = itemId,
                CreatedAt = DateTime.Now,
                Opened = false,
                Viewed = false
            };

            foreach (var follower in followers)
            {
                notification.ReceiverId = follower;
                await client.Child("Notifications").PostAsync(notification);
            }
        }
        public static async Task<PostDetail> GetPostByIdAsync(Guid postId)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var item = (await client.Child("Posts")
                    .OnceAsync<Post>()).FirstOrDefault
                    (a => a.Object.Id == postId);

            var member = (await client.Child("Members")
                    .OnceAsync<Member>()).FirstOrDefault
                    (a => a.Object.Id == item.Object.MemberId);

            var post = new PostDetail
            {
                Id = item.Object.Id,
                MemberId = item.Object.MemberId,
                Title = item.Object.Title,
                Body = item.Object.Body,
                IsPublicPost = item.Object.IsPublicPost,
                CreatedAt = item.Object.CreatedAt,
                PostedAt = BloggyConstant.GetTimeMessage(item.Object.CreatedAt),
                PostImage = string.IsNullOrEmpty(item.Object.PostImage) ? "defaultImage.png" : item.Object.PostImage,
                PostImageName = item.Object.PostImageName,
                NumLikes = item.Object.NumLikes,
                IsLikedByMe = item.Object.Likes.Contains(token.User.LocalId),
                LikeImage = item.Object.Likes.Contains(token.User.LocalId) ? "liked.png" : "unliked.png",
                CommentImage = item.Object.NumComments > 0 ? "comment2.png" : "comment.png",
                NumComments = item.Object.NumComments,
                Details = item.Object.NumLikes.ToString() + " Like(s)    " + item.Object.NumComments.ToString() + " Comment(s)",
                ProfileImage = string.IsNullOrEmpty(member.Object.ProfileImage) ? "defaultprofile.png" : member.Object.ProfileImage,
                BloggyName = member.Object.Name,
                Avatar = member.Object.Avatar,
                AvatarColor = member.Object.AvatarColor,
                InfoComments = (item.Object.NumComments > 0) ? item.Object.NumComments.ToString() + " Comments" : item.Object.NumComments.ToString() + " Comment",
                ShortTitle = (!string.IsNullOrEmpty(item.Object.Title) && item.Object.Title.Length > 50) ? item.Object.Title.Substring(0, 50) + "... " : item.Object.Title
            };

            return post;
        }

        public static async Task<ObservableCollection<PostDetail>> GetPostsAsync()
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var posts = (await client
                  .Child("Posts")
                  .OnceAsync<Post>())
                  .Where(a => a.Object.IsPublicPost)
                  .Select(item => new PostDetail
                  {
                      Id = item.Object.Id,
                      MemberId = item.Object.MemberId,
                      Title = item.Object.Title,
                      Body = item.Object.Body,
                      IsPublicPost = item.Object.IsPublicPost,
                      CreatedAt = item.Object.CreatedAt,
                      PostedAt = BloggyConstant.GetTimeMessage(item.Object.CreatedAt),
                      PostImage = string.IsNullOrEmpty(item.Object.PostImage) ? "defaultImage.png" : item.Object.PostImage,
                      PostImageName = item.Object.PostImageName,
                      NumLikes = item.Object.NumLikes,
                      IsLikedByMe = item.Object.Likes.Contains(token.User.LocalId),
                      LikeImage = item.Object.Likes.Contains(token.User.LocalId) ? "liked.png" : "unliked.png",
                      CommentImage = item.Object.NumComments > 0 ? "comment2.png" : "comment.png",
                      NumComments = item.Object.NumComments,
                      Details = item.Object.NumLikes.ToString() + " Like(s)    " + item.Object.NumComments.ToString() + " Comment(s)"
                  }).OrderByDescending(m => m.CreatedAt).ToList();

            if (posts.Count == 0)
            {
                return null;
            }


            var members = (await client
                  .Child("Members")
                  .OnceAsync<Member>()).Select(item => new Models.Bloggy
                  {
                      Id = item.Object.Id,
                      Name = item.Object.Name,
                      Avatar = item.Object.Avatar,
                      AvatarColor = item.Object.AvatarColor,
                      Email = item.Object.Email,
                      ProfileImage = string.IsNullOrEmpty(item.Object.ProfileImage) ? "defaultprofile.png" : item.Object.ProfileImage
                  }).ToList();

            foreach (var post in posts)
            {
                var member = members.Where(m => m.Id == post.MemberId).FirstOrDefault();
                if (member != null)
                {
                    post.BloggyName = member.Name;
                    post.ProfileImage = member.ProfileImage;
                    post.Avatar = member.Avatar;
                    post.AvatarColor = member.AvatarColor;
                }
            }

            return new ObservableCollection<PostDetail>(posts);
        }
        public static async Task<ObservableCollection<PostDetail>> GetAllPostsAsync()
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var postHelper = new PostHelper();

            var posts = (await client
                  .Child("Posts")
                  .OnceAsync<Post>())
                  .Where(a => a.Object.IsPublicPost)
                  .Select(item => new PostDetail
                  {
                      Id = item.Object.Id,
                      MemberId = item.Object.MemberId,
                      Title = item.Object.Title,
                      Body = item.Object.Body,
                      IsPublicPost = item.Object.IsPublicPost,
                      CreatedAt = item.Object.CreatedAt,
                      PostedAt = BloggyConstant.GetTimeMessage(item.Object.CreatedAt),
                      PostImage = string.IsNullOrEmpty(item.Object.PostImage) ? "defaultImage.png" : item.Object.PostImage,
                      PostImageName = item.Object.PostImageName,
                      NumLikes = item.Object.NumLikes,
                      IsLikedByMe = item.Object.Likes.Contains(token.User.LocalId),
                      LikeImage = item.Object.Likes.Contains(token.User.LocalId) ? "liked.png" : "unliked.png",
                      CommentImage = item.Object.NumComments > 0 ? "comment2.png" : "comment.png",
                      NumComments = item.Object.NumComments,
                      Details = item.Object.NumLikes.ToString() + " Like(s)    " + item.Object.NumComments.ToString() + " Comment(s)"
                  }).OrderByDescending(m => m.CreatedAt).ToList();

            if (posts.Count == 0)
            {
                return null;
            }


            var members = (await client
                  .Child("Members")
                  .OnceAsync<Member>()).Select(item => new Models.Bloggy
                  {
                      Id = item.Object.Id,
                      Name = item.Object.Name,
                      Avatar = item.Object.Avatar,
                      AvatarColor = item.Object.AvatarColor,
                      Email = item.Object.Email,
                      ProfileImage = string.IsNullOrEmpty(item.Object.ProfileImage) ? "defaultprofile.png" : item.Object.ProfileImage
                  }).ToList();

            foreach (var post in posts)
            {
                var member = members.Where(m => m.Id == post.MemberId).FirstOrDefault();
                if (member != null)
                {
                    post.BloggyName = member.Name;
                    post.ProfileImage = member.ProfileImage;
                    post.Avatar = member.Avatar;
                    post.AvatarColor = member.AvatarColor;
                }
            }

            return new ObservableCollection<PostDetail>(posts);
        }
        public static async Task<ObservableCollection<PostDetail>> GetAllPostsByIdAsync(string memberId)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var postHelper = new PostHelper();

            var MemberId = string.Empty;
            if (string.IsNullOrEmpty(memberId))
            {
                MemberId = token.User.LocalId;
            }
            else
            {
                MemberId = memberId;
            }

            var posts = (await client
                  .Child("Posts")
                  .OnceAsync<Post>())
                  .Where(a => a.Object.MemberId == MemberId)
                  .Select(item => new PostDetail
                  {
                      Id = item.Object.Id,
                      MemberId = item.Object.MemberId,
                      Title = item.Object.Title,
                      Body = item.Object.Body,
                      IsPublicPost = item.Object.IsPublicPost,
                      CreatedAt = item.Object.CreatedAt,
                      PostedAt = BloggyConstant.GetTimeMessage(item.Object.CreatedAt),
                      PostImage = string.IsNullOrEmpty(item.Object.PostImage) ? "defaultImage.png" : item.Object.PostImage,
                      PostImageName = item.Object.PostImageName,
                      NumLikes = item.Object.NumLikes,
                      IsLikedByMe = item.Object.Likes.Contains(token.User.LocalId),
                      LikeImage = item.Object.Likes.Contains(token.User.LocalId) ? "liked.png" : "unliked.png",
                      CommentImage = item.Object.NumComments > 0 ? "comment2.png" : "comment.png",
                      NumComments = item.Object.NumComments,
                      Details = item.Object.NumLikes.ToString() + " Like(s)    " + item.Object.NumComments.ToString() + " Comment(s)"
                  }).OrderByDescending(m => m.CreatedAt).ToList();

            if(posts.Count == 0)
            {
                return null;
            }

            var members = (await client
                  .Child("Members")
                  .OnceAsync<Member>()).Select(item => new Models.Bloggy
                  {
                      Id = item.Object.Id,
                      Name = item.Object.Name,
                      Avatar = item.Object.Avatar,
                      AvatarColor = item.Object.AvatarColor,
                      Email = item.Object.Email,
                      ProfileImage = string.IsNullOrEmpty(item.Object.ProfileImage) ? "defaultprofile.png" : item.Object.ProfileImage
                  }).ToList();

            foreach (var post in posts)
            {
                var member = members.Where(m => m.Id == post.MemberId).FirstOrDefault();
                if (member != null)
                {
                    post.BloggyName = member.Name;
                    post.ProfileImage = member.ProfileImage;
                    post.Avatar = member.Avatar;
                    post.AvatarColor = member.AvatarColor;    
                }
            }

            return new ObservableCollection<PostDetail>(posts);
        }

        public static async Task DeletePostAsync(PostDetail model)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var post = (await client.Child("Posts")
            .OnceAsync<Post>()).FirstOrDefault
            (a => a.Object.Id == model.Id);

            await client.Child("Posts").Child(post.Key).DeleteAsync();

            var toUpdateUser = (await client.Child("Members")
                    .OnceAsync<Member>()).FirstOrDefault
                    (a => a.Object.Id == token.User.LocalId);

            var user = toUpdateUser.Object;
            user.NumPosts -= 1;

            await client.Child("Members").Child(toUpdateUser.Key).PutAsync(user);
        }
        public static async Task UpdatePostAsync(PostDetail model)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var cancelToken = new CancellationTokenSource();

            var firebaseStorage = new FirebaseStorage(BloggyConstant.UploadConnString,
                new FirebaseStorageOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken), ThrowOnCancel = true });

            var toUpdatePost = (await client.Child("Posts")
            .OnceAsync<Post>()).FirstOrDefault
            (a => a.Object.Id == model.Id);

            var post = toUpdatePost.Object;
            post.Title = model.Title;
            post.Body = model.Body;
            post.IsPublicPost = model.IsPublicPost;

            if (model.ImageArray != null)
            {
                var filename = Guid.NewGuid() + ".png";
                Stream fs = new MemoryStream(model.ImageArray);
                if (!string.IsNullOrEmpty(post.PostImageName))
                {
                    await firebaseStorage.Child("images/posts").Child(post.PostImageName).DeleteAsync();
                }
                var task = await firebaseStorage.Child("images/posts").Child(filename).PutAsync(fs, cancelToken.Token);
                post.PostImage = await firebaseStorage.Child("images/posts").Child(filename).GetDownloadUrlAsync();
                post.PostImageName = filename;
            }

            await client.Child("Posts").Child(toUpdatePost.Key).PutAsync(post);

        }
        #endregion

        #region Comments
        public static async Task CreatePostCommentAsync(CommentDetail model)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var comment = new Comment
            {
                Id = model.Id,
                Body = model.Body,
                AddedBy = token.User.Email,
                MemberId = token.User.LocalId,
                CreatedAt = DateTime.Now,
                PostId = model.PostId
            };
            await client.Child("Comments").PostAsync(comment);

            var toUpdatePost = (await client.Child("Posts")
            .OnceAsync<Post>()).FirstOrDefault
            (a => a.Object.Id == model.PostId);

            var post = toUpdatePost.Object;
            post.NumComments += 1;
            await client.Child("Posts").Child(toUpdatePost.Key).PutAsync(post);

            var userPost = (await client.Child("Members")
            .OnceAsync<Member>()).FirstOrDefault
            (a => a.Object.Id == toUpdatePost.Object.MemberId).Object;

            var user = (await client.Child("Members")
            .OnceAsync<Member>()).FirstOrDefault
            (a => a.Object.Id == token.User.LocalId).Object;

            var msg = "";
            if(token.User.LocalId == toUpdatePost.Object.MemberId)
            {
                msg = user.Name + " commented his own post.";
            }
            else
            {
                msg = user.Name + " commented " + userPost.Name + "'s post.";
            }
            await NotifyFollowers(token.User.LocalId, post.Id.ToString(), msg, BloggyConstant.LikePost);

        }
        public static async Task<ObservableCollection<CommentDetail>> GetAllCommentsAsync()
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var commentList = new List<CommentDetail>();


            var comments = client
            .Child("Comments")
            .AsObservable<Comment>()
            .AsObservableCollection();

            var members = (await client
                  .Child("Members")
                  .OnceAsync<Member>()).Select(item => new Models.Bloggy
                  {
                      Id = item.Object.Id,
                      Name = item.Object.Name,
                      Avatar = item.Object.Avatar,
                      AvatarColor = item.Object.AvatarColor,
                      Email = item.Object.Email,
                      ProfileImage = string.IsNullOrEmpty(item.Object.ProfileImage) ? "defaultprofile.png" : item.Object.ProfileImage
                  }).ToList();

            foreach (var comment in comments)
            {
                var member = members.Where(m => m.Email == comment.AddedBy).FirstOrDefault();
                if (member != null)
                {
                    var com = new CommentDetail
                    {
                        Id = comment.Id,
                        Body = comment.Body,
                        AddedBy = comment.AddedBy,
                        PostId = comment.PostId,
                        CreatedAt = comment.CreatedAt,
                        MemberId = comment.MemberId,
                        MemberName = member.Name,
                        ProfileImage = member.ProfileImage,
                        PostedAt = BloggyConstant.GetTimeMessage(comment.CreatedAt),
                        Avatar = member.Avatar,
                        AvatarColor = member.AvatarColor
                    };

                    commentList.Add(com);
                }
            }

            return new ObservableCollection<CommentDetail>(commentList);
        }

        public static async Task<ObservableCollection<CommentDetail>> GetCommentsByPostIdAsync(Guid postId)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var commentList = new List<CommentDetail>();


            var comments = (await client.Child("Comments").OnceAsync<Comment>())
                .Select(item => new Comment
                {
                    Id = item.Object.Id,
                    Body = item.Object.Body,
                    AddedBy = item.Object.AddedBy,
                    MemberId = item.Object.MemberId,
                    CreatedAt = item.Object.CreatedAt,
                    PostId = item.Object.PostId
                }).Where(c => c.PostId == postId).ToList();

            var members = (await client
                  .Child("Members")
                  .OnceAsync<Member>()).Select(item => new Models.Bloggy
                  {
                      Id = item.Object.Id,
                      Name = item.Object.Name,
                      Avatar = item.Object.Avatar,
                      AvatarColor = item.Object.AvatarColor,
                      Email = item.Object.Email,
                      ProfileImage = string.IsNullOrEmpty(item.Object.ProfileImage) ? "defaultprofile.png" : item.Object.ProfileImage
                  }).ToList();

            foreach (var comment in comments)
            {
                var member = members.Where(m => m.Email == comment.AddedBy).FirstOrDefault();
                if (member != null)
                {
                    var com = new CommentDetail
                    {
                        Id = comment.Id,
                        Body = comment.Body,
                        AddedBy = comment.AddedBy,
                        PostId = comment.PostId,
                        CreatedAt = comment.CreatedAt,
                        MemberId = comment.MemberId,
                        MemberName = member.Name,
                        ProfileImage = member.ProfileImage,
                        PostedAt = BloggyConstant.GetTimeMessage(comment.CreatedAt),
                        Avatar = member.Avatar,
                        AvatarColor = member.AvatarColor
                    };

                    commentList.Add(com);
                }
            }

            return new ObservableCollection<CommentDetail>(commentList);
        }
        public static async Task DeleteCommentAsync(Comment model)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var member = (await client.Child("Comments")
                .OnceAsync<Comment>()).FirstOrDefault
                (a => a.Object.Id == model.Id);

            await client.Child("Comments").Child(member.Key).DeleteAsync();
        }
        public static async Task UpdateCommentAsync(Comment model)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var member = (await client.Child("Comments")
                .OnceAsync<Comment>()).FirstOrDefault
                (a => a.Object.Id == model.Id);

            await client.Child("Comments").Child(member.Key).PutAsync(model);
        }
        public static async Task<int> GetNumCommentByPostIdAsync(Guid postId)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var comments = (await client.Child("Comments").OnceAsync<Comment>())
                .Select(item => new Comment
                {
                    Id = item.Object.Id,
                    PostId = item.Object.PostId
                }).Where(c => c.PostId == postId).ToList();

            return comments.Count();
        }
        #endregion

        #region Like posts
        public static async Task LikePostAsync(Guid PostId)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var toUpdatePost = (await client.Child("Posts")
            .OnceAsync<Post>()).FirstOrDefault
            (a => a.Object.Id == PostId);

            var userPost = (await client.Child("Members")
            .OnceAsync<Member>()).FirstOrDefault
            (a => a.Object.Id == toUpdatePost.Object.MemberId).Object;

            var user = (await client.Child("Members")
            .OnceAsync<Member>()).FirstOrDefault
            (a => a.Object.Id == token.User.LocalId).Object;

            var post = toUpdatePost.Object;

            if (post.Likes.Contains(token.User.LocalId))
            {
                // Dislike
                post.Likes = post.Likes.Replace(token.User.LocalId + ",", "");
                post.NumLikes--;
            }
            else
            {
                //Like
                post.Likes += token.User.LocalId + ",";
                post.NumLikes++;

                var msg = "";
                if (token.User.LocalId == toUpdatePost.Object.MemberId)
                {
                    msg = user.Name + " liked his own post.";
                }
                else
                {
                    msg = user.Name + " liked " + userPost.Name + "'s post.";
                }
                await NotifyFollowers(token.User.LocalId, post.Id.ToString(), msg, BloggyConstant.LikePost);
            }
            await client.Child("Posts").Child(toUpdatePost.Key).PutAsync(post);
        }
        #endregion

        #region notifications
        public static async Task<ObservableCollection<NotificationDetail>> GetNotificationsAsync(int pageNumber, int pageSize)
        {
            FirebaseAuthLink token = await GetRefreshLink();
            var client = new FirebaseClient(BloggyConstant.ConnectionString,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });

            var user = (await client.Child("Members")
            .OnceAsync<Member>()).FirstOrDefault
            (a => a.Object.Id == token.User.LocalId).Object;

            var notifications = (await client.Child("Notifications").OnceAsync<Notification>())
                .Select(item => new NotificationDetail
                {
                    Id = item.Object.Id,
                    SenderId = item.Object.SenderId,
                    ReceiverId = item.Object.ReceiverId,
                    ReceiverName = user.Name,
                    ReceiverProfile = user.ProfileImage,
                    ReceiverAvatar = user.Avatar,
                    ReceiverAvatarColor = user.AvatarColor,
                    Message = item.Object.Message,
                    CreatedAt = item.Object.CreatedAt,
                    NotifyAt = BloggyConstant.GetTimeMessage(item.Object.CreatedAt),
                    Target = item.Object.Target,
                    Link = item.Object.Link,
                    Opened = item.Object.Opened,
                    Viewed = item.Object.Viewed
                }).Where(n => n.ReceiverId == token.User.LocalId).ToList();

            var members = (await client
                  .Child("Members")
                  .OnceAsync<Member>()).Select(item => new Models.Bloggy
                  {
                      Id = item.Object.Id,
                      Name = item.Object.Name,
                      Avatar = item.Object.Avatar,
                      AvatarColor = item.Object.AvatarColor,
                      ProfileImage = item.Object.ProfileImage,
                  }).Where(m => m.Id != token.User.LocalId).ToList();

            foreach (var noti in notifications)
            {
                var sender = members.Where(m => m.Id == noti.SenderId).FirstOrDefault();
                if (sender != null)
                {
                    noti.SenderName = sender.Name;
                    noti.SenderAvatar = sender.Avatar;
                    noti.SenderAvatarColor = sender.AvatarColor;
                    noti.SenderProfile = sender.ProfileImage;
                }
            }

            notifications.OrderByDescending(m => m.CreatedAt).ToList();

            var filter = notifications.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new ObservableCollection<NotificationDetail>(filter);
        }
        #endregion

        #region Others
        public static async Task<FirebaseAuthLink> GetRefreshLink()
        {
            var authTokenStr = Preferences.Get("BloggyToken", string.Empty);
            var token = JsonConvert.DeserializeObject<FirebaseAuthLink>(authTokenStr);

            if (token.IsExpired())
            {
                var email = Preferences.Get("Bloggy_Email", string.Empty);
                var password = Preferences.Get("Bloggy_Password", string.Empty);
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(BloggyConstant.WebAPIKey));
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);

                token = await auth.GetFreshAuthAsync();
                var serializedcontent = JsonConvert.SerializeObject(token);
                Preferences.Set("BloggyToken", serializedcontent);
            }
            return token;
        }
        #endregion
    }
}
