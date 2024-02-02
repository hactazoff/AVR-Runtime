using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVR
{
    namespace Utils
    {
        public class ConfigManager : Base.Manager<int>
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
                config = new()
                {
                    profile = profile,
                    path = path,
                    created_at = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds
                };
                config.Set("type", "development");
                config.Save();
                return config;
            }
        }

        [Serializable]
        public class Config
        {
            public string profile = "default";
            public string path = "";
            public string[] mods_paths = new string[] { };
            public long created_at;
            public string data = "";

            public void Save()
            {
                string path = ConfigManager.ConfigPath(profile);
                string json = JsonUtility.ToJson(this, true);
                System.IO.File.WriteAllText(path, json);
            }

            public T Get<T>(string key)
            {
                if (data == null || data == "") return default(T);
                var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(data));
                var jtoken = JToken.Parse(json);
                var value = jtoken[key];
                if (value == null) return default(T);
                return value.ToObject<T>();
            }

            public T Set<T>(string key, T value)
            {
                var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(data));
                var jtoken = JToken.Parse(json);
                jtoken[key] = JToken.FromObject(value);
                data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(jtoken.ToString()));
                return value;
            }
        }
    }
}