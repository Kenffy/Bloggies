using System;
using System.Collections.Generic;
using System.Text;

namespace KenffySoft.Bloggy.Models
{
    public class CommentDetail
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public string Role { get; set; }
        public string AddedBy { get; set; }
        public string MemberId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid PostId { get; set; }

        public string PostedAt { get; set; }
        public string MemberName { get; set; }
        public string ProfileImage { get; set; }
        public string Avatar { get; set; }
        public string AvatarColor { get; set; }
    }
}
