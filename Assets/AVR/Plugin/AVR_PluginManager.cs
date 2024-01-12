using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
namespace AVR
{
    public static class PluginManager
    {
        public static List<AVR.Plugin> Plugins { get; private set; } = new();

        public delegate void OnInitiate(AVR.Plugin plugin);
        public static event OnInitiate onInitiate;
        public static void Initiate(AVR.Plugin plugin)
        {
            Plugins.Add(plugin);
            onInitiate?.Invoke(plugin);
        }

        public delegate void OnLoad(AVR.Plugin plugin);
        public static event OnLoad onLoad;
        public static void Load(AVR.Plugin plugin) => onLoad?.Invoke(plugin);

        public delegate void OnUnload(AVR.Plugin plugin);
        public static event OnUnload onUnload;
        public static void Unload(AVR.Plugin plugin) => onUnload?.Invoke(plugin);

        public class OnMessageEvent
        {
            public AVR.Plugin plugin;
            public string evt;
            public Dictionary<string, object> data;
        }

        public delegate void OnMessage(OnMessageEvent evt);
        public static event OnMessage onMessage;
        public static void Message(OnMessageEvent evt) => onMessage?.Invoke(evt);

        public static AVR.Plugin GetPlugin(string id)
        {
            foreach (AVR.Plugin plugin in Plugins)
                if (plugin.Id == id)
                    return plugin;
            return null;
        }

        public static AVR.Plugin[] FindAllPlugins()
        {
            List<AVR.Plugin> plugins = new List<AVR.Plugin>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
                foreach (Type type in assembly.GetTypes())
                    if (type.IsSubclassOf(typeof(AVR.Plugin)))
                        plugins.Add(Activator.CreateInstance(type) as AVR.Plugin);
            foreach (AVR.Plugin plugin in plugins)
                plugin.Start();
            return plugins.ToArray();
        }

        public static void Start(AVR.Startup startup)
        {
            foreach (AVR.Plugin plugin in FindAllPlugins())
                plugin.Initialize(startup);
        }

        public static void Update()
        {
            foreach (AVR.Plugin plugin in Plugins)
                plugin.Update();
        }

        public static T GetPlugin<T>() where T : AVR.Plugin
        {
            foreach (AVR.Plugin plugin in Plugins)
                if (plugin is T)
                    return plugin as T;
            return null;
        }
    }
}