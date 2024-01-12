using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace AVR
{
    [Serializable]
    public class WorldAsset
    {
        public string id;
        public string version;
        public string url;
        public string hash;
        public string engine;
        public int size;
        public string platform;

        public bool IsCompatible() => platform == AVR.Utils.GetPlatfrom() && engine == AVR.Utils.GetEngine();

        // Download asset
        public delegate void OnDownloadAsset(bool success);
        public event OnDownloadAsset onDownloadAsset;
        public void DownloadAssetAsync(Action<bool> callback) => AVR.Utils.GetStartup().StartCoroutine(DownloadAssetCoroutine(callback));
        private IEnumerator DownloadAssetCoroutine(Action<bool> callback)
        {
            AVR.Downloader downloader = new() { url = url, hash = hash };
            yield return null;
            downloader.DownloadAsset((success) =>
            {
                onDownloadAsset?.Invoke(success);
                callback(success);
            });
        }

    }
}