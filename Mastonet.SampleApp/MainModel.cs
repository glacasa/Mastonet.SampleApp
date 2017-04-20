using Mastonet.Entities;
using System;
using System.Collections.Generic;
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
            InitModel().Wait();
        }


        private async Task InitModel()
        {
            //var home = await Client.GetHomeTimeline();
        }







        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
