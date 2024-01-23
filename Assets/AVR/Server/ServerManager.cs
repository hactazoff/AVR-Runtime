using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace AVR {
    namespace Server {
        public class ServerManager : Base.Manager<Server> {

            public static Server GetServer(string address) {
                foreach (var server in Cache)
                    if (server.address == address)
                        return server;
                return null;
            }

            public static void SetServer(Server server) {
                RemoveServer(server);
                Cache.Add(server);
            }

            public static void RemoveServer(Server server) {
                if (GetServer(server.address) != null)
                    Cache.Remove(server);
            }

            public static async UniTask<Server> GetOrFetchServer(string address) {
                Server server = GetServer(address);
                if (server != null)
                    return server;
                server = new Server() { address = address }; 
                return await server.FetchInfos();
            }
        }
    }
}