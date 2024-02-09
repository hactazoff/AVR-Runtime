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
            public delegate void OnProgressEvent(Progress progress);
            public static event OnProgressEvent OnProgress;
            
            public async static UniTask<T> Get<T>(string url, UnityWebRequest request = null)
            {
                try
                {
                    request ??= UnityWebRequest.Get(url);
                    request.SetRequestHeader("Cookie", "");
                    request.SetRequestHeader("User-Agent", "AVR/" + Application.version + " (Unity/" + Application.unityVersion + "; " + Application.platform + ")");
                    var res = await request.SendWebRequest();
                    if (request.responseCode != 200)
                        return default;
                    while(!res.isDone)
                    {
                        AVR.Utils.Debug.Log("Downloading " + url + " " + request.downloadProgress + " " + request.uploadProgress);
                        OnProgress?.Invoke(new Progress()
                        {
                            request = url,
                            downloadprogress = request.downloadProgress,
                            uploadprogress = request.uploadProgress
                        });
                        await UniTask.Yield();
                    }
                    return JsonUtility.FromJson<Response<T>>(request.downloadHandler.text).data;
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
            
            public async static UniTask<bool> Download(string url, string path, UnityWebRequest request = null)
            {
                try
                {
                    request ??= UnityWebRequest.Get(url);
                    request.downloadHandler = new DownloadHandlerFile(path);
                    request.SetRequestHeader("Cookie", "");
                    request.SetRequestHeader("User-Agent", "AVR/" + Application.version + " (Unity/" + Application.unityVersion + "; " + Application.platform + ")");
                    var res = await request.SendWebRequest();
                    if (request.responseCode != 200)
                        return false;
                    while(!res.isDone)
                    {
                        AVR.Utils.Debug.Log("Downloading " + url + " " + request.downloadProgress + " " + request.uploadProgress);
                        OnProgress?.Invoke(new Progress()
                        {
                            request = url,
                            downloadprogress = request.downloadProgress,
                            uploadprogress = request.uploadProgress
                        });
                        await UniTask.Yield();
                    }
                    return true;
                }
                catch { return false; }
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