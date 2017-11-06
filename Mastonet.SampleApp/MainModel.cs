using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mastonet.SampleApp
{
    public class MainModel : INotifyPropertyChanged
    {
        public Compat.MastodonClient Client { get; }

        public MainModel(AppRegistration app, Auth auth)
        {
            Client = new Compat.MastodonClient(app, auth);
            InitModel();
        }
        
        private async void InitModel()
        {
            var home = await Client.GetHomeTimeline();
            Home = new ObservableCollection<Status>(home);
            var homeStream = Client.GetUserStreaming();
            homeStream.OnUpdate += HomeStream_OnUpdate;
            //homeStream.Start();
        }

        // Timelines

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


        private void HomeStream_OnUpdate(object sender, StreamUpdateEventArgs e)
        {
            Home.Insert(0, e.Status);
        }


        // Post

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

        public async Task Post()
        {
            var mediaIds = attachments.Select(a => a.Id);

            await Client.PostStatus(Status, Visibility.Private, mediaIds:mediaIds);
            attachments = new List<Attachment>();
            Status = "";
        }

        List<Attachment> attachments = new List<Attachment>();

        public async Task Upload(Stream stream, string fileName)
        {
            var attachment = await Client.UploadMedia(stream, "img");
            attachments.Add(attachment);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
