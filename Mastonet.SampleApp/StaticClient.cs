using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastonet.SampleApp
{
    public class StaticClient
    {
        public static MastodonClient Client { get; private set; }

        public static async Task<AppRegistration> Register(string instanceName)
        {
            var app = await MastodonClient.CreateApp(instanceName, "Mastonet Sample App", Scope.Read | Scope.Write | Scope.Follow);
            Client = new MastodonClient(app);
            return app;
        }

    }
}
