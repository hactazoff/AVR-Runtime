
using System.Collections.Generic;
using UnityEngine;

namespace AVR
{
public class Plugin
{
    public void Start() { }

    public string Id { get; set; } = "default";
    public AVR.Startup Startup { get; private set; }

    public virtual void Initialize(AVR.Startup startup)
    {
        Startup = startup;
        AVR.PluginManager.Initiate(this);
    }

    public virtual void Update() { }
}
}