using System;
using System.Collections.Generic;
using System.Text;

namespace KenffySoft.Bloggy.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PostImage { get; set; }
        public string Likes { get; set; }
        public int NumLikes { get; set; }
        public int NumComments { get; set; }
        public string MemberId { get; set; }
        public bool IsPublicPost { get; set; }
    }
}
