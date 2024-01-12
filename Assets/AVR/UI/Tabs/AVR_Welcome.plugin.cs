using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Plugin_Welcome : AVR.Plugin
{

    public override void Initialize(AVR.Startup startup)
    {
        Id = "avr-welcome";
        AVR.PluginManager.onMessage += OnMessage;

        // instanciate menu prefab

        base.Initialize(startup);
        // Button button = Menu.Q<Button>("avr-login-submit");
        // button.RegisterCallback<ClickEvent>((ClickEvent evt) => OnLogin(evt));
        // Widget.Q<Label>("avr-widget-name").text = "Logout";
    }

    private GameObject Menu = Resources.Load<GameObject>("avr-welcome");
    private GameObject WidgetContentLogout = Resources.Load<GameObject>("avr-widget-logout");
    private GameObject Widget = Resources.Load<GameObject>("avr-widget");

    private void OnLogin(ClickEvent evt)
    {
        // string server = Menu.Q<TextField>("avr-login-server").value;
        // string username = Menu.Q<TextField>("avr-login-username").value;
        // string password = Menu.Q<TextField>("avr-login-password").value;
        // new  AVR.Server() { address = server }.GetInfoAsync(( AVR.Server info) =>
        // {
        //     if (info == null)
        //     {
        //          AVR.Debug.Log("Failed to get server info");
        //         return;
        //     }
        //     new  AVR.Session().GetSessionLoginAsync(Startup, username, password, server, (session) => OnLoginComplete(session, info));
        // });
    }

    public void OnLoginComplete(AVR.Session session, AVR.Server server)
    {
        if (session == null)
        {
            AVR.Debug.Log("Login failed");
            return;
        }

        //  AVR.UserMe user = session.user;
        // user.GetUserMeAsync(( AVR.UserMe info) =>
        // {
        //     if (info == null)
        //     {
        //          AVR.Debug.Log("Failed to get user info");
        //         return;
        //     }
        //      AVR.Startup startup =  AVR.Utils.GetStartup();
        //      AVR.Utils.ConfigRaw config =  AVR.Utils.Config;
        //     config.token = session.token;
        //     config.server = server.address;
        //      AVR.Utils.Config = config;
        //      AVR.Scene.LoadDefaultSceneAsync(startup, (Scene scene) => { });
        // });
    }

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
                        { "for", (string)ev.data["for"] },
                        { "tab", Menu },
                        { "hide", true }
                    }
                });
        }
        else if (ev.evt == "avr.settab.ready")
        {
            var id = (string)ev.data["id"];
            if (id == Id)
            {
                Debug.Log("Welcome tab ready");
            }
        }
        else if (ev.evt == "avr.widget")
            AVR.PluginManager.Message(new()
            {
                evt = "avr.widget.response",
                plugin = this,
                data = new Dictionary<string, object>()
                {
                    { "id", Id + "-logout" },
                    { "by", (string)ev.data["id"] },
                    { "widget", Widget },
                    { "width", 1 },
                    { "height", 1 },
                    { "ui", (AVR.SDK.BaseUI)ev.data["ui"] }
                }
            });
        else if (ev.evt == "avr.widget.ready")
        {
            var id = (string)ev.data["id"];
            if (id == Id + "-logout")
            {
                var widget = AVR.Utils.FindDeep((GameObject)ev.data["widget"], "avr-widget-button");
                var content = GameObject.Instantiate(WidgetContentLogout, widget.transform);
            }
        }
    }
}
