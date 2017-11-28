using Mastonet.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainModel Model { get { return (MainModel)DataContext; } }

        public MainView(AppRegistration app, Auth auth)
        {
            InitializeComponent();
            this.DataContext = new MainModel(app, auth);
        }

        private async void Toot_Click(object sender, RoutedEventArgs e)
        {
            await Model.Post();
        }

        private void Image_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "*.jpg|*.png";
            dialog.Multiselect = false;

            var result = dialog.ShowDialog();
            if (result == true)
            {
                var file = dialog.OpenFile();
                var name = dialog.SafeFileName;
                Model.Upload(file, name);
            }
        }


        private void Avatar_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "*.jpg|*.png";
            dialog.Multiselect = false;

            var result = dialog.ShowDialog();
            if (result == true)
            {
                var file = dialog.OpenFile();
                var name = dialog.SafeFileName;
                Model.UploadAvatar(file, name);
            }
        }
    }
}
