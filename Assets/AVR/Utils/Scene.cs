using UnityEngine;

namespace AVR
{
    namespace Utils
    {
        public class Scene
        {
            public static UnityEngine.SceneManagement.Scene[] GetScenes()
            {
                var nbscenes = UnityEngine.SceneManagement.SceneManager.sceneCount;
                var scenes = new UnityEngine.SceneManagement.Scene[nbscenes + 1];
                for (int i = 0; i < nbscenes; i++)
                    scenes[i] = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                scenes[nbscenes] = GetSceneDontDestroyOnLoad();
                return scenes;
            }

            public static UnityEngine.SceneManagement.Scene GetSceneDontDestroyOnLoad()
            {
                return UnityEngine.SceneManagement.SceneManager.GetSceneByName("DontDestroyOnLoad");
            }
        }
    }
}