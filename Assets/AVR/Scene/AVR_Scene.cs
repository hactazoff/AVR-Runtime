using System.Collections;
using UnityEngine;

namespace AVR
{
public static class Scene
{
    public const string DefaultScene = "AVR.Lobby";

    public class LoadSceneEventArgs
    {
        public UnityEngine.SceneManagement.Scene scene;
        public string hash;
        public AVR.SDK.WorldDescriptor descriptor;
        public bool error;
    }

    // loadSync scene from asset bundle
    public delegate void OnLoadSceneSync(LoadSceneEventArgs scene);
    public static event OnLoadSceneSync onLoadSceneSync;
    public static void LoadSceneAsync(string hash, System.Action<UnityEngine.SceneManagement.Scene> callback) => AVR.Utils.GetStartup().StartCoroutine(LoadSceneCoroutine(hash, callback));
    private static IEnumerator LoadSceneCoroutine(string hash, System.Action<UnityEngine.SceneManagement.Scene> callback)
    {
        string path = AVR.Utils.LocalPathCache + '/' + hash;
        AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(path);
        yield return bundleRequest;
        AssetBundle bundle = bundleRequest.assetBundle;
        if (bundle == null)
        {
            onLoadSceneSync?.Invoke(new LoadSceneEventArgs() { scene = default, hash = hash, descriptor = null, error = true });
            callback(default);
            yield break;
        }


        string[] paths = bundle.GetAllScenePaths();
        if (paths.Length == 0)
        {
            bundle.Unload(true);
            onLoadSceneSync?.Invoke(new LoadSceneEventArgs() { scene = default, hash = hash, descriptor = null, error = true });
            callback(default);
            yield break;
        }
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(paths[0], UnityEngine.SceneManagement.LoadSceneMode.Single);
        yield return asyncOperation;
        UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(paths[0]);
        if (scene == null)
        {
            bundle.Unload(true);
            onLoadSceneSync?.Invoke(new LoadSceneEventArgs() { scene = default, hash = hash, descriptor = null, error = true });
            callback(default);
            yield break;
        }

        // verif if scene is valid (has AVR.WorldDescriptor)
        var worldDescriptor = AVR.Utils.GetComponent<AVR.SDK.WorldDescriptor>(scene);
        if (worldDescriptor == null)
        {
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
            yield return operation;
            bundle.Unload(true);
            onLoadSceneSync?.Invoke(new LoadSceneEventArgs() { scene = default, hash = hash, descriptor = worldDescriptor, error = true });
            callback(default);
            yield break;
        }

        // unload all other scenes
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
        {
            UnityEngine.SceneManagement.Scene s = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            if (s == scene)
                continue;
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(s);
            yield return operation;
        }

        AVR.Debug.Log("AVR.Scene LoadScene scene loaded: " + worldDescriptor.LocalId);
        onLoadSceneSync?.Invoke(new LoadSceneEventArgs() { scene = scene, hash = hash, descriptor = worldDescriptor, error = false });
        callback(scene);
    }

    public static void LoadDefaultSceneAsync(System.Action<UnityEngine.SceneManagement.Scene> callback) => AVR.Utils.GetStartup().StartCoroutine(LoadDefaultSceneAsyncCoroutine(callback));
    private static IEnumerator LoadDefaultSceneAsyncCoroutine( System.Action<UnityEngine.SceneManagement.Scene> callback)
    {
        string hash = AVR.Utils.Config.fallback_world;
        if (hash != null && hash != "")
        {
            LoadSceneAsync(hash, callback);
            yield break;
        }

        AsyncOperation loading = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(DefaultScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
        yield return loading;
        UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(DefaultScene);
        if (scene == null)
        {
            onLoadSceneSync?.Invoke(new LoadSceneEventArgs() { scene = default, hash = DefaultScene, descriptor = null, error = true });
            callback(default);
            yield break;
        }
        var worldDescriptor = AVR.Utils.GetComponent<AVR.SDK.WorldDescriptor>(scene);
        if (worldDescriptor == null)
        {
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
            yield return operation;
            onLoadSceneSync?.Invoke(new LoadSceneEventArgs() { scene = default, hash = DefaultScene, descriptor = worldDescriptor, error = true });
            callback(default);
            yield break;
        }

        // unload all other scenes
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
        {
            UnityEngine.SceneManagement.Scene s = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            if (s == scene)
                continue;
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(s);
            yield return operation;
        }

        onLoadSceneSync?.Invoke(new LoadSceneEventArgs() { scene = scene, hash = DefaultScene, descriptor = worldDescriptor, error = false });
        callback(scene);
    }
}
}