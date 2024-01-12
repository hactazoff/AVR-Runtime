using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Plugin_Settings : AVR.Plugin
{
    public override void Initialize(AVR.Startup startup)
    {
        Id = "avr-settings";
        AVR.PluginManager.onMessage += OnMessage;
        base.Initialize(startup);
        Widget.Q<Label>("avr-widget-name").text = "Settings";
        Widget.RegisterCallback<ClickEvent>((ClickEvent evt) =>
        {
            AVR.PluginManager.Message(new()
            {
                evt = "avr.settab",
                plugin = this,
                data = new Dictionary<string, object>() {
                    { "id", Id }
                }
            });
        });
    }

    private VisualElement Menu = Resources.Load<VisualTreeAsset>("avr.settings").CloneTree();
    private VisualElement Widget = Resources.Load<VisualTreeAsset>("avr.widget").CloneTree();

    void OnMessage(AVR.PluginManager.OnMessageEvent ev)
    {
        if (ev.evt == "avr.settab")
        {
            string tab = (string)ev.data["id"];
            if (tab != null && tab == Id)
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
