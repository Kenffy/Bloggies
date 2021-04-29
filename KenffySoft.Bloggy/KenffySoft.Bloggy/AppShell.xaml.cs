using KenffySoft.Bloggy.ViewModels;
using KenffySoft.Bloggy.Views;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KenffySoft.Bloggy
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(InvitationsPage), typeof(InvitationsPage));
            Routing.RegisterRoute(nameof(ConnectionsPage), typeof(ConnectionsPage));
            Routing.RegisterRoute(nameof(PostDetailPage), typeof(PostDetailPage));
            Routing.RegisterRoute(nameof(ImagePage), typeof(ImagePage));

        }

    }
}
