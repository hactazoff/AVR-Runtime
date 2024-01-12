using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
namespace AVR
{
    public class Downloader
    {
        public delegate void OnDownloadAsset(bool success);
        public event OnDownloadAsset onDownloadAsset;

        public string url;
        public string hash;

        public void DownloadAsset(Action<bool> callback) => AVR.Utils.GetStartup().StartCoroutine(DownloadAssetCoroutine(callback));
        private IEnumerator DownloadAssetCoroutine(Action<bool> callback)
        {
            // has file in cache?
            string path = AVR.Utils.LocalPathCache + '/' + hash;
            if (System.IO.File.Exists(path))
            {
                AVR.Debug.Log("DownloadAssetCoroutine: " + path);
                onDownloadAsset?.Invoke(true);
                callback(true);
                yield break;
            }

            // download file
            var client = new System.Net.WebClient();
            AVR.Debug.Log("DownloadAssetCoroutine: " + url);
            client.DownloadFileAsync(new Uri(url), path);
            yield return null;
            onDownloadAsset?.Invoke(true);
            while (client.IsBusy)
                yield return null;
            client.Dispose();
            string Hash = AVR.Utils.GetHash(path);
            if (hash != Hash)
            {
                AVR.Debug.Log("DownloadAssetCoroutine: " + hash + " != " + Hash);
                System.IO.File.Delete(path);
                onDownloadAsset?.Invoke(false);
                callback(false);
            }
            else
            {
                onDownloadAsset?.Invoke(true);
                callback(true);
            }
        }

        public static void FetchAsync<T, R>(string url, Action<T> callback, AVR.Server server = null, string method = "GET", R body = default)
            => AVR.Utils.GetStartup().StartCoroutine(FetchAsyncCoroutine(url, callback, server, method, body));
        private static IEnumerator FetchAsyncCoroutine<T, R>(string url, Action<T> callback, AVR.Server server, string method, R body)
        {
            UnityWebRequest request;
            url = server.gateways.http + url;
            switch (method)
            {
                case "GET":
                    request = UnityWebRequest.Get(url);
                    break;
                case "POST":
                    if (body is string)
                        request = UnityWebRequest.Post(url, body as string, "text/plain");
                    else if (body is byte[])
                        request = UnityWebRequest.Post(url, (body as byte[]).ToString(), "application/octet-stream");
                    else if (body is object)
                        request = UnityWebRequest.Post(url, JsonUtility.ToJson(body), "application/json");
                    else request = UnityWebRequest.Post(url, "", "text/plain");
                    break;
                case "PUT":
                    if (body is string)
                    {
                        request = UnityWebRequest.Put(url, body as string); ;
                    }
                    else if (body is byte[])
                    {
                        request = UnityWebRequest.Put(url, (body as byte[]).ToString());
                        request.SetRequestHeader("Content-Type", "application/octet-stream");
                    }
                    else if (body is object)
                    {
                        request = UnityWebRequest.Put(url, JsonUtility.ToJson(body));
                        request.SetRequestHeader("Content-Type", "application/json");
                    }
                    else request = UnityWebRequest.Put(url, "");
                    break;
                case "DELETE":
                    request = UnityWebRequest.Delete(url);
                    break;
                default:
                    request = UnityWebRequest.Get(url);
                    break;
            };
            request.SetRequestHeader("User-Agent", "AVR/" + Application.version + " (Client;" + AVR.Utils.GetPlatfrom() + ")");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Cookie", "");
            if (server.address == AVR.ServerManager.ServerMe?.address || url.StartsWith(AVR.Utils.Config.server_gateway))
                request.SetRequestHeader("Authorization", AVR.Utils.Config.token ?? "");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                AVR.Debug.Log("Fetch: " + url + " " + request.downloadHandler.text);
                callback(default);
                yield break;
            }
            AVR.Response<T> response = JsonUtility.FromJson<AVR.Response<T>>(request.downloadHandler.text);
            if (response == null || response.data == null || response.error.message != null)
            {
                AVR.Debug.Log("Fetch: " + url + " " + request.downloadHandler.text);
                callback(default);
                yield break;
            }
            AVR.Debug.Log("Fetch: " + url + " " + request.downloadHandler.text);
            callback(response.data);
        }

        public static T Fetch<T>(string path, AVR.Server server)
        {
            server ??= AVR.ServerManager.ServerMe ?? new() { gateways = new() { http = "" } };
            UnityWebRequest request = UnityWebRequest.Get(server.gateways.http + path);
            request.SetRequestHeader("User-Agent", "AVR/" + Application.version + " (Client;" + AVR.Utils.GetPlatfrom() + ")");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Cookie", "");
            request.SetRequestHeader("Authorization", AVR.Utils.Config.token ?? "");
            request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                AVR.Debug.Log("Fetch: " + path + " " + request.downloadHandler.text);
                return default;
            }
            AVR.Response<T> response = JsonUtility.FromJson<AVR.Response<T>>(request.downloadHandler.text);
            if (response == null || response.data == null || response.error.message != null)
            {
                AVR.Debug.Log("Fetch: " + path + " " + request.downloadHandler.text);
                return default;
            }
            return response.data;
        }

        public static void GetBetterProtocolAsync(string host, Action<string> callback) => AVR.Utils.GetStartup()
            .StartCoroutine(GetBetterProtocolAsyncCoroutine(host, callback));
        private static IEnumerator GetBetterProtocolAsyncCoroutine(string host, Action<string> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get("https://" + host);
            request.SetRequestHeader("User-Agent", "AVR/" + Application.version + " (Client;" + AVR.Utils.GetPlatfrom() + ")"); ;
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                callback("https");
                yield break;
            }
        }
    }
}