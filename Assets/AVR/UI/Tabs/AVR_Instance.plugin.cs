using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Plugin_Instance : AVR.Plugin
{
    public override void Initialize(AVR.Startup startup)
    {
        Id = "avr-instance";
        AVR.PluginManager.onMessage += OnMessage;
        base.Initialize(startup);
    }

    private VisualElement Menu = Resources.Load<VisualTreeAsset>("avr.server").CloneTree();

    void OnMessage(AVR.PluginManager.OnMessageEvent ev)
    {
    }
}

