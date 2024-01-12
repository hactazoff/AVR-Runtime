using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
namespace AVR
{
    namespace SDK
    {
        public class PanelUpload : AVR.SDK.Plugin
        {

            [InitializeOnLoadMethod]
            static void Init()
            {
                AVR.SDK.Panel.Plugins.Add(new AVR.SDK.PanelUpload() { Title = "Upload" });
            }


            public AVR.UserMe user = null;
            private AVR.SDK.Panel Panel;

            private ProgressBar progress;
            private DropdownField platformList;
            private Button sumbit;
            private TextField version;
            private TextField world_id;

            public override VisualElement OnPanel(AVR.SDK.Panel panel)
            {
                Panel = panel;
                VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AVR/SDK/Editor/Plugins/PanelUpload/Uploader.uxml");
                VisualElement labelFromUXML = visualTree.Instantiate();
                progress = labelFromUXML.Q<ProgressBar>("puw_progress");
                sumbit = labelFromUXML.Q<Button>("puw_submit");
                version = labelFromUXML.Q<TextField>("puw_version");
                world_id = labelFromUXML.Q<TextField>("puw_id");
                platformList = labelFromUXML.Q<DropdownField>("puw_platform");

                sumbit.RegisterCallback<ClickEvent>((ClickEvent evt) => Upload());

                platformList.value = AVR.SDK.BuildAssetBundle.SupportedPlatforms.NoTarget.ToString();
                foreach (AVR.SDK.BuildAssetBundle.SupportedPlatforms platform in Enum.GetValues(typeof(AVR.SDK.BuildAssetBundle.SupportedPlatforms)))
                {
                    platformList.choices.Add(platform.ToString());
                    if (Panel.Config.platform_target == ((int)platform))
                        platformList.value = platform.ToString();
                }
                return labelFromUXML;
            }

            public bool IsUploading = false;

            public void Upload()
            {
                if (IsUploading)
                    return;
                IsUploading = true;
                sumbit.SetEnabled(false);
                progress.value = 0;
                progress.title = "Uploading...";

                string platform = platformList.value;
                if (platform == AVR.SDK.BuildAssetBundle.SupportedPlatforms.NoTarget.ToString())
                {
                    sumbit.SetEnabled(true);
                    progress.value = 0;
                    progress.title = "Select a platform";
                    IsUploading = false;
                    return;
                }

                if (world_id.value == "" || world_id.value == null)
                {
                    sumbit.SetEnabled(true);
                    progress.value = 0;
                    progress.title = "Select a world";
                    IsUploading = false;
                    return;
                }

                if (version.value == "" || version.value == null)
                {
                    sumbit.SetEnabled(true);
                    progress.value = 0;
                    progress.title = "Select a version";
                    IsUploading = false;
                    return;
                }

                var plugin_build = AVR.SDK.Panel.Plugins.Find((AVR.SDK.Plugin plugin) => plugin is AVR.SDK.PanelBuild) as AVR.SDK.PanelBuild;
                var plugin_profile = AVR.SDK.Panel.Plugins.Find((AVR.SDK.Plugin plugin) => plugin is AVR.SDK.PanelProfile) as AVR.SDK.PanelProfile;

                if (plugin_build.BuildScene == default || plugin_build.BuildScene == null)
                {
                    sumbit.SetEnabled(true);
                    progress.value = 0;
                    progress.title = "Select a scene";
                    IsUploading = false;
                    return;
                }

                string assetBundleDirectory = "Assets/BuildOutput/" + plugin_build.BuildScene.name.ToLower() + '.' + platform.ToString().ToLower() + ".avrw";

                if (!System.IO.File.Exists(assetBundleDirectory))
                {
                    sumbit.SetEnabled(true);
                    progress.value = 0;
                    progress.title = "Build the scene before upload";
                    IsUploading = false;
                    return;
                }

                string token = Panel.Config.token;
                string server = Panel.Config.server;

                if (token == "" || token == null || server == "" || server == null)
                {
                    sumbit.SetEnabled(true);
                    progress.value = 0;
                    progress.title = "Login before upload";
                    IsUploading = false;
                    return;
                }

                // upload assetbundle to the server at /api/world/asset 
                // and upload world with multipart/form-data

                string url = "http" + server + "/api/world/asset";

                WWWForm form = new WWWForm();
                form.AddField("version", version.value);
                form.AddField("world_id", world_id.value);
                form.AddField("platform", platform.ToString().ToLower());
                form.AddBinaryData("file", System.IO.File.ReadAllBytes(assetBundleDirectory), plugin_build.BuildScene.name.ToLower() + '.' + platform.ToString().ToLower() + ".avrw", "application/octet-stream");

                UnityWebRequest request = UnityWebRequest.Post(url, form);
                request.SetRequestHeader("User-Agent", "AVR");
                request.SetRequestHeader("Accept", "application/json");
                request.SetRequestHeader("Cookie", "");
                request.SetRequestHeader("Authorization", token);

                request.SendWebRequest();

                while (!request.isDone)
                {
                    progress.value = request.uploadProgress * 100;
                    progress.title = "Uploading... " + (request.uploadProgress * 100).ToString("0.00") + "%";
                }

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    sumbit.SetEnabled(true);
                    progress.value = 0;
                    progress.title = "Upload failed";
                    IsUploading = false;
                    return;
                }

                AVR.Response<string> response = JsonUtility.FromJson<AVR.Response<string>>(request.downloadHandler.text);
                if (response == null || response.data == null || response.error.message != null)
                {
                    sumbit.SetEnabled(true);
                    progress.value = 0;
                    progress.title = response.error.message;
                    IsUploading = false;
                    return;
                }


                progress.value = 100;
                progress.title = "Upload success";
                IsUploading = false;
                sumbit.SetEnabled(true);
                return;
            }
        }
    }
}