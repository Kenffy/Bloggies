using System;
using System.Collections.Generic;
using System.Text;

namespace KenffySoft.Bloggy.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public string Target { get; set; }
        public string Link { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Opened { get; set; }
        public bool Viewed { get; set; }
    }
}
