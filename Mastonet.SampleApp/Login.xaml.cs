using Mastonet.Entities;
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
        public event EventHandler<LoggedEventArgs> Logged;

        private AuthenticationClient client;

        private AppRegistration app;
        private Auth auth;

        public Login()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            client = new AuthenticationClient(instanceName.Text);
            app = await client.CreateApp("Mastonet Sample App", Scope.Read | Scope.Write | Scope.Follow);
            // TODO : save app

            var url = client.OAuthUrl();

            browser.Visibility = System.Windows.Visibility.Visible;
            browser.Navigate(url);
        }

        private static Regex authRegex = new Regex("/oauth/authorize/([a-z0-9]{64})", RegexOptions.Compiled);

        private async void browser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var uri = e.Uri.AbsoluteUri;
            var m = authRegex.Match(uri);

            if (m.Success)
            {
                var code = m.Groups[1].Value;

                auth = await client.ConnectWithCode(code);
                // TODO : save token

                Logged?.Invoke(this, new LoggedEventArgs(app, auth));
            }

        }

   
    }

    public class LoggedEventArgs : EventArgs
    {
        public AppRegistration App { get; set; }
        public Auth Auth { get; set; }

        public LoggedEventArgs(AppRegistration app, Auth auth)
        {
            App = app;
            Auth = auth;
        }
    }
}
