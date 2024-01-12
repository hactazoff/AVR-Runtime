using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

namespace AVR
{
    namespace SDK
    {
        public class Panel : EditorWindow
        {
            public static AVR.SDK.Panel Window = null;
            [MenuItem("AVR/Open Panel")]
            public static void Init()
            {
                if (Window == null)
                    Window = (AVR.SDK.Panel)EditorWindow.GetWindow(typeof(AVR.SDK.Panel), false, "AtelierVR SDK");
                Window.Show();
            }

            void OnResize()
            {
                EditorApplication.delayCall += () =>
                {
                    var body = rootVisualElement.Q<Box>("body");
                    var root = rootVisualElement.Q<Box>("root");
                    var sum = 0f;
                    foreach (var child in root.Children())
                        sum += child.resolvedStyle.height;
                    sum = rootVisualElement.resolvedStyle.height - sum + body.resolvedStyle.height;
                    body.style.height = sum;
                };
            }

            private void CreateGUI()
            {
                AVR.Debug.Log("Config path: " + ConfigPath);
                LoadGUI();
                rootVisualElement.RegisterCallback<GeometryChangedEvent>(e => OnResize());
                EditorApplication.delayCall += () => OnResize();
            }

            [MenuItem("AVR/Reload SDK")]
            public static void ReloadSDK() => AssetDatabase.Refresh();

            public void LoadGUI()
            {
                rootVisualElement.Clear();
                var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AVR/SDK/Editor/AVRSDK_Panel.uxml");
                VisualElement labelFromUXML = visualTree.Instantiate();
                rootVisualElement.Add(labelFromUXML);
                var button = rootVisualElement.Q<Button>("reload");
                button.clickable.clicked += () =>
                {
                    ReloadSDK();
                    LoadGUI();
                };
                Box box = rootVisualElement.Q<Box>("buttons");
                Box body = rootVisualElement.Q<Box>("body");
                // make button and in fist position
                foreach (var plugin in Plugins)
                {
                    var buttonPlugin = new Button();
                    buttonPlugin.text = plugin.Title;
                    buttonPlugin.AddToClassList("button");
                    buttonPlugin.clickable.clicked += () =>
                    {
                        foreach (var child in body.Children())
                            if (child.ClassListContains(plugin.Title))
                                child.style.display = DisplayStyle.Flex;
                            else child.style.display = DisplayStyle.None;
                    };
                    VisualElement panel = plugin.OnPanel(this);
                    if (panel != null)
                    {
                        panel.AddToClassList("panel");
                        panel.AddToClassList(plugin.Title);
                        panel.style.display = DisplayStyle.None;
                        body.Add(panel);
                    }
                    box.Insert(0, buttonPlugin);
                }
                EditorApplication.delayCall += () => OnResize();
            }
            public static List<AVR.SDK.Plugin> Plugins = new();

            public string ConfigPath => AVR.Utils.LocalPath + "/" + Application.productName.ToLower().Replace(" ", "_").Normalize() + ".config.json";

            private ConfigJSON ConfigCache = null;

            public ConfigJSON Config => ConfigCache ??= LoadConfig();

            public ConfigJSON LoadConfig()
            {
                if (!System.IO.File.Exists(ConfigPath))
                    return new ConfigJSON();
                return ConfigCache = JsonUtility.FromJson<ConfigJSON>(System.IO.File.ReadAllText(ConfigPath));
            }

            public void SaveConfig(ConfigJSON config) => System.IO.File.WriteAllText(ConfigPath, JsonUtility.ToJson(ConfigCache = config));

            [System.Serializable]
            public class ConfigJSON
            {
                public string server = "";
                public string token = "";
                public int platform_target = 0;
            }
        }
    }
}