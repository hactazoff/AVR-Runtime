using System;
using System.Collections.Generic;
using UnityEngine;

namespace AVR
{
    namespace Utils
    {
        public class ConfigManager : AVR.Base.Manager<int>
        {
            public static string ConfigPath(string profile) => Save.DefaultSavePath + profile + ".json";

            public static Config Open(string profile = null)
            {
                profile = profile != null ? profile.Trim() + ".config" : "config";
                string path = ConfigPath(profile);
                Config config = null;
                if (System.IO.File.Exists(path))
                {
                    var json = System.IO.File.ReadAllText(path);
                    config = JsonUtility.FromJson<Config>(json);
                    config.profile = profile;
                    config.path = path;
                    return config;
                }
                config = new() { profile = profile, path = path, created_at = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds };
                config.Save();
                return config;
            }
        }

        [System.Serializable]
        public class Config
        {
            public string profile = "default";
            public string path = "";
            public string[] mods_paths;
            public long created_at;
            public Dictionary<string, object> data = new();

            public void Save()
            {
                string path = ConfigManager.ConfigPath(profile);
                string json = JsonUtility.ToJson(this, true);
                System.IO.File.WriteAllText(path, json);
            }

            public T Get<T>(string value)
            {
                if (data.ContainsKey(value))
                    return (T)data[value];
                return default;
            }

            public T Set<T>(string value, T data)
            {
                if (this.data.ContainsKey(value))
                    this.data[value] = data;
                else this.data.Add(value, data);
                return data;
            }
        }
    }
}