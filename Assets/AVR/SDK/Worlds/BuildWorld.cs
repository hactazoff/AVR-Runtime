#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace AVR.SDK.Worlds
{
    public class BuildWorld : AVR.SDK.Base.Builder
    {
        public static bool Build(WorldDescriptor descriptor, Scene scene)
        {
            if(descriptor.BuildTarget == SupportedPlatforms.NoTarget)
                descriptor.BuildTarget = DetectBuildTarget();
            if (descriptor.BuildTarget == SupportedPlatforms.NoTarget)
            {
                EditorUtility.DisplayDialog("Error", "Unsupported platform", "OK");
                return false;
            }
            
            string assetBundleDirectory = "Assets/BuildOutput/";
            if (!Directory.Exists(assetBundleDirectory)) 
                Directory.CreateDirectory(assetBundleDirectory);
            string[] dependencies = AssetDatabase.GetDependencies(scene.path);
            BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
            
            AssetBundleBuild assetBundleDefinition = new();
            { 
                assetBundleDefinition.assetBundleName = scene.name.ToLower() + '.' + descriptor.BuildTarget.ToString().ToLower() + ".avrw";
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
                targetPlatform = (BuildTarget)descriptor.BuildTarget,
                options = options,
                bundleDefinitions = new AssetBundleBuild[] { assetBundleDefinition }
            };

            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(buildInput);

            if (manifest == null)
            {
                EditorUtility.DisplayDialog("Error", "Failed to build AssetBundle", "OK");
                return false;
            }

            EditorUtility.DisplayDialog("AVR Build", "AssetBundle built successfully", "OK");
            EditorUtility.RevealInFinder(assetBundleDirectory + assetBundleDefinition.assetBundleName);
            return true;
        }
    }
}
#endif