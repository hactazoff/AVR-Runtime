using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


namespace AVR
{
    [System.Serializable]
    public class Server
    {

        public string id;
        public string version;
        public string title;
        public string address;
        public string description;
        public bool secure;
        public int ready_at;
        public AVR.ServerIntegrity integrity;

        public string icon;
        public Texture2D Icon = null;

        public AVR.ServerGateway gateways;

        public delegate void OnServerInfo(AVR.Server info);
        public static event OnServerInfo onServerInfo;

        // GET /api/server
        public void GetInfoAsync(System.Action<AVR.Server> callback)
        {
            string path = "/api/server";
            if (address != AVR.ServerManager.ServerMe?.address)
                path += "@" + address;
            AVR.Downloader.FetchAsync<AVR.Server, int>(path, server => AVR.Utils.GetStartup().StartCoroutine(ParserInfoAsync(server, callback)), AVR.ServerManager.ServerMe ?? this);
        }

        private IEnumerator ParserInfoAsync(AVR.Server server, System.Action<AVR.Server> callback)
        {
            if (server == null)
            {
                onServerInfo?.Invoke(null);
                callback(null);
                yield break;
            }
            id = server.id;
            version = server.version;
            title = server.title;
            address = server.address;
            description = server.description;
            secure = server.secure;
            ready_at = server.ready_at;
            icon = server.icon;
            gateways = server.gateways;
            if (icon != null)
            {
                UnityWebRequest thumbnailRequest = UnityWebRequestTexture.GetTexture(icon);
                yield return thumbnailRequest.SendWebRequest();
                if (thumbnailRequest.result == UnityWebRequest.Result.ConnectionError || thumbnailRequest.result == UnityWebRequest.Result.ProtocolError)
                    AVR.Debug.Log("Error: " + thumbnailRequest.error);
                else Icon = ((DownloadHandlerTexture)thumbnailRequest.downloadHandler).texture;
            }
            AVR.ServerManager.SetServer(this);
            onServerInfo?.Invoke(this);
            callback(this);
        }

        public void GetIntegrityAsync(string server_address, Action<AVR.ServerIntegrity> callback)
        {
            string path = "/api/integrity";
            if (address != AVR.ServerManager.ServerMe?.address)
                path += "@" + address;
            AVR.ServerManager.GetOrFetch(server_address, server =>
            {
                if (server == null)
                {
                    callback(null);
                    return;
                }
                AVR.Downloader.FetchAsync<AVR.ServerIntegrity, AVR.ServerIntegrityInput>(path,
                    integrity =>
                    {
                        server.integrity = integrity;
                        callback(integrity);
                    },
                    server, "POST",
                    new AVR.ServerIntegrityInput { server = server_address }
                );
            });
        }
    }

    [Serializable]
    public class ServerGateway
    {
        public string http;
        public string ws;
    }

    [Serializable]
    public class ServerIntegrity
    {
        public string token;
        public string user;
    }

    [Serializable]
    public class ServerIntegrityInput
    {
        public string server;
    }
}