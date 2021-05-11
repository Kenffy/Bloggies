using System;
using System.Collections.Generic;
using System.Text;

namespace KenffySoft.Bloggy.Models
{
    public class NotificationDetail
    {
        public Guid Id { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderProfile { get; set; }
        public string SenderAvatar { get; set; }
        public string SenderAvatarColor { get; set; }
        public string ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverProfile { get; set; }
        public string ReceiverAvatar { get; set; }
        public string ReceiverAvatarColor { get; set; }
        public string Message { get; set; }
        public string Target { get; set; }
        public string Link { get; set; }
        public DateTime CreatedAt { get; set; }
        public string NotifyAt { get; set; }
        public bool Opened { get; set; }
        public bool Viewed { get; set; }
    }
}
