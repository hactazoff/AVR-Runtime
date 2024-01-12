using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace AVR
{
    namespace SDK
    {
        public class PanelBuild : AVR.SDK.Plugin
        {

            public Box content;

            [InitializeOnLoadMethod]
            static void Init()
            {
                AVR.SDK.Panel.Plugins.Add(new AVR.SDK.PanelBuild() { Title = "Build" });
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += (UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode) =>
                {
                    AVR.SDK.PanelBuild plugin = (AVR.SDK.PanelBuild)AVR.SDK.Panel.Plugins.Find(x => x.Title == "Build");
                    plugin?.UpdatePanel();
                };
                // inpector hirearchy change
                EditorApplication.hierarchyChanged += () =>
                {
                    AVR.SDK.PanelBuild plugin = (AVR.SDK.PanelBuild)AVR.SDK.Panel.Plugins.Find(x => x.Title == "Build");
                    plugin?.UpdatePanel();
                };
            }

            public AVR.SDK.Panel Panel;

            public override VisualElement OnPanel(AVR.SDK.Panel panel)
            {
                Panel = panel;
                VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AVR/SDK/Editor/Plugins/PanelBuild/UI.uxml");
                VisualElement labelFromUXML = visualTree.Instantiate();
                content = labelFromUXML.Q<Box>("pb_base");
                if (content != null)
                    UpdatePanel();
                return labelFromUXML;
            }

            public override void OnPanelOpen()
            {
                UpdatePanel();
            }

            public void UpdatePanel()
            {
                UpdateInfos();
                if (content == null) return;
                if (BuildScene != default && BuildWorld != null)
                {
                    VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AVR/SDK/Editor/Plugins/PanelBuild/World.uxml");
                    VisualElement labelFromUXML = visualTree.Instantiate();
                    labelFromUXML.Q<TextField>("pbw_name").value = BuildScene.name;
                    labelFromUXML.Q<TextField>("pbw_name").SetEnabled(false);
                    labelFromUXML.Q<TextField>("pbw_localid").value = BuildWorld.LocalId;
                    labelFromUXML.Q<TextField>("pbw_localid").RegisterCallback<ChangeEvent<string>>((evt) => BuildWorld.LocalId = evt.newValue);

                    var platformList = labelFromUXML.Q<DropdownField>("pbw_platform");
                    platformList.value = AVR.SDK.BuildAssetBundle.SupportedPlatforms.NoTarget.ToString();
                    foreach (AVR.SDK.BuildAssetBundle.SupportedPlatforms platform in Enum.GetValues(typeof(AVR.SDK.BuildAssetBundle.SupportedPlatforms)))
                    {
                        platformList.choices.Add(platform.ToString());
                        if (Panel.Config.platform_target == ((int)platform))
                            platformList.value = platform.ToString();
                    }
                    platformList.RegisterCallback<ChangeEvent<string>>((evt) =>
                    {
                        Panel.Config.platform_target = (int)Enum.Parse(typeof(AVR.SDK.BuildAssetBundle.SupportedPlatforms), evt.newValue);
                        Panel.SaveConfig(Panel.Config);
                    });
                    // button select GameObject in inspector 
                    labelFromUXML.Q<Button>("pbw_select").clicked += () =>
                    {
                        Selection.activeObject = BuildWorld.gameObject;
                        EditorGUIUtility.PingObject(BuildWorld.gameObject);
                    };
                    // button detect build target
                    labelFromUXML.Q<Button>("pbw_detect").clicked += () =>
                    {
                        var platform = AVR.SDK.BuildAssetBundle.DetectBuildTarget();
                        Panel.Config.platform_target = (int)platform;
                        platformList.value = platform.ToString();
                    };

                    // button build
                    var buttonBuild = labelFromUXML.Q<Button>("pbw_build");
                    buttonBuild.clicked += () =>
                    {
                        buttonBuild.SetEnabled(false);
                        // save scene
                        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(BuildScene);
                        AVR.SDK.BuildAssetBundle.BuildBundleAssetWorld(BuildScene, BuildWorld, (AVR.SDK.BuildAssetBundle.SupportedPlatforms)Panel.Config.platform_target);
                        buttonBuild.SetEnabled(true);
                    };

                    content.Clear();
                    content.Add(labelFromUXML);
                }
                else if (BuildAvatar != default && BuildAvatar != null)
                {
                    VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AVR/SDK/Editor/Plugins/PanelBuild/Avatar.uxml");
                    VisualElement labelFromUXML = visualTree.Instantiate();
                    labelFromUXML.Q<TextField>("pba_name").value = BuildAvatar.name;
                    labelFromUXML.Q<TextField>("pba_name").SetEnabled(false);

                    var platformList = labelFromUXML.Q<DropdownField>("pba_platform");
                    platformList.value = AVR.SDK.BuildAssetBundle.SupportedPlatforms.NoTarget.ToString();
                    foreach (AVR.SDK.BuildAssetBundle.SupportedPlatforms platform in Enum.GetValues(typeof(AVR.SDK.BuildAssetBundle.SupportedPlatforms)))
                    {
                        platformList.choices.Add(platform.ToString());
                        if (Panel.Config.platform_target == ((int)platform))
                            platformList.value = platform.ToString();
                    }
                    platformList.RegisterCallback<ChangeEvent<string>>((evt) =>
                    {
                        Panel.Config.platform_target = (int)Enum.Parse(typeof(AVR.SDK.BuildAssetBundle.SupportedPlatforms), evt.newValue);
                        Panel.SaveConfig(Panel.Config);
                    });
                    // button detect build target
                    labelFromUXML.Q<Button>("pba_detect").clicked += () =>
                    {
                        var platform = AVR.SDK.BuildAssetBundle.DetectBuildTarget();
                        Panel.Config.platform_target = (int)platform;
                        platformList.value = platform.ToString();
                    };
                    // button select GameObject in inspector 
                    labelFromUXML.Q<Button>("pba_select").clicked += () =>
                    {
                        Selection.activeObject = BuildAvatar.gameObject;
                        EditorGUIUtility.PingObject(BuildAvatar.gameObject);
                    };
                    // button build
                    var buttonBuild = labelFromUXML.Q<Button>("pba_build");
                    buttonBuild.clicked += () =>
                    {
                        buttonBuild.SetEnabled(false);
                        AVR.SDK.BuildAssetBundle.BuildBundleAssetAvatar(BuildScene, BuildAvatar, (AVR.SDK.BuildAssetBundle.SupportedPlatforms)Panel.Config.platform_target);
                        buttonBuild.SetEnabled(true);
                    };

                    content.Clear();
                    content.Add(labelFromUXML);
                }
                else
                {
                    VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AVR/SDK/Editor/Plugins/PanelBuild/Empty.uxml");
                    VisualElement labelFromUXML = visualTree.Instantiate();
                    content.Clear();
                    content.Add(labelFromUXML);
                }
            }



            public UnityEngine.SceneManagement.Scene BuildScene = default;
            public AVR.SDK.WorldDescriptor BuildWorld = null;
            public AVR.SDK.AvatarDescriptor BuildAvatar = null;

            public void UpdateInfos()
            {
                UnityEngine.SceneManagement.Scene Scenefound = default;
                AVR.SDK.WorldDescriptor Worldfound = null;
                AVR.SDK.AvatarDescriptor Avatarfound = null;
                for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
                {
                    var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                    var worldDescriptor = AVR.Utils.GetComponent<AVR.SDK.WorldDescriptor>(scene);
                    if (worldDescriptor != null && worldDescriptor != default)
                    {
                        Scenefound = scene;
                        Worldfound = worldDescriptor;
                        break;
                    }
                    var avatarDescriptor = AVR.Utils.GetComponent<AVR.SDK.AvatarDescriptor>(scene);
                    if (avatarDescriptor != null && avatarDescriptor != default)
                    {
                        Scenefound = scene;
                        Avatarfound = avatarDescriptor;
                        break;
                    }
                }
                BuildScene = Scenefound;
                BuildWorld = Worldfound;
                BuildAvatar = Avatarfound;
            }
        }
    }
}