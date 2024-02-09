using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using System.IO;

namespace AVR
{
    namespace Modding
    {
        class Manager : Base.Manager<SDK.Modding.Mod>
        {
            /**
             * <summary>Get all mods.</summary>
             * <returns>Mods.</returns>
             */
            public static string[] GetModFolderPaths()
            {
                var list = new List<string>() { "Assets/AVR/Modding/Mods", Path.Combine(Utils.Save.DefaultSavePath, "mods") };
                foreach (var path in Utils.ConfigManager.Open().mods_paths)
                    if (!list.Contains(path)) list.Add(path);
                foreach (var path in list.ToArray())
                    if (!Directory.Exists(path)) list.Remove(path);
                return list.ToArray();
            }

            public static string[] GetModPaths()
            {
                var list = new List<string>();
                foreach (var path in GetModFolderPaths())
                    foreach (var file in Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
                        list.Add(file);
                return list.ToArray();
            }

            public static SDK.Modding.Mod[] GetMods()
            {
                var list = new List<SDK.Modding.Mod>();
                Utils.Debug.Log("Loading " + GetModPaths().Length + " mods...");
                foreach (var path in GetModPaths())
                    LoadAssembly(path);
                return list.ToArray();
            }
            
            public static SDK.Modding.Mod GetMod(string id)
            {
                foreach (var mod in Cache)
                    if (mod.Id == id) return mod;
                return null;
            }

            public static SDK.Modding.Mod LoadAssembly(string path)
            {
                if (!File.Exists(path) || Path.GetExtension(path) != ".dll") {
                    Utils.Debug.LogError("Mod at path " + path + " does not exist or is not a dll file.");
                    return null;
                }
                var name = Path.GetFileNameWithoutExtension(path);
                Utils.Debug.Log("Loading mod " + name + "...");
                var assembly = System.Reflection.Assembly.LoadFile(path);
                if (assembly == null) {
                    Utils.Debug.LogError("Error loading assembly at path " + path);
                    return null;
                }
                var types = assembly.GetTypes();
                SDK.Modding.Mod mod = null;
                foreach (var type in types)
                    if (type.BaseType == typeof(SDK.Modding.Mod) && type.IsClass)
                    {
                        foreach (var m in Cache)
                            if (m.GetType() == type) return null;
                        mod = (SDK.Modding.Mod)assembly.CreateInstance(type.FullName);
                    }
                if (mod == null) {
                    Utils.Debug.LogError("Mod at path " + path + " does not contain a class that inherits from SDK.Modding.Mod.");
                    return null;
                }
                mod.Setup(assembly, name, path);
                mod.OnLoad();
                Cache.Add(mod);
                return mod;
            }

            public static AVR.SDK.Modding.Mod[] LoadedMods()
            {
                var li = new List<AVR.SDK.Modding.Mod>();
                foreach (var mod in Cache)
                    if (mod.loaded)
                        li.Add(mod);
                return li.ToArray();
            }
        }
    }
}