using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AVR
{
    namespace SDK
    {
        class BuildAssetBundle
        {
            public enum SupportedPlatforms
            {
                Mac = BuildTarget.StandaloneOSX,
                Linux = BuildTarget.StandaloneLinux64,
                Windows = BuildTarget.StandaloneWindows64,
                NoTarget = BuildTarget.NoTarget
            }

            public static SupportedPlatforms DetectBuildTarget()
            {
                return Application.platform switch
                {
                    RuntimePlatform.WindowsEditor => SupportedPlatforms.Windows,
                    RuntimePlatform.OSXEditor => SupportedPlatforms.Mac,
                    RuntimePlatform.LinuxEditor => SupportedPlatforms.Linux,
                    _ => SupportedPlatforms.NoTarget,
                };
            }

            public static bool BuildBundleAssetAvatar(UnityEngine.SceneManagement.Scene scene = default, AVR.SDK.AvatarDescriptor avatar = null, SupportedPlatforms buildTarget = SupportedPlatforms.NoTarget)
            {
                if (scene == default || avatar == null)
                {
                    AVR.Debug.LogError("Scene or Avatar is null");
                    return false;
                }

                if (buildTarget == SupportedPlatforms.NoTarget)
                    buildTarget = DetectBuildTarget();

                if (buildTarget == SupportedPlatforms.NoTarget)
                {
                    AVR.Debug.LogError("Build target is not supported");
                    return false;
                }

                string assetBundleDirectory = "Assets/BuildOutput/";
                if (!Directory.Exists(assetBundleDirectory))
                    Directory.CreateDirectory(assetBundleDirectory);

                // make prefab
                string prefabPath = assetBundleDirectory + avatar.name.ToLower() + ".prefab";
                if (File.Exists(prefabPath))
                    File.Delete(prefabPath);
                PrefabUtility.SaveAsPrefabAsset(avatar.gameObject, prefabPath);

                // get of prefab
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                // get dependencies of prefab
                string[] dependencies = AssetDatabase.GetDependencies(prefabPath);

                BuildAssetBundleOptions options = BuildAssetBundleOptions.None;

                AssetBundleBuild assetBundleDefinition = new();
                {
                    assetBundleDefinition.assetBundleName = avatar.name.ToLower() + '.' + buildTarget.ToString().ToLower() + ".avra";
                    assetBundleDefinition.assetNames = new string[] { prefabPath };
                    List<string> assets = new();
                    foreach (string dependency in dependencies)
                    {
                        if (assets.Contains(dependency)) continue;
                        if (Path.GetExtension(dependency) == ".cs") continue;
                        if (Path.GetExtension(dependency) == ".dll") continue;
                        if (Path.GetExtension(dependency) == ".meta") continue;
                        if (Path.GetExtension(dependency) == ".unity") continue;
                        assets.Add(dependency);
                    }
                    assetBundleDefinition.addressableNames = assets.ToArray();
                }

                BuildAssetBundlesParameters buildInput = new()
                {
                    outputPath = assetBundleDirectory,
                    targetPlatform = (BuildTarget)buildTarget,
                    options = options,
                    bundleDefinitions = new AssetBundleBuild[] { assetBundleDefinition }
                };

                AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(buildInput);

                if (manifest == null)
                {
                    AVR.Debug.LogError("Failed to build AssetBundles");
                    return false;
                }

                AVR.Debug.Log("AssetBundle built: " + avatar.name.ToLower());

                return true;
            }

            public static bool BuildBundleAssetWorld(UnityEngine.SceneManagement.Scene scene = default, AVR.SDK.WorldDescriptor world = null, SupportedPlatforms buildTarget = SupportedPlatforms.NoTarget)
            {
                if (scene == default || world == null)
                {
                    AVR.Debug.LogError("Scene or World is null");
                    return false;
                }

                if (buildTarget == SupportedPlatforms.NoTarget)
                    buildTarget = DetectBuildTarget();

                if (buildTarget == SupportedPlatforms.NoTarget)
                {
                    AVR.Debug.LogError("Build target is not supported");
                    return false;
                }

                string assetBundleDirectory = "Assets/BuildOutput/";
                if (!Directory.Exists(assetBundleDirectory))
                    Directory.CreateDirectory(assetBundleDirectory);


                // get dependencies
                string[] dependencies = AssetDatabase.GetDependencies(scene.path);


                BuildAssetBundleOptions options = BuildAssetBundleOptions.None;


                AssetBundleBuild assetBundleDefinition = new();
                {
                    assetBundleDefinition.assetBundleName = scene.name.ToLower() + '.' + buildTarget.ToString().ToLower() + ".avrw";
                    assetBundleDefinition.assetNames = new string[] { scene.path };
                    List<string> assets = new();
                    foreach (string dependency in dependencies)
                    {
                        if (assets.Contains(dependency)) continue;
                        if (Path.GetExtension(dependency) == ".cs") continue;
                        if (Path.GetExtension(dependency) == ".dll") continue;
                        if (Path.GetExtension(dependency) == ".meta") continue;
                        if (Path.GetExtension(dependency) == ".unity") continue;
                        assets.Add(dependency);
                    }
                    assetBundleDefinition.addressableNames = assets.ToArray();
                }

                BuildAssetBundlesParameters buildInput = new()
                {
                    outputPath = assetBundleDirectory,
                    targetPlatform = (BuildTarget)buildTarget,
                    options = options,
                    bundleDefinitions = new AssetBundleBuild[] { assetBundleDefinition }
                };

                AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(buildInput);

                if (manifest == null)
                {
                    AVR.Debug.LogError("Failed to build AssetBundles");
                    return false;
                }

                AVR.Debug.Log("AssetBundle built: " + scene.name.ToLower());
                // open folder at assetBundleDirectory
                EditorUtility.RevealInFinder(assetBundleDirectory + assetBundleDefinition.assetBundleName);
                return true;
            }
        }
    }
}