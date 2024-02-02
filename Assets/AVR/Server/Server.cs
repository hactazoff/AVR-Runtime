using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace AVR {
    namespace Servers {
        [System.Serializable]
        public class Server {
            public string id;
            public string address;
            public string title;
            public string description;
            public string version;
            public string icon;
            public ServerGateways gateways;

            /**
             * Fetch the server infos.
             */
            public async UniTask<Server> FetchInfos() {
                if(gateways == null)
                    await FetchMostGateway();
                var json = await Network.HTTPRequest.Get<Server>(gateways.CombineHTTP("/api/server"));
                if (json == null)
                    return null;
                id = json.id;
                address = json.address;
                title = json.title;
                description = json.description;
                version = json.version;
                icon = json.icon;
                gateways.http = json.gateways.http;
                gateways.ws = json.gateways.ws;
                gateways.proxy = json.gateways.proxy;
                ServerManager.SetServer(this);
                return this;
            }

            /**
             * Fetch the most suitable gateway for the server.
             */
            public async UniTask<ServerGateways> FetchMostGateway() {
                var protocol = await Network.HTTPRequest.FetchMostProtocol(address);
                if (protocol == null)
                    return null;
                gateways = new ServerGateways() {
                    http = protocol + "://" + address,
                    ws = protocol == "https" ? "wss://" + address : "ws://" + address,
                    proxy = protocol + "://" + address
                };
                return gateways;
            }

            public override string ToString() => ":@" + address;
        }
    }
}