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

        public async UniTask<World> Fetch(bool withEmpty = false)
        {
            var server_infos = Server ?? await Servers.ServerManager.GetOrFetchServer(server);
            if (server_infos == null)
                return null;
            var json = await Network.HTTPRequest.Get<World>(server_infos.gateways.CombineHTTP("/api/worlds/" 
                    + id
                    + (withEmpty ? "?empty=true" : "")));
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
            foreach (var asset in assets)
            {
                asset.world_id = id;
                asset.server = server;
            }
            WorldManager.SetWorld(this);
            return this;
        }
        
        public WorldAsset GetBetterAsset()
        {
            WorldAsset better = null;
            foreach (var asset in assets)
                if (asset.platform == Utils.Platform.CurrentPlatform && asset.engine == Utils.Platform.CurrentEngine)
                    if (better == null || better.version.CompareTo(asset.version) < 0)
                        better = asset;
            return better;
        }

        public override string ToString() => id + "@" + server;
    }
}