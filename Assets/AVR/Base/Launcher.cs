using UnityEngine;
using UnityEngine.SceneManagement;

namespace AVR
{
    namespace Base
    {
        public class Launcher : MonoBehaviour
        {
            public static Launcher Current { get; private set; } = null;
            public string Type { get; set; } = "unknown";
            public static Launcher GetCurrent()
            {
                if (Current != null) 
                    return Current;
                foreach (var obj in AVR.Utils.Scene.GetSceneDontDestroyOnLoad().GetRootGameObjects())
                    if (obj.GetComponent<Launcher>() != null)
                        Current = obj.GetComponent<Launcher>();
                return Current;
            }

            public void Super()
            {
                Utils.Debug.Log("Loading mods...");
                DontDestroyOnLoad(gameObject);
                SDK.Modding.Mod[] mods = Modding.Manager.GetMods();
                foreach (SDK.Modding.Mod mod in mods)
                    Utils.Debug.Log("Mod detected: " + mod.Name);

                Utils.Debug.Log("Loading scenes...");
            }
        }
    }
}