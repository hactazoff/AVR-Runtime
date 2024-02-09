#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Collections.Generic;
using AVR.SDK;

[InitializeOnLoad]
public class SDKBuildMod : AVR.SDK.Modding.SDKMod
{
    static SDKBuildMod() => Super(typeof(SDKBuildMod));
    
    public override void OnLoad()
    {
        Id = "avr.sdk.buildmod";
        Name = "Build Mod";
        if (!Directory.Exists(Path.Combine(Application.dataPath, "../Builds")))
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "../Builds"));
    }


    public SupportedPlatforms BuildTarget = AVR.SDK.Base.Builder.DetectBuildTarget();
    public List<string> Mods = new List<string>();
    public string OutputPath = Path.Combine(Application.dataPath, "../Builds");
    
    public Tab OnSDKTab()
    {
        var tab = new Tab(Name);
        tab.AddToClassList(Id.Replace(@".", "-"));
        var div = new VisualElement();
        div.AddToClassList("mods");
        var button = new Button(() => UpdateModList(div));
        button.text = "Update mod list";
        tab.Add(button);
        tab.Add(div);
        UpdateModList(div);
        var enumField = new EnumField("Build target", BuildTarget);
        enumField.RegisterValueChangedCallback((evt) =>
        {
            BuildTarget = (SupportedPlatforms)evt.newValue;
            Debug.Log("Build target changed to " + evt.newValue);
        });
        var button2 = new Button(() =>
        {
            BuildTarget = AVR.SDK.Base.Builder.DetectBuildTarget();
            enumField.SetValueWithoutNotify(BuildTarget);
            Debug.Log("Build target detected: " + BuildTarget);
        });
        button2.text = "Detect current build target";
        tab.Add(button2);
        tab.Add(enumField);
        var textField = new TextField("Output path");
        textField.RegisterValueChangedCallback((evt) =>
        {
            if (!Directory.Exists(evt.newValue))
                return;
            OutputPath = evt.newValue;
            Debug.Log("Output path changed to " + evt.newValue);
        });
        textField.SetValueWithoutNotify(OutputPath);
        tab.Add(textField);
        // button to open output path
        var button4 = new Button(() => UnityEditor.EditorUtility.RevealInFinder(OutputPath));
        button4.text = "Open output path";
        // button to select output path
        var button5 = new Button(() =>
        {
            var path = UnityEditor.EditorUtility.OpenFolderPanel("Select output path", OutputPath, "");
            if (path != "")
            {
                OutputPath = path;
                textField.SetValueWithoutNotify(OutputPath);
            }
        });
        button5.text = "Select output path";
        var div2 = new VisualElement();
        div2.style.flexDirection = FlexDirection.Row;
        div2.Add(button4);
        button4.style.flexGrow = 1;
        div2.Add(button5);
        button5.style.flexGrow = 1;
        tab.Add(div2);
        var button3 = new Button(() => Build());
        button3.text = "Build";
        tab.Add(button3);
        return tab;
    }
    
    public void UpdateModList(VisualElement div)
    {
        div.Clear();
        foreach (var mod in GetMods())
        {
            var toggle = new Toggle(mod);
            toggle.RegisterValueChangedCallback((evt) =>
            {
                if (evt.newValue) Mods.Add(mod);
                else Mods.Remove(mod);
            });
            toggle.SetValueWithoutNotify(Mods.Contains(mod));
            div.Add(toggle);
        }
    }
    
    public string[] GetMods()
    {
        List<string> mods = new List<string>();
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in assembly.GetTypes())
                if (type.IsSubclassOf(typeof(AVR.SDK.Modding.Mod)) 
                    && Directory.GetFiles("Assets", type.Namespace + ".asmdef", SearchOption.AllDirectories).Length == 1)
                        mods.Add(type.Namespace); 
        return mods.ToArray();
    }

    public void Build()
    {
        if (Mods.Count == 0)
        {
            UnityEditor.EditorUtility.DisplayDialog("Error", "No mods selected!", "Ok");
            return;
        }
        if (!Directory.Exists(OutputPath))
        {
            UnityEditor.EditorUtility.DisplayDialog("Error", "Output path does not exist!", "Ok");
            return;
        }
        foreach (var file in Directory.GetFiles(OutputPath))
            File.Delete(file);
        foreach (var dir in Directory.GetDirectories(OutputPath))
            Directory.Delete(dir, true);
        Debug.Log("Building...");
        Debug.Log("Build target: " + AVR.SDK.Base.Builder.ConvertToBuildTarget(BuildTarget));
        Debug.Log("Output path: " + OutputPath);
        var b = UnityEditor.BuildPipeline.BuildPlayer(
             new UnityEditor.EditorBuildSettingsScene[] { },
            Path.Combine(OutputPath, "build.exe"), 
            AVR.SDK.Base.Builder.ConvertToBuildTarget(BuildTarget),
            UnityEditor.BuildOptions.None
        );
        if (b.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            UnityEditor.EditorUtility.DisplayDialog("Error", "Build failed!", "Ok");
            return;
        }
        List<AssetBundleBuild> assetBundleDefinitionList = new();
        foreach (var mod in Mods)
        {
            var paths = Directory.GetFiles(OutputPath, mod + ".dll", SearchOption.AllDirectories);
            if (paths.Length == 0)
            {
                Debug.LogWarning("Mod " + mod + " not found!");
                continue;
            }
            File.Copy(paths[0], Path.Combine(OutputPath, mod.ToLower() + '-' + BuildTarget.ToString().ToLower() + ".dll"), true);
            var asmdef = Directory.GetFiles("Assets", mod + ".asmdef", SearchOption.AllDirectories);
            if (asmdef.Length == 0)
            {
                Debug.LogWarning("asmdef for mod " + mod + " not found!");
                continue;
            }
            var parent = Directory.GetParent(asmdef[0]).FullName;
            AssetBundleBuild assetBundleDefinition = new();
            { 
                assetBundleDefinition.assetBundleName = mod.ToLower() + '-' + BuildTarget.ToString().ToLower() + ".avrm";
                assetBundleDefinition.assetNames = RecursiveGetAllAssetsInDirectory(parent).ToArray();
                Debug.Log("Mod " + mod + " has " + assetBundleDefinition.assetNames.Length + " assets");
                Debug.Log(string.Join(" ", assetBundleDefinition.assetNames));

                List<string> assets = new();
                foreach (string dependency in assetBundleDefinition.assetNames)
                   foreach (var d in AssetDatabase.GetDependencies(dependency))
                       if (!assets.Contains(d)
                        && Path.GetExtension(d) != ".cs"
                            && Path.GetExtension(d) != ".dll"
                            && Path.GetExtension(d) != ".meta"
                            && Path.GetExtension(d) != ".unity")
                            assets.Add(d);
                assetBundleDefinition.addressableNames = assets.ToArray();
                Debug.Log("Mod " + mod + " has " + assetBundleDefinition.addressableNames.Length + " addressable assets");
                Debug.Log(string.Join(" ", assetBundleDefinition.addressableNames));
            }
            assetBundleDefinitionList.Add(assetBundleDefinition);
        }
        BuildAssetBundlesParameters buildInput = new()
        { 
            outputPath = OutputPath,
            targetPlatform = AVR.SDK.Base.Builder.ConvertToBuildTarget(BuildTarget),
            options = BuildAssetBundleOptions.None,
            bundleDefinitions = assetBundleDefinitionList.ToArray()
        };
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(buildInput);
        if (manifest == null)
            Debug.LogWarning("Failed to build AssetBundles");
        foreach (var file in Directory.GetFiles(OutputPath))
        {
            if (Path.GetExtension(file) == ".dll" && Mods.Contains(Path.GetFileNameWithoutExtension(file).Split('-')[0]))
                continue;
            if (Path.GetExtension(file) == ".avrm" && Mods.Contains(Path.GetFileNameWithoutExtension(file).Split('-')[0]))
                continue;
            File.Delete(file);
        }
        foreach (var dir in Directory.GetDirectories(OutputPath))
            Directory.Delete(dir, true);
        UnityEditor.EditorUtility.DisplayDialog("Success", "Build succeeded!", "Ok");
    }
    
    static List<string> RecursiveGetAllAssetsInDirectory(string path)
    {
        List<string> assets = new();
        foreach (var f in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            if (Path.GetExtension(f) != ".meta" &&
                Path.GetExtension(f) != ".cs" &&  // Scripts are not supported in AssetBundles
                Path.GetExtension(f) != ".dll" && // Assemblies are not supported in AssetBundles
                Path.GetExtension(f) != ".asmdef" && // Assembly definitions are not supported in AssetBundles
                Path.GetExtension(f) != ".unity") // Scenes cannot be mixed with other file types in a bundle
                assets.Add(Path.GetFullPath(f).Replace(Path.GetFullPath(Application.dataPath), "Assets"));
        return assets;
    }
}
#endif