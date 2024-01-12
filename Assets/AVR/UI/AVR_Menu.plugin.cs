using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Plugin_Menu : AVR.Plugin
{
    public override void Initialize(AVR.Startup startup)
    {
        Id = "avr-menu";
        UpdateUIDocuments();
        AVR.PluginManager.onMessage += OnMessage;
        AVR.UserMe.onUserMe += OnUserMe;
        AVR.Scene.onLoadSceneSync += OnLoadScene;
        base.Initialize(startup);
    }

    void OnLoadScene(AVR.Scene.LoadSceneEventArgs ev)
    {
        UpdateUIDocuments();
    }

    void OnUserMe(AVR.UserMe user)
    {
    }

    public AVR.SDK.BaseUI[] GetUIDocuments()
    {
        List<AVR.SDK.BaseUI> documents = new();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Debug.Log("Scene " + i + " " + SceneManager.GetSceneAt(i).name);
            var uis = AVR.Utils.GetComponents<AVR.SDK.BaseUI>(SceneManager.GetSceneAt(i));
            foreach (var ui in uis)
                documents.Add(ui);
        }

        Scene scene = AVR.Utils.GetDontDestroyOnLoadScene();
        var uis2 = AVR.Utils.GetComponents<AVR.SDK.BaseUI>(scene);
        foreach (var ui in uis2)
            documents.Add(ui);

        Debug.Log("Found " + documents.Count + " documents");
        return documents.ToArray();
    }

    public bool init = false;
    void OnMessage(AVR.PluginManager.OnMessageEvent ev)
    {
        if (ev.evt == "avr.settab.response")
        {
            if (!init)
            {
                init = true;
                LoadEvents();
            }
            AVR.Debug.Log("Set tab response " + ev.data["id"] + " from " + (ev.plugin?.ToString() ?? "Guess") + ".");
            foreach (AVR.SDK.BaseUI document in GetUIDocuments())
            {
                Debug.Log(document);
                // have RectTransform
                GameObject tab = document.content;
                if (tab != null)
                    tab.transform.DetachChildren();
                GameObject content = GameObject.Instantiate((GameObject)ev.data["tab"]);
                content.name = ev.data["id"] + "-content";
                if (content != null)
                    content.transform.SetParent(tab.transform, false);
                document.SetActiveNav((bool)ev.data.GetValueOrDefault("hide", false));
                AVR.PluginManager.Message(new()
                {
                    evt = "avr.settab.ready",
                    plugin = this,
                    data = new Dictionary<string, object>() {
                        { "id", ev.data["id"] },
                        { "for", (string)ev.data["for"] },
                        { "ui", document },
                        { "tab", content },
                        { "state", ev.data.GetValueOrDefault("state", null) }
                    }
                });
            }
        }
        else if (ev.evt == "avr.settab")
            AVR.Debug.Log("Request to set tab " + ev.data["id"] + " from " + (ev.plugin?.ToString() ?? "Guess") + ".");
    }

    void UpdateUIDocuments()
    {
        var documents = GetUIDocuments();
        foreach (var document in documents)
            if (!document.ready && document != null)
            {
                document.ready = true;

            }
        AVR.Debug.Log("Menu added to " + documents.Length + " documents");

    }

    void LoadEvents()
    {

    }
}