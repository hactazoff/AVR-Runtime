using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace AVR
{
    [Serializable]
    public class Session
    {
        public string id;
        public string token;
        public Int64 created_at;
        public AVR.UserMe user;

        // POST /api/login
        public delegate void OnSessionLogin(AVR.Session info);
        public static event OnSessionLogin onSessionLogin;
        public void GetSessionLoginAsync(MonoBehaviour context, string identity, string password, string server, Action<AVR.Session> callback) => context.StartCoroutine(GetSessionLoginCoroutine(identity, password, server, callback));
        private IEnumerator GetSessionLoginCoroutine(string identity, string password, string server, Action<AVR.Session> callback)
        {
            string url = "http" + server + "/api/login";
            string passhash = AVR.Utils.GetHash256(password);
            string json = "{\"identity\":\"" + identity + "\",\"password\":\"" + passhash + "\"}";
            AVR.Debug.Log(json);
            UnityWebRequest request = UnityWebRequest.Post(url, json, "application/json");
            AVR.Debug.Log("LOGIN RESPONSE: " + request.downloadHandler.text);
            request.SetRequestHeader("User-Agent", "AVR");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Cookie", "");
            request.SetRequestHeader("Authorization", "");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                onSessionLogin?.Invoke(null);
                callback(null);
                yield break;
            }
            AVR.Response<AVR.Session> response = JsonUtility.FromJson<AVR.Response<AVR.Session>>(request.downloadHandler.text);
            if (response == null || response.data == null || response.error.message != null)
            {
                onSessionLogin?.Invoke(null);
                callback(null);
                yield break;
            }
            id = response.data.id;
            token = response.data.token;
            created_at = response.data.created_at;
            user = response.data.user;
            user.token = response.data.token;
            onSessionLogin?.Invoke(this);
            callback(this);
        }

        public AVR.Session GetSessionLogin(string identity, string password, string server)
        {
            string url = "http" + server + "/api/login";
            string passhash = AVR.Utils.GetHash256(password);
            string json = "{\"identity\":\"" + identity + "\",\"password\":\"" + passhash + "\"}";
            UnityWebRequest request = UnityWebRequest.Post(url, json, "application/json");
            request.SetRequestHeader("User-Agent", "AVR");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Cookie", "");
            request.SetRequestHeader("Authorization", "");
            request.SendWebRequest();
            while (!request.isDone) { }
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                onSessionLogin?.Invoke(null);
                return null;
            }
            AVR.Response<AVR.Session> response = JsonUtility.FromJson<AVR.Response<AVR.Session>>(request.downloadHandler.text);
            if (response == null || response.data == null || response.error.message != null)
            {
                onSessionLogin?.Invoke(null);
                return null;
            }
            id = response.data.id;
            token = response.data.token;
            created_at = response.data.created_at;
            user = response.data.user;
            user.token = response.data.token;
            onSessionLogin?.Invoke(this);
            return this;
        }

        // POST /api/register
        public delegate void OnSessionRegister(AVR.Session info);
        public static event OnSessionRegister onSessionRegister;
        public void GetSessionRegisterAsync(MonoBehaviour context, string identity, string password, string display, string server, Action<AVR.Session> callback) => context.StartCoroutine(GetSessionRegisterCoroutine(identity, password, display, server, callback));
        private IEnumerator GetSessionRegisterCoroutine(string identity, string password, string display, string server, Action<AVR.Session> callback)
        {
            string url = "http" + server + "/api/register";
            string passhash = AVR.Utils.GetHash256(password);
            string json = "{\"identity\":\"" + identity + "\",\"password\":\"" + passhash + "\",\"display\":\"" + display + "\"}";
            UnityWebRequest request = UnityWebRequest.Post(url, json, "application/json");
            request.SetRequestHeader("User-Agent", "AVR");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Cookie", "");
            request.SetRequestHeader("Authorization", "");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                onSessionRegister?.Invoke(null);
                callback(null);
                yield break;
            }
            AVR.Response<AVR.Session> response = JsonUtility.FromJson<AVR.Response<AVR.Session>>(request.downloadHandler.text);
            if (response == null || response.data == null || response.error.message != null)
            {
                onSessionRegister?.Invoke(null);
                callback(null);
                yield break;
            }
            id = response.data.id;
            token = response.data.token;
            created_at = response.data.created_at;
            user = response.data.user;
            user.token = response.data.token;
            onSessionRegister?.Invoke(this);
            callback(this);
        }
        public AVR.Session GetSessionRegister(string identity, string password, string display, string server)
        {
            string url = "http" + server + "/api/register";
            string passhash = AVR.Utils.GetHash256(password);
            string json = "{\"identity\":\"" + identity + "\",\"password\":\"" + passhash + "\",\"display\":\"" + display + "\"}";
            UnityWebRequest request = UnityWebRequest.Post(url, json, "application/json");
            request.SetRequestHeader("User-Agent", "AVR");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Cookie", "");
            request.SetRequestHeader("Authorization", "");
            request.SendWebRequest();
            while (!request.isDone) { }
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                onSessionRegister?.Invoke(null);
                return null;
            }
            AVR.Response<AVR.Session> response = JsonUtility.FromJson<AVR.Response<AVR.Session>>(request.downloadHandler.text);
            if (response == null || response.data == null || response.error.message != null)
            {
                onSessionRegister?.Invoke(null);
                return null;
            }
            id = response.data.id;
            token = response.data.token;
            created_at = response.data.created_at;
            user = response.data.user;
            user.token = response.data.token;
            onSessionRegister?.Invoke(this);
            return this;
        }

        // GET /api/logout
        public delegate void OnSessionLogout(AVR.Session info);
        public static event OnSessionLogout onSessionLogout;
        public void GetSessionLogoutAsync(MonoBehaviour context, string server, Action<AVR.Session> callback) => context.StartCoroutine(GetSessionLogoutCoroutine(server, callback));
        private IEnumerator GetSessionLogoutCoroutine(string server, Action<AVR.Session> callback)
        {
            string url = "http" + server + "/api/logout";
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("User-Agent", "AVR");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Cookie", "");
            request.SetRequestHeader("Authorization", token);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                onSessionLogout?.Invoke(null);
                callback(null);
                yield break;
            }
            AVR.Response<string> response = JsonUtility.FromJson<AVR.Response<string>>(request.downloadHandler.text);
            if (response == null || response.data == null || response.error.message != null)
            {
                onSessionLogout?.Invoke(null);
                callback(null);
                yield break;
            }
            id = "LOGOUT";
            token = null;
            created_at = 0;
            user = null;
            onSessionLogout?.Invoke(this);
            callback(this);
        }
        public AVR.Session GetSessionLogout(string server)
        {
            string url = "http" + server + "/api/logout";
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("User-Agent", "AVR");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Cookie", "");
            request.SetRequestHeader("Authorization", token);
            request.SendWebRequest();
            while (!request.isDone) { }
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                onSessionLogout?.Invoke(null);
                return null;
            }
            AVR.Response<string> response = JsonUtility.FromJson<AVR.Response<string>>(request.downloadHandler.text);
            if (response == null || response.data == null || response.error.message != null)
            {
                onSessionLogout?.Invoke(null);
                return null;
            }
            id = "LOGOUT";
            token = null;
            created_at = 0;
            user = null;
            onSessionLogout?.Invoke(this);
            return this;
        }

        // DELETE /api/logout
        public delegate void OnSessionDelete(AVR.Session info);
        public static event OnSessionDelete onSessionDelete;
        public void GetSessionDeleteAsync(MonoBehaviour context, string server, Action<AVR.Session> callback) => context.StartCoroutine(GetSessionDeleteCoroutine(server, callback));
        private IEnumerator GetSessionDeleteCoroutine(string server, Action<AVR.Session> callback)
        {
            string url = "http" + server + "/api/logout";
            UnityWebRequest request = UnityWebRequest.Delete(url);
            request.SetRequestHeader("User-Agent", "AVR");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Cookie", "");
            request.SetRequestHeader("Authorization", token);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                onSessionDelete?.Invoke(null);
                callback(null);
                yield break;
            }
            AVR.Response<string> response = JsonUtility.FromJson<AVR.Response<string>>(request.downloadHandler.text);
            if (response == null || response.data == null || response.error.message != null)
            {
                onSessionDelete?.Invoke(null);
                callback(null);
                yield break;
            }
            id = "DELETED";
            token = null;
            created_at = 0;
            user = null;
            onSessionDelete?.Invoke(this);
            callback(this);
        }
        public AVR.Session GetSessionDelete(string server)
        {
            string url = "http" + server + "/api/logout";
            UnityWebRequest request = UnityWebRequest.Delete(url);
            request.SetRequestHeader("User-Agent", "AVR");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Cookie", "");
            request.SetRequestHeader("Authorization", token);
            request.SendWebRequest();
            while (!request.isDone) { }
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                onSessionDelete?.Invoke(null);
                return null;
            }
            AVR.Response<string> response = JsonUtility.FromJson<AVR.Response<string>>(request.downloadHandler.text);
            if (response == null || response.data == null || response.error.message != null)
            {
                onSessionDelete?.Invoke(null);
                return null;
            }
            id = "DELETED";
            token = null;
            created_at = 0;
            user = null;
            onSessionDelete?.Invoke(this);
            return this;
        }
    }
}