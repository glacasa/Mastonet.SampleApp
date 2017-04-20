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

namespace Mastonet.SampleApp
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public event EventHandler<EventArgs> Logged;

        public Login()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var app = await StaticClient.Register(instanceName.Text);
            // TODO : save app

            var url = StaticClient.Client.OAuthUrl();

            browser.Visibility = System.Windows.Visibility.Visible;
            browser.Navigate(url);
        }

        private static Regex authRegex = new Regex("/oauth/authorize/([a-z0-9]{64})", RegexOptions.Compiled);

        private  async void browser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var uri = e.Uri.AbsoluteUri;
            var m = authRegex.Match(uri);

            if (m.Success)
            {
                var code = m.Groups[1].Value;

                var auth = await StaticClient.Client.ConnectWithCode(code);
                // TODO : save token

                Logged?.Invoke(this, EventArgs.Empty);
            }

        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Logged?.Invoke(this, EventArgs.Empty);
        //}
    }
}
