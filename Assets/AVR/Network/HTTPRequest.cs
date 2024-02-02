using Cysharp.Threading.Tasks;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Networking;

namespace AVR
{
    namespace Network
    {
        public class HTTPRequest
        {

            public delegate void OnSuccess<T>(Response<T> response);
            public delegate void OnError<T>(Response<T> error);



            public async static UniTask<T> Get<T>(string url, UnityWebRequest request = null)
            {
                try
                {
                    request ??= UnityWebRequest.Get(url);
                    request.SetRequestHeader("Cookie", "");
                    request.SetRequestHeader("User-Agent", "AVR/" + Application.version + " (Unity/" + Application.unityVersion + "; " + Application.platform + ")");
                    await request.SendWebRequest();
                    AVR.Utils.Debug.Log(request.downloadHandler.text);
                    if (request.responseCode == 200)
                        return JsonUtility.FromJson<Response<T>>(request.downloadHandler.text).data;
                    return default;
                }
                catch { return default; }
            }

            public async static UniTask<string> FetchMostProtocol(string address)
            {
                UnityWebRequest request;
                try
                {
                    request = UnityWebRequest.Head("https://" + address + "/.well-known/avr");
                    await request.SendWebRequest();
                    if (request.responseCode == 200)
                        return "https";
                }
                catch { }
                try
                {
                    request = UnityWebRequest.Head("http://" + address + "/.well-known/avr");
                    await request.SendWebRequest();
                    if (request.responseCode == 200)
                        return "http";
                }
                catch { }
                return null;
            }
        }

        [System.Serializable]
        public class Progress
        {
            public string request;
            public float downloadprogress;
            public float uploadprogress;
        }

        [System.Serializable]
        public class Response<T>
        {
            public T data;
            public string request;
            public int time;
        }

        [System.Serializable]
        public class ResponseError
        {
            public string message;
            public int status;
            public int code;
        }
    }
}