using System;
using System.Collections.Generic;
using System.Text;

namespace KenffySoft.Bloggy.Models
{
    public class PostDetail
    {
        public Guid Id { get; set; }
        public string MemberId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string VisibilityStatus { get; set; }
        public bool IsPublicPost { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PostImage { get; set; }
        public string PostImageName { get; set; }
        public string PostedAt { get; set; }
        public string ProfileImage { get; set; }
        public string Avatar { get; set; }
        public string AvatarColor { get; set; }
        public string BloggyName { get; set; }
        public string LikeImage { get; set; }
        public string CommentImage { get; set; }
        public string ShortTitle { get; set; }
        public string ShortBody { get; set; }
        public string Details { get; set; }
        public bool IsLikedByMe { get; set; }
        public int NumLikes { get; set; }
        public int NumComments { get; set; }
        public string InfoComments { get; set; }
        public byte[] ImageArray { get; set; }
    }
}
