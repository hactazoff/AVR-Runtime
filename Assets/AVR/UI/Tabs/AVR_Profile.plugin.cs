using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plugin_Profile : AVR.Plugin
{
    public override void Initialize(AVR.Startup startup)
    {
        Id = "avr-profile";
        AVR.PluginManager.onMessage += OnMessage;
        base.Initialize(startup);
    }

    private GameObject Widget = Resources.Load<GameObject>("avr-widget");
    private GameObject WidgetContentProfile = Resources.Load<GameObject>("avr-widget-profile");
    private GameObject Menu = Resources.Load<GameObject>("avr-profile");

    void OnMessage(AVR.PluginManager.OnMessageEvent ev)
    {
        if (ev.evt == "avr.settab")
        {
            string tab = (string)ev.data.GetValueOrDefault("id", null);
            AVR.User user = (AVR.User)ev.data.GetValueOrDefault("profile", null);
            if (tab != null && user != null)
            {
                AVR.PluginManager.Message(new()
                {
                    evt = "avr.settab.response",
                    plugin = this,
                    data = new Dictionary<string, object>() {
                        { "id", tab },
                        { "for", (string)ev.data["for"] },
                        { "tab", Menu },
                        { "hide", false },
                        { "state", user }
                    }
                });
            }
        }
        else if (ev.evt == "avr.widget")
            AVR.PluginManager.Message(new()
            {
                evt = "avr.widget.response",
                plugin = this,
                data = new Dictionary<string, object>()
                {
                    { "id", Id+"-own-profile" },
                    { "by", (string)ev.data["id"] },
                    { "widget", Widget },
                    { "width", 2 },
                    { "height", 1 },
                    { "ui", (AVR.SDK.BaseUI)ev.data["ui"] }
                }
            });
        else if (ev.evt == "avr.settab.ready")
        {
            var id = (string)ev.data["id"];
            if (id == Id + "-menu")
            {
                var user = (AVR.User)ev.data["state"];
                AVR.Utils.FindDeep((GameObject)ev.data["tab"], "avr-profile-display").GetComponent<Text>().text = user.display;
                AVR.Utils.FindDeep((GameObject)ev.data["tab"], "avr-profile-thumbnail").GetComponent<RawImage>().texture = user.Thumbnail;
                AVR.Utils.FindDeep((GameObject)ev.data["tab"], "avr-profile-banner").GetComponent<RawImage>().texture = user.Banner;
                AVR.Utils.FindDeep((GameObject)ev.data["tab"], "avr-profile-username").GetComponent<Text>().text = user.username;
                AVR.Utils.FindDeep((GameObject)ev.data["tab"], "avr-profile-id").GetComponent<Text>().text = user.id;
                AVR.Utils.FindDeep((GameObject)ev.data["tab"], "avr-profile-server").GetComponent<Text>().text = user.server;
            }
        }
        else if (ev.evt == "avr.widget.ready")
        {
            var id = (string)ev.data["id"];
            if (id == Id + "-own-profile")
            {
                var widget = AVR.Utils.FindDeep((GameObject)ev.data["widget"], "avr-widget-button");
                var content = GameObject.Instantiate(WidgetContentProfile, widget.transform);
                var UserMe = AVR.UserManager.UserMe;

                var Thumbnail = AVR.Utils.FindDeep(content, "avr-widget-profile-thumbnail").GetComponent<RawImage>();
                Thumbnail.texture = UserMe.Thumbnail;
                var Name = AVR.Utils.FindDeep(content, "avr-widget-profile-display").GetComponent<Text>();
                Name.text = UserMe.display;

                widget.GetComponent<Button>().onClick.AddListener(() => AVR.PluginManager.Message(new()
                {
                    evt = "avr.settab",
                    plugin = this,
                    data = new Dictionary<string, object>() {
                            { "id", Id+"-menu" },
                            { "profile", UserMe },
                            { "ui", (AVR.SDK.BaseUI)ev.data["ui"] },
                            { "for", id }
                        }
                }));
            }
        }
    }
}
