﻿using System;
using System.Collections.Generic;
using System.Text;

namespace KenffySoft.Bloggy.Models
{
    public class Bloggy
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string AvatarColor { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string ProfileImage { get; set; }
        public string Followers { get; set; }
        public int NumFollowers { get; set; }
    }
}
