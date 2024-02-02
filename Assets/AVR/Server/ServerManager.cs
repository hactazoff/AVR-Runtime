using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace AVR
{
    namespace Servers
    {
        public class ServerManager : Base.Manager<Server>
        {

            public delegate void OnServerUpdateEvent(Server server);
            public static event OnServerUpdateEvent OnServerUpdate;
            public static void OnUpdate(Server server) => OnServerUpdate?.Invoke(server);

            public delegate void OnServerRemoveEvent(Server server);
            public static event OnServerRemoveEvent OnServerRemove;
            public static void OnRemove(Server server) => OnServerRemove?.Invoke(server);

            public delegate void OnServerAddEvent(Server server);
            public static event OnServerAddEvent OnServerAdd;
            public static void OnAdd(Server server) => OnServerAdd?.Invoke(server);

            public static Server GetServer(string address)
            {
                foreach (var server in Cache)
                    if (server.address == address)
                        return server;
                return null;
            }

            public static void SetServer(Server server)
            {
                if (GetServer(server.address) != null)
                {
                    Cache.Remove(server);
                    Cache.Add(server);
                    OnUpdate(server);
                }
                else
                {
                    Cache.Add(server);
                    OnAdd(server);
                }
            }

            public static void RemoveServer(Server server)
            {
                Cache.Remove(server);
                OnRemove(server);
            }

            public static async UniTask<Server> GetOrFetchServer(string address)
            {
                Server server = GetServer(address);
                if (server != null)
                    return server;
                server = new Server() { address = address };
                return await server.FetchInfos();
            }
        }
    }
}