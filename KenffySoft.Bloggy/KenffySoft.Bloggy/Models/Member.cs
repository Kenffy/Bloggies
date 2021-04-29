using System;
using System.Collections.Generic;
using System.Text;

namespace KenffySoft.Bloggy.Models
{
    public class Member
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string AvatarColor { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string ProfileImage { get; set; }
        public string PhoneNumber { get; set; }
        public string Followers { get; set; }
        public string Following { get; set; }
        public int NumPosts { get; set; }
        public int NumFollowers { get; set; }
        public int NumFollowing { get; set; }
    }
}
