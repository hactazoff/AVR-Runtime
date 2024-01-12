using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AVR
{
    public static class Utils
    {
        public static string GetPlatfrom() => Application.platform switch
        {
            RuntimePlatform.WebGLPlayer => "web",
            RuntimePlatform.Android => "android",
            RuntimePlatform.IPhonePlayer => "ios",
            RuntimePlatform.LinuxEditor => "linux",
            RuntimePlatform.LinuxPlayer => "linux",
            RuntimePlatform.OSXEditor => "mac",
            RuntimePlatform.OSXPlayer => "mac",
            RuntimePlatform.WindowsEditor => "windows",
            RuntimePlatform.WindowsPlayer => "windows",
            _ => "unknown",
        };

        public static string GetHash256(string input)
        {
            System.Security.Cryptography.SHA256Managed crypt = new();
            System.Text.StringBuilder hash = new();
            byte[] crypto = crypt.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input), 0, System.Text.Encoding.UTF8.GetByteCount(input));
            foreach (byte theByte in crypto)
                hash.Append(theByte.ToString("x2"));
            return hash.ToString();
        }

        public static string GetHash(string path)
        {
            System.Security.Cryptography.SHA256 crypt = System.Security.Cryptography.SHA256.Create();
            byte[] crypto = crypt.ComputeHash(System.IO.File.ReadAllBytes(path));
            string hash = "";
            foreach (byte b in crypto)
                hash += b.ToString("x2");
            return hash;
        }

        public static T GetComponent<T>(UnityEngine.SceneManagement.Scene scene)
        {
            foreach (GameObject RO in scene.GetRootGameObjects())
            {
                T component = RO.GetComponent<T>();
                if (component != null)
                    return component;
            }
            return default;
        }

        public static T[] GetComponents<T>(UnityEngine.SceneManagement.Scene scene)
        {
            // find all components in scene
            List<T> result = new List<T>();
            foreach (GameObject RO in scene.GetRootGameObjects())
            {
                T[] components = RO.GetComponentsInChildren<T>();
                foreach (T component in components)
                    result.Add(component);
            }
            return result.ToArray();
        }

        public static string LocalPath
        {
            get
            {
                string path = Application.persistentDataPath;
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string LocalPathCache
        {
            get
            {
                string path = LocalPath + "/cache";
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string LocalPathTmp
        {
            get
            {
                string path = LocalPath + "/tmp";
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                return path;
            }
        }

        public enum LocomotionTurnType
        {
            Continuous = 1,
            Snap = 2,
            None = 0
        }

        public enum LocomotionMoveType
        {
            Continuous = 1,
            Teleport = 2,
            None = 0
        }

        [Serializable]
        public class Locomotion
        {
            public LocomotionTurnType turn = LocomotionTurnType.Snap;
            public LocomotionMoveType move = LocomotionMoveType.Teleport;

            public float turn_snap_dead_zone = 0.75f;
            public float turn_snap_angle = 45f;
            public float turn_snap_timeout = 0.5f;
            public float turn_continuous_speed = 60f;
            public float turn_continuous_dead_zone_min = 0.125f;
            public float turn_continuous_dead_zone_max = 0.925f;
            public float move_continuous_speed_max = 3f;
            public float move_continuous_dead_zone_min = 0.125f;
            public float move_continuous_dead_zone_max = 0.925f;
        }

        [Serializable]
        public class ConfigRaw
        {
            public string server_gateway;
            public string token;
            public string fallback_world;
            public Locomotion locomotion;
        }
        private static string _profile;
        public static string Profile
        {
            get
            {
                if (_profile != null)
                    return _profile;
                string[] args = Environment.GetCommandLineArgs();
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-avr-profile" && i + 1 < args.Length)
                        return _profile = args[i + 1];
                }
                return _profile ?? "";
            }
        }
        public static string ConfigPath
        {
            get
            {
                string path = LocalPath + $"/{(Profile == "" ? "" : (Profile + "."))}config.json";
                return path;
            }
        }

        public static ConfigRaw Config
        {
            get
            {
                if (!File.Exists(ConfigPath))
                    return new ConfigRaw();
                string json = File.ReadAllText(ConfigPath);
                return JsonUtility.FromJson<ConfigRaw>(json);
            }
            set => File.WriteAllText(ConfigPath, JsonUtility.ToJson(value));
        }

        public static UnityEngine.SceneManagement.Scene GetDontDestroyOnLoadScene()
        {
            UnityEngine.SceneManagement.Scene dontDestroyOnLoad = default;
            GameObject temp = null;
            try
            {
                temp = new GameObject();
                UnityEngine.Object.DontDestroyOnLoad(temp);
                dontDestroyOnLoad = temp.scene;
                UnityEngine.Object.DestroyImmediate(temp);
                temp = null;
            }
            finally
            {
                if (temp != null)
                    UnityEngine.Object.DestroyImmediate(temp);
            }
            return dontDestroyOnLoad;
        }

        public static AVR.Startup GetStartup() => GetComponent<AVR.Startup>(GetDontDestroyOnLoadScene());

        public static string GetEngine() => "unity";

        public static GameObject FindDeep(GameObject parent, string name)
        {
            Transform[] transforms = parent.GetComponentsInChildren<Transform>(true);
            foreach (Transform transform in transforms)
                if (transform.name == name)
                    return transform.gameObject;
            return null;
        }

        public static string GetGameObjectPath(GameObject obj, bool includeScene = true)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            if (includeScene)
                path = obj.scene.name + path;
            return path;
        }
    }
}