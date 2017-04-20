using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mastonet.SampleApp
{
    public class MainModel : INotifyPropertyChanged
    {
        public MastodonClient Client { get; }

        public MainModel(AppRegistration app, Auth auth)
        {
            Client = new MastodonClient(app, auth);
            InitModel();
        }


        private async void InitModel()
        {
            var home = await Client.GetHomeTimeline();
            Home = new ObservableCollection<Status>(home);
        }

        private ObservableCollection<Status> home;
        public ObservableCollection<Status> Home
        {
            get { return home; }
            set
            {
                if (home != value)
                {
                    home = value;
                    OnPropertyChanged();
                }
            }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged();
                }
            }
        }

        public Task Post()
        {
            return Client.PostStatus(Status, Visibility.Private);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
