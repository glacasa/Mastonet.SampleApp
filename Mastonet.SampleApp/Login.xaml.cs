using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public void CheckLogin()
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.AppInfo))
            {
                app = JsonSerializer.Deserialize<AppRegistration>(Properties.Settings.Default.AppInfo);
                client = new AuthenticationClient(app);

                if (!String.IsNullOrEmpty(Properties.Settings.Default.AuthToken))
                {
                    auth = JsonSerializer.Deserialize<Auth>(Properties.Settings.Default.AuthToken);
                    Logged?.Invoke(this, new LoggedEventArgs(app, auth));
                }
                else
                {
                    OpenLogin();
                }
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            client = new AuthenticationClient(instanceName.Text);

            app = await client.CreateApp("Mastonet Sample App", null, null, GranularScope.Read, GranularScope.Write, GranularScope.Follow);

            Properties.Settings.Default.AppInfo = JsonSerializer.Serialize(app);
            Properties.Settings.Default.Save();

            OpenLogin();
        }

        private void OpenLogin()
        {
            var url = client.OAuthUrl();

            browser.Visibility = System.Windows.Visibility.Visible;
            browser.Navigate(url);

        }

        private static Regex authRegex = new Regex(@"/oauth/authorize/native\?code=([a-zA-Z0-9_-]+)", RegexOptions.Compiled);

        private async void browser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var uri = e.Uri.AbsoluteUri;
            var m = authRegex.Match(uri);

            if (m.Success)
            {
                var code = m.Groups[1].Value;

                auth = await client.ConnectWithCode(code);
                Properties.Settings.Default.AuthToken = JsonSerializer.Serialize(auth);
                Properties.Settings.Default.Save();


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
