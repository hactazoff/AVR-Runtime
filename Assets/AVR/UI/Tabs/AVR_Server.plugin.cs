using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Plugin_Server : AVR.Plugin
{
    public override void Initialize(AVR.Startup startup)
    {
        Id = "avr-server";
        AVR.PluginManager.onMessage += OnMessage;
        base.Initialize(startup);
        Widget.Q<Label>("avr-widget-name").text = "My Server";

        Widget.RegisterCallback<ClickEvent>((ClickEvent evt) =>
        {
            if (AVR.ServerManager.ServerMe != null)
                AVR.PluginManager.Message(new()
                {
                    evt = "avr.settab",
                    plugin = this,
                    data = new Dictionary<string, object>() {
                        { "id", Id },
                        { "server",  AVR.ServerManager.ServerMe}
                    }
                });
        });
    }

    private VisualElement Menu = Resources.Load<VisualTreeAsset>("avr.server").CloneTree();
    private VisualElement Widget = Resources.Load<VisualTreeAsset>("avr.widget").CloneTree();

    void OnMessage(AVR.PluginManager.OnMessageEvent ev)
    {
        if (ev.evt == "avr.settab")
        {
            string tab = (string)ev.data.GetValueOrDefault("id", null);
            AVR.Server server = (AVR.Server)ev.data.GetValueOrDefault("server", null);
            if (tab != null && tab == Id && server != null)
            {
                Menu.Q<Label>("avr-st").text = server.title ?? server.id;
                Menu.Q<Label>("avr-sid").text = "ID: " + server.id;
                Menu.Q<Label>("avr-sv").text = "Version: " + server.version;
                Menu.Q<Label>("avr-sip").text = "IP: " + server.address;
                Menu.Q<Image>("avr-si").image = server.Icon;

                AVR.PluginManager.Message(new()
                {
                    evt = "avr.settab.response",
                    plugin = this,
                    data = new Dictionary<string, object>() {
                        { "id", tab },
                        { "tab", Menu }
                    }
                });
            }
        }
        // else if (ev.evt == "avr.widget")
        //      AVR.PluginManager.Message(new()
        //     {
        //         evt = "avr.widget.response",
        //         plugin = this,
        //         data = new Dictionary<string, object>() {
        //                 { "widget", Widget }
        //             }
        //     });
    }
}

