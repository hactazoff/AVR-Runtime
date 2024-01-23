using Cysharp.Threading.Tasks;

namespace AVR
{
    namespace User
    {

        [System.Serializable]
        public class User
        {
            public string id;
            public string username;
            public string display;
            public string thumbnail;
            public string banner;
            public string[] tags;
            public string server;
            public Server.Server Server => AVR.Server.ServerManager.GetServer(server);

            /**
             * Fetch the user infos.
             */
            public async UniTask<User> Fetch()
            {
                var server_infos = Server ?? await AVR.Server.ServerManager.GetOrFetchServer(server);
                if (server_infos == null)
                    return null;
                var json = await Network.HTTPRequest.Get<User>(server_infos.gateways.CombineHTTP("/api/users/" + (id ?? username)));
                if (json == null)
                    return null;
                id = json.id;
                username = json.username;
                display = json.display;
                thumbnail = json.thumbnail;
                banner = json.banner;
                tags = json.tags;
                server = json.server;
                UserManager.SetUser(this);
                return this;
            }
        }
    }
}