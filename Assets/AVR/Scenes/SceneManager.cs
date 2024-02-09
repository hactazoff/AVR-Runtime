using Cysharp.Threading.Tasks;
using System.Collections;
using AVR.SDK.Base;
using AVR.SDK.Worlds;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AVR.Scenes
{
    public class SceneManager : AVR.Base.Manager<BundleScene>
    {
        public delegate void OnSceneLoadEvent(BundleScene scene);
        public static event OnSceneLoadEvent OnSceneLoad;
        public static void OnLoad(BundleScene scene) => OnSceneLoad?.Invoke(scene);
        
        public delegate void OnSceneUnloadEvent(BundleScene scene);
        public static event OnSceneUnloadEvent OnSceneUnload;
        public static void OnUnload(BundleScene scene) => OnSceneUnload?.Invoke(scene);
        
        
        public static async UniTask<bool> LoadAsset(AVR.Worlds.WorldAsset asset)
        {
            var request = await AssetBundle.LoadFromFileAsync(asset.CachePath);
            if (request == null)
            {
                AVR.Utils.Debug.Log("Bundle is null");
                return false;
            }
            if (!request.isStreamedSceneAssetBundle || request.GetAllScenePaths().Length == 0)
            {
                AVR.Utils.Debug.Log("Bundle is not a scene");
                request.Unload(true);
                return false;
            }
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(request.GetAllScenePaths()[0], UnityEngine.SceneManagement.LoadSceneMode.Single);
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(request.GetAllScenePaths()[0]);
            if (scene == null || !scene.IsValid())
            {
                AVR.Utils.Debug.Log("Scene is not valid");
                request.Unload(true);
                return false;
            }
            var bundleScene = new BundleScene()
            {
                world_id = asset.world_id,
                asset_id = asset.id,
                scene_path = scene.path
            };

            var descriptor = bundleScene.Descriptor;
            if (descriptor == null)
            {
                AVR.Utils.Debug.Log("Descriptor is null");
                request.Unload(true);
                return false;
            }
            Cache.Add(bundleScene);
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(bundleScene.scene_path, UnityEngine.SceneManagement.LoadSceneMode.Single);
            OnLoad(bundleScene);
            return true;
        }
        
        public static BundleScene GetScene(string world_id, string asset_id)
        {
            foreach (var scene in Cache)
                if (scene.world_id == world_id && scene.asset_id == asset_id)
                    return scene;
            return null;
        }
    }
}