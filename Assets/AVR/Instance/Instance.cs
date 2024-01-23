using Cysharp.Threading.Tasks;

namespace AVR
{
    namespace Instance
    {
        [System.Serializable]
        public class Instance
        {
            public string id;
            public string name;
            public int capacity;
            public string world;
            public string owner;
            public string master;
            public string server;
            public string[] tags;
            public string[] users;
            public int connected;
            public Server.Server Server => AVR.Server.ServerManager.GetServer(server);

            public async UniTask<Instance> Fetch()
            {
                var server_infos = Server ?? await AVR.Server.ServerManager.GetOrFetchServer(server);
                if (server_infos == null)
                    return null;
                var json = await Network.HTTPRequest.Get<Instance>(server_infos.gateways.CombineHTTP("/api/instances/" + id));
                if (json == null)
                    return null;
                id = json.id;
                name = json.name;
                capacity = json.capacity;
                world = json.world;
                owner = json.owner;
                master = json.master;
                server = json.server;
                tags = json.tags;
                users = json.users;
                connected = json.connected;
                InstanceManager.SetInstance(this);
                return this;
            }
        }
    }
}