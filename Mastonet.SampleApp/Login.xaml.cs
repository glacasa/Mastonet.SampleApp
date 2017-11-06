using Mastonet.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public void CheckLogin()
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.AppInfo) && !String.IsNullOrEmpty(Properties.Settings.Default.Instance))
            {
                app = JsonConvert.DeserializeObject<AppRegistration>(Properties.Settings.Default.AppInfo);
                app.Instance = Properties.Settings.Default.Instance;
                app.Scope = Scope.Read | Scope.Write | Scope.Follow;
                client = new AuthenticationClient(app);

                if (!String.IsNullOrEmpty(Properties.Settings.Default.AuthToken))
                {
                    auth = JsonConvert.DeserializeObject<Auth>(Properties.Settings.Default.AuthToken);
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

            app = await client.CreateApp("Mastonet Sample App", Scope.Read | Scope.Write | Scope.Follow);

            Properties.Settings.Default.AppInfo = JsonConvert.SerializeObject(app);
            Properties.Settings.Default.Instance = instanceName.Text;
            Properties.Settings.Default.Save();

            OpenLogin();
        }

        private void OpenLogin()
        {
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
                Properties.Settings.Default.AuthToken = JsonConvert.SerializeObject(auth);
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
