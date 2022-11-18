using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Login_Logged(object sender, LoggedEventArgs e)
        {
            this.rootGrid.Children.RemoveAt(0);

            this.rootGrid.Children.Add(new MainView(e.App.Instance, e.Auth.AccessToken));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoginControl.CheckLogin();
        }
    }
}
