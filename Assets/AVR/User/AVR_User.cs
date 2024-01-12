using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace AVR
{
    [Serializable]
    public class User
    {
        public string id;
        public string username;
        public string display;
        public string thumbnail;
        public string server;
        public string banner;
        public AVR.Server Server { get => AVR.ServerManager.GetServerByAddress(server); }
        public Texture2D Thumbnail = null;
        public Texture2D Banner = null;
        public Int64 created_at;

        // GET /api/user
        public delegate void OnUserInfo(AVR.User info);
        public static event OnUserInfo onUserInfo;

        public void GetUserAsync(Action<AVR.User> callback, AVR.Server Server = null)
        {
            Server ??= AVR.ServerManager.ServerMe;
            string path = "/api/users/" + id;
            if (server != Server?.address)
                path += "@" + server;
            AVR.Downloader.FetchAsync<AVR.User, int>(path, user => AVR.Utils.GetStartup().StartCoroutine(ParserUserAsync(user, callback)), Server);
        }

        private IEnumerator ParserUserAsync(AVR.User user, Action<AVR.User> callback)
        {
            if (user == null)
            {
                onUserInfo?.Invoke(null);
                callback(null);
                yield break;
            }
            id = user.id;
            username = user.username;
            display = user.display;
            thumbnail = user.thumbnail;
            server = user.server;
            created_at = user.created_at;
            banner = user.banner;
            if (thumbnail != null)
            {
                UnityWebRequest thumbnailRequest = UnityWebRequestTexture.GetTexture(thumbnail);
                yield return thumbnailRequest.SendWebRequest();
                if (thumbnailRequest.result == UnityWebRequest.Result.ConnectionError || thumbnailRequest.result == UnityWebRequest.Result.ProtocolError)
                    AVR.Debug.Log("Error: " + thumbnailRequest.error);
                else Thumbnail = ((DownloadHandlerTexture)thumbnailRequest.downloadHandler).texture;
            }
            if (banner != null)
            {
                UnityWebRequest bannerRequest = UnityWebRequestTexture.GetTexture(banner);
                yield return bannerRequest.SendWebRequest();
                if (bannerRequest.result == UnityWebRequest.Result.ConnectionError || bannerRequest.result == UnityWebRequest.Result.ProtocolError)
                    AVR.Debug.Log("Error: " + bannerRequest.error);
                else Banner = ((DownloadHandlerTexture)bannerRequest.downloadHandler).texture;
            }
            AVR.UserManager.SetUser(this);
            onUserInfo?.Invoke(this);
            callback(this);
        }
    }
}