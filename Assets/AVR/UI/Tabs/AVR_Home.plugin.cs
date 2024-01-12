using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Plugin_Home : AVR.Plugin
{

    public override void Initialize(AVR.Startup startup)
    {
        Id = "avr-home";
        base.Initialize(startup);
        AVR.UserMe.onUserMe += OnUserMe;
        AVR.PluginManager.onMessage += OnMessage;
    }

    private List<GameObject> uis = new List<GameObject>();

    void OnUserMe(AVR.UserMe user)
    {
        foreach (GameObject ui in uis)
        {
            GameObject welcome = AVR.Utils.FindDeep(ui, "avr-home-title-welcome");
            if (welcome != null && welcome.TryGetComponent<Text>(out var debug))
                debug.text = user == null ? "Welcome!" : ("Welcome " + user.display + "!");
        }
    }

    public List<string> debugs = new List<string>() { "", "" };
    float lastTime = 0f;
    public override void Update()
    {
        if (Time.time - lastTime > 1)
        {
            lastTime = Time.time;
            debugs[0] = "FPS: " + (int)(1 / Time.deltaTime);
            foreach (GameObject ui in uis)
            {
                GameObject welcome = AVR.Utils.FindDeep(ui, "avr-home-debug-area");
                if (welcome != null && welcome.TryGetComponent<Text>(out var debug))
                    debug.text = string.Join("\n", debugs);
            }
        }
    }

    private GameObject Menu = Resources.Load<GameObject>("avr-home");

    void OnMessage(AVR.PluginManager.OnMessageEvent ev)
    {
        if (ev.evt == "avr.ping.response")
        {
            string address = (string)ev.data.GetValueOrDefault("address", null);
            if (address != null && AVR.ServerManager.ServerMe != null && AVR.ServerManager.ServerMe.address == address)
            {
                int ping = (int)ev.data.GetValueOrDefault("ping", -1);
                debugs[1] = "Ping: " + ping + "ms";
            }
        }
        else if (ev.evt == "avr.settab")
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
                        { "hide", false }
                    }
                });
        }
        else if (ev.evt == "avr.settab.ready")
        {
            var id = (string)ev.data["id"];
            if (id == Id)
            {
                OnUserMe(AVR.UserManager.UserMe);
                foreach (Transform child in AVR.Utils.FindDeep((GameObject)ev.data["tab"], "avr-home-widgets-grid").transform)
                    GameObject.Destroy(child.gameObject);
                AVR.PluginManager.Message(new()
                {
                    evt = "avr.widget",
                    plugin = this,
                    data = new Dictionary<string, object>() {
                        { "ui", (AVR.SDK.BaseUI)ev.data["ui"] },
                        { "id", Id }
                    }
                });

            }
        }
        else if (ev.evt == "avr.widget.response")
        {
            var id = (string)ev.data["by"];
            var menu = (AVR.SDK.BaseUI)ev.data["ui"];
            var widget = (GameObject)ev.data["widget"];
            if (id == Id)
            {
                var grid = AVR.Utils.FindDeep(menu.content, "avr-home-widgets-grid");
                if (widget != null && grid != null)
                {
                    var widget_instance = GameObject.Instantiate(widget);
                    var rect = widget_instance.GetComponent<AVR.SDK.RectSizeSplit>();
                    if (rect != null)
                    {
                        rect.spanx = (int)ev.data["width"];
                        rect.spany = (int)ev.data["height"];
                        Coodinates position = GetMostPositionWidget(widget, grid);
                        if (position != null)
                        {
                            widget_instance.transform.SetParent(grid.transform, false);
                            rect.index = position.y * rect.collumns + position.x;
                            rect.Split();
                            AVR.PluginManager.Message(new()
                            {
                                evt = "avr.widget.ready",
                                plugin = this,
                                data = new Dictionary<string, object>() {
                                    { "id", (string)ev.data["id"] },
                                    { "by", Id },
                                    { "widget", widget_instance },
                                    { "ui", menu }
                                }
                            });
                        }
                    }
                }
            }
        }
    }

    public class Coodinates
    {
        public int x = 0;
        public int y = 0;
    }

    public Coodinates dimentions_grid = new() { x = 6, y = 4 };

    public Coodinates GetMostPositionWidget(GameObject widget, GameObject grid)
    {
        for (int y = 0; y < dimentions_grid.y; y++)
            for (int x = 0; x < dimentions_grid.x; x++)
            {
                if (CanPlaceWidget(widget, grid, new Coodinates() { x = x, y = y }))
                    return new Coodinates() { x = x, y = y };
            }
        return null;
    }

    public bool CanPlaceWidget(GameObject widget, GameObject grid, Coodinates position)
    {
        int x = position.x;
        int y = position.y;
        int w = widget.GetComponent<AVR.SDK.RectSizeSplit>().spanx;
        int h = widget.GetComponent<AVR.SDK.RectSizeSplit>().spany;
        if (x + w > dimentions_grid.x || y + h > dimentions_grid.y)
            return false;
        for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++)
                if (IsPositionOccupied(grid, new Coodinates() { x = x + i, y = y + j }))
                    return false;
        return true;
    }

    public bool IsPositionOccupied(GameObject grid, Coodinates position)
    {
        if (position.x < 0 || position.y < 0)
            return true;
        if (position.x >= dimentions_grid.x || position.y >= dimentions_grid.y)
            return true;
        foreach (Transform game_object in grid.transform)
        {
            var posed_widget = game_object.gameObject;
            var rect = posed_widget.GetComponent<AVR.SDK.RectSizeSplit>();
            int index = rect.index;
            int collumns = rect.collumns;
            int x = index % collumns;
            int y = index / collumns;
            int w = rect.spanx;
            int h = rect.spany;
            if (position.x >= x && position.x < x + w && position.y >= y && position.y < y + h)
                return true;
        }
        return false;
    }
}