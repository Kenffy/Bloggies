using System;
using System.Collections.Generic;
using System.Text;

namespace KenffySoft.Bloggy.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public string AddedBy { get; set; }
        public string MemberId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid PostId { get; set; }
    }
}
