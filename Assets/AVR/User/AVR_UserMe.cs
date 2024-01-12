
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace AVR
{
    [System.Serializable]
    public class UserMe : AVR.User
    {
        public string home;
        public string token;

        // GET /api/me
        public delegate void OnUserMe(AVR.UserMe info);
        public static event OnUserMe onUserMe;

        public void GetUserMeAsync(Action<AVR.UserMe> callback, AVR.Server Server = null)
        {
            string path = "/api/users/@me";
            AVR.Downloader.FetchAsync<AVR.UserMe, int>(path, user => AVR.Utils.GetStartup().StartCoroutine(ParserUserMeAsync(user, callback)), Server ?? AVR.ServerManager.ServerMe);
        }

        private IEnumerator ParserUserMeAsync(AVR.UserMe user, Action<AVR.UserMe> callback)
        {
            if (user == null)
            {
                onUserMe?.Invoke(null);
                callback(null);
                yield break;
            }
            id = user.id;
            username = user.username;
            display = user.display;
            thumbnail = user.thumbnail;
            server = user.server;
            created_at = user.created_at;
            home = user.home;
            if (thumbnail != null)
            {
                UnityWebRequest thumbnailRequest = UnityWebRequestTexture.GetTexture(thumbnail);
                yield return thumbnailRequest.SendWebRequest();
                if (thumbnailRequest.result == UnityWebRequest.Result.ConnectionError || thumbnailRequest.result == UnityWebRequest.Result.ProtocolError)
                    AVR.Debug.Log("Error: " + thumbnailRequest.error);
                else Thumbnail = ((DownloadHandlerTexture)thumbnailRequest.downloadHandler).texture;
            }
            AVR.UserManager.UserMe = this;
            onUserMe?.Invoke(this);
            callback(this);
        }

        public AVR.UserMe GetUserMe(AVR.Server Server = null)
        {
            string path = "/api/users/@me";
            if (server != AVR.ServerManager.ServerMe?.address)
                path += "@" + server;
            var user = AVR.Downloader.Fetch<AVR.UserMe>(path, Server ?? AVR.ServerManager.ServerMe);
            if (user == null)
                return null;
            id = user.id;
            username = user.username;
            display = user.display;
            thumbnail = user.thumbnail;
            server = user.server;
            created_at = user.created_at;
            home = user.home;
            if (thumbnail != null)
            {
                UnityWebRequest thumbnailRequest = UnityWebRequestTexture.GetTexture(thumbnail);
                thumbnailRequest.SendWebRequest();
                if (thumbnailRequest.result == UnityWebRequest.Result.ConnectionError || thumbnailRequest.result == UnityWebRequest.Result.ProtocolError)
                    AVR.Debug.Log("Error: " + thumbnailRequest.error);
                else Thumbnail = ((DownloadHandlerTexture)thumbnailRequest.downloadHandler).texture;
            }
            AVR.UserManager.SetUser(this);
            onUserMe?.Invoke(this);
            return this;
        }

        public void GetHomeWorldAsync(Action<AVR.World> callback)
        {
            AVR.Downloader.FetchAsync<AVR.World, int>("/api/users/@me/home", world =>
            {
                if (world != null)
                    AVR.Utils.GetStartup().StartCoroutine(world.ParserInfoAsync(world, callback));
                else callback(null);
            }, Server);
        }

        public void GetIntegrityAsync(string server_address, Action<AVR.ServerIntegrity> callback)
        {

        }
    }

}