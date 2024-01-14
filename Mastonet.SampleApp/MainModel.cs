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
        public MastodonClient Client { get; }

        public MainModel(string instance, string token)
        {
            Client = new MastodonClient(instance, token);
            InitModel();
        }

        private async void InitModel()
        {
            var home = await Client.GetHomeTimeline();
            Home = new ObservableCollection<Status>(home);
            var homeStream = Client.GetUserStreaming();
            homeStream.OnUpdate += HomeStream_OnUpdate;
            homeStream.Start();

            rateLimits = new Dictionary<ApiCallCategory, RateLimitEventArgs>();
            Client.RateLimitsUpdated += Client_RateLimitsUpdated;
        }

        Dictionary<ApiCallCategory, RateLimitEventArgs> rateLimits;

        private void Client_RateLimitsUpdated(object sender, RateLimitEventArgs e)
        {
            rateLimits[e.RateLimitCategory] = e;
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

            await Client.PublishStatus(Status, Visibility.Private, mediaIds: mediaIds);
            attachments = new List<Attachment>();
            Status = "";
        }

        List<Attachment> attachments = new List<Attachment>();

        public async Task Upload(Stream stream, string fileName)
        {
            var media = new MediaDefinition(stream, fileName ?? "img");
            var attachment = await Client.UploadMedia(media);
            attachments.Add(attachment);
        }

        public async Task<Account> UploadAvatar(Stream stream, string fileName)
        {
            var media = new MediaDefinition(stream, fileName ?? "img");
            var profile = await Client.UpdateCredentials(avatar: media);
            return profile;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
