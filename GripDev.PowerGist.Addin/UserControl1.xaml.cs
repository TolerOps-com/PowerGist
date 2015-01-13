﻿using GistsApi;
using Microsoft.PowerShell.Host.ISE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GripDev.PowerGist.Addin
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : IAddOnToolHostObject
    {
        private GistClient gistClient;

        public UserControl1()
        {
            InitializeComponent();

            gistClient = new GistClient("1eb530bea98d9f863c57", "1e55daaec72d64581f8688e7bbb3e779c83b3262", "powershellISEAddin");

            //navigate to "https://github.com/login/oauth/authorize" 
            webBrowser.Navigate(gistClient.AuthorizeUrl);
        }

        private async void webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (e.Uri == null)
            { return; }

            if (e.Uri.AbsoluteUri.Contains("code="))
            {
                var authCode = Regex.Split(e.Uri.AbsoluteUri, "code=")[1];

                //get access token
                await gistClient.Authorize(authCode);

                await GetUserGists();
            }
        }

        private async Task GetUserGists()
        {
            var gists = await gistClient.ListGists(GistClient.ListMode.AuthenticatedUserGist);

            foreach (var g in gists)
            {
                listBox.Items.Add(g.files.Select(x => x.filename));
            }
        }

        public ObjectModelRoot HostObject { get; set; }

        private void webBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri.ToString().Contains("gripdev"))
            {
                webBrowser.Visibility = Visibility.Collapsed;
            }
        }
    }
}
