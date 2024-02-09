using AVR.Scenes;
using AVR.SDK.Worlds;
using AVR.Utils;
using Cysharp.Threading.Tasks;

namespace AVR.Worlds
{
    [System.Serializable]
    public class WorldAsset {
        public string id;
        public string version;
        public string url;
        public string hash;
        public string engine;
        public string platform;
        public int size;
        public bool empty;
        public string world_id;
        public string server;
        public World World => WorldManager.GetWorld(world_id + "@" + server);
        public Servers.Server Server => Servers.ServerManager.GetServer(server);

        public bool Downloaded => System.IO.File.Exists(CachePath);
		public string CachePath => AVR.Utils.Cache.CachePath + hash;
		
		public delegate void OnDownloadEvent(AVR.Worlds.DownloadAssetEvent progress);
		public event OnDownloadEvent OnDownload;

		public async UniTask<bool> Download() {
			if (Downloaded)
                return true;
           AVR.Network.HTTPRequest.OnProgress += OnProgress;
           var res = await AVR.Network.HTTPRequest.Download(url, CachePath);
           AVR.Network.HTTPRequest.OnProgress -= OnProgress;
           if(!res) return false;
           if (!AVR.Utils.Cache.ValidChecksum(CachePath, hash))
           {
	           System.IO.File.Delete(CachePath);
	           return false;
           }
           return true;
		}

		public void OnProgress(AVR.Network.Progress progress)
		{
			if (progress.request != url)
                return;
            OnDownload?.Invoke(new AVR.Worlds.DownloadAssetEvent()
            {
                asset_id = id,
                world_id = world_id,
                downloadprogress = progress.downloadprogress,
                uploadprogress = progress.uploadprogress
            });
		}
		
		public async UniTask<BundleScene> Enter()
		{
			if (!Downloaded && !await Download()) return null;
			AVR.Utils.Debug.Log("Entering world " + World.id);
			var e = await AVR.Scenes.SceneManager.LoadAsset(this);
			if(!e) return null;
			var w = AVR.Scenes.SceneManager.GetScene(world_id, id);
			if(w == null) return null;
			var spawn = w.Descriptor.ChoiseSpawn();
			AVR.Entity.Player.Instance.Teleport(spawn.transform);
			return w;
		}
		
		public override string ToString() => id+":"+world_id+":"+version+"("+platform+"-"+engine+")@"+server;
    }
}