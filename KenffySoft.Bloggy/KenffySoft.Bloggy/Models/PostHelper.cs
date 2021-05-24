using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace KenffySoft.Bloggy.Models
{
    public class PostHelper
    {
        public int PostCount { get; set; }
        public ObservableCollection<PostDetail> PostCollection { get; set; }
    }
}
