using UnityEngine.SceneManagement;

namespace AVR.Scenes
{
    public class BundleScene
    {
        public string world_id;
        public string asset_id;
        public string scene_path;
        public AVR.SDK.Worlds.WorldDescriptor Descriptor 
        {
            get
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(scene_path);
                foreach (var go in scene.GetRootGameObjects())
                {
                    AVR.Utils.Debug.Log("Checking " + go.name);
                    var descriptor = go.GetComponent<AVR.SDK.Worlds.WorldDescriptor>();
                    if (descriptor != null)
                        return descriptor;
                }
                return null;
            }
        }
    }
};

