using Cysharp.Threading.Tasks;

namespace AVR.Worlds
{
    [System.Serializable]
    public class World
    {
        public string id;
        public string title;
        public string description;
        public string owner_id;
        public int capacity;
        public string[] tags;
        public string server;
        public WorldAsset[] assets;
        public Servers.Server Server => Servers.ServerManager.GetServer(server);

        public async UniTask<World> Fetch()
        {
            var server_infos = Server ?? await Servers.ServerManager.GetOrFetchServer(server);
            if (server_infos == null)
                return null;
            var json = await Network.HTTPRequest.Get<World>(server_infos.gateways.CombineHTTP("/api/worlds/" + id));
            if (json == null)
                return null;
            id = json.id;
            title = json.title;
            description = json.description;
            owner_id = json.owner_id;
            capacity = json.capacity;
            tags = json.tags;
            server = json.server;
            assets = json.assets;
            WorldManager.SetWorld(this);
            return this;
        }

        public override string ToString() => id + "@" + server;
    }
}