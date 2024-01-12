using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AVR
{
    [Serializable]
    public class World
    {
        public string title;
        public string description;
        public string release;
        public int capacity;
        public string id;
        public string server;
        public string version;
        public string[] tags;
        public AVR.WorldAsset[] assets;
        public AVR.WorldAsset[] Assets
        {
            get
            {
                List<AVR.WorldAsset> asst = new();
                foreach (AVR.WorldAsset ast in assets)
                    if (ast.IsCompatible()) asst.Add(ast);
                asst.Sort((a, b) => a.version.CompareTo(b.version));
                return asst.ToArray();
            }
        }

        // GET /api/world
        public delegate void OnWorldInfo(AVR.World info);
        public static event OnWorldInfo onWorldInfo;

        public void GetInfoAsync(Action<AVR.World> callback, AVR.Server Server = null)
        {
            Server ??= AVR.ServerManager.ServerMe;
            string path = "/api/worlds/" + id;
            if (server != Server?.address)
                path += "@" + server;
            AVR.Downloader.FetchAsync<AVR.World, int>(path, world => AVR.Utils.GetStartup().StartCoroutine(ParserInfoAsync(world, callback)), Server);
        }

        public IEnumerator ParserInfoAsync(AVR.World world, System.Action<AVR.World> callback)
        {
            if (world == null)
            {
                onWorldInfo?.Invoke(null);
                callback(null);
                yield break;
            }
            title = world.title;
            description = world.description;
            release = world.release;
            capacity = world.capacity;
            id = world.id;
            server = world.server;
            version = world.version;
            onWorldInfo?.Invoke(this);
            callback(this);
        }
    }
}