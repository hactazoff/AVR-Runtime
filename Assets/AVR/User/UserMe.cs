using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace AVR
{
    namespace Users
    {
        [System.Serializable]
        public class UserMe : User
        {
            public string[] friends;
            public string status;
            public string token;
            public string home;
            
            public Worlds.World Home {
                get {
                    AVR.Utils.Debug.Log("Fetching home world, "+home+"...");
                    if (string.IsNullOrEmpty(home))
                        return null;
                    AVR.Utils.Debug.Log("Home world is set, fetching from cache...");
                    var w = Worlds.WorldManager.GetWorld(home);
                    if (w != null)
                        return w;
                    AVR.Utils.Debug.Log("Home world not found in cache, parsing world patern...");
                    var wp = Worlds.WorldPatern.Parser(home);
                    AVR.Utils.Debug.Log(wp.ToString());
                    if (wp == null)
                        return null;
                    AVR.Utils.Debug.Log("Home world patern parsed, fetching world...");
                    return new Worlds.World() { id = wp.id, server = wp.server };
                }
            }

            /**
             * Fetch the user infos.
             */
            public async UniTask<UserMe> FetchMe()
            {
                var server_infos = Server ?? await Servers.ServerManager.GetOrFetchServer(server);
                if (server_infos == null)
                    return null;
                var request = UnityWebRequest.Get(server_infos.gateways.CombineHTTP("/api/users/@me"));
                request.SetRequestHeader("Authorization", token);
                var json = await Network.HTTPRequest.Get<UserMe>(null, request);
                if (json == null)
                    return null;
                id = json.id;
                username = json.username;
                display = json.display;
                thumbnail = json.thumbnail;
                banner = json.banner;
                tags = json.tags;
                server = json.server;
                friends = json.friends;
                status = json.status;
                token = json.token;
                UserManager.SetUser(this);
                return this;
            }
        }
    }
}