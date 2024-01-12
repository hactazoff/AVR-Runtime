using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace AVR
{
    namespace SDK
    {
        public class PanelProfile : AVR.SDK.Plugin
        {
            public Button loginButton;
            public Button logoutButton;
            public TextField username;
            public TextField password;
            public TextField server;
            public TextField token;
            public Label error;
            public Box form;

            [InitializeOnLoadMethod]
            static void Init()
            {
                AVR.SDK.Panel.Plugins.Add(new AVR.SDK.PanelProfile() { Title = "Profile" });
            }


            public AVR.UserMe user = null;
            private AVR.SDK.Panel Panel;

            public override VisualElement OnPanel(AVR.SDK.Panel panel)
            {
                Panel = panel;
                if (panel.Config.token != null && panel.Config.token.Length > 0 && panel.Config.server != null && panel.Config.server.Length > 0)
                {
                    user = new AVR.UserMe() { token = panel.Config.token, server = panel.Config.server }.GetUserMe();
                    if (user == null)
                    {
                        panel.Config.token = "";
                        panel.Config.server = "";
                        panel.SaveConfig(panel.Config);
                    }
                }
                return UpdatePanel();
            }

            public VisualElement UpdatePanel() => user == null ? LoginPanel() : ProfilePanel();

            public VisualElement ProfilePanel()
            {
                VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AVR/SDK/Editor/Plugins/PanelProfile/Logged.uxml");
                VisualElement labelFromUXML = visualTree.Instantiate();

                logoutButton = labelFromUXML.Q<Button>("ppl_logout");

                labelFromUXML.Q<TextField>("ppi_username").SetValueWithoutNotify(user.username);
                labelFromUXML.Q<TextField>("ppi_display").SetValueWithoutNotify(user.display);
                labelFromUXML.Q<TextField>("ppi_server").SetValueWithoutNotify(user.server);
                labelFromUXML.Q<TextField>("ppi_username").SetEnabled(false);
                labelFromUXML.Q<TextField>("ppi_display").SetEnabled(false);
                labelFromUXML.Q<TextField>("ppi_server").SetEnabled(false);

                logoutButton.clicked += () =>
                {
                    user = null;
                    logoutButton.SetEnabled(false);
                    AVR.Session session = new AVR.Session() { token = Panel.Config.token }.GetSessionLogout(Panel.Config.server);
                    if (session.id == "LOGOUT")
                    {
                        Panel.Config.token = "";
                        Panel.Config.server = "";
                        Panel.SaveConfig(Panel.Config);
                        Panel.LoadGUI();
                    }
                    else logoutButton.SetEnabled(true);
                };

                return labelFromUXML;
            }

            public VisualElement LoginPanel()
            {

                VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/AVR/SDK/Editor/Plugins/PanelProfile/Login.uxml");
                VisualElement labelFromUXML = visualTree.Instantiate();

                loginButton = labelFromUXML.Q<Button>("ppl_login");
                username = labelFromUXML.Q<TextField>("ppl_username");
                password = labelFromUXML.Q<TextField>("ppl_password");
                password.isPasswordField = true;
                server = labelFromUXML.Q<TextField>("ppl_server");
                token = labelFromUXML.Q<TextField>("ppl_token");
                form = labelFromUXML.Q<Box>("ppl_form");
                error = labelFromUXML.Q<Label>("ppl_error");

                loginButton.clicked += OnLogin;
                return labelFromUXML;
            }

            public void OnLogin()
            {
                username.SetEnabled(false);
                password.SetEnabled(false);
                server.SetEnabled(false);
                loginButton.SetEnabled(false);
                token.SetEnabled(false);
                error.text = "Loading...";

                // if valid server with regex (startwith :// or s:// and the next char is not /)
                if (!new System.Text.RegularExpressions.Regex(@"^(://|s://)[^/]").IsMatch(server.value))
                {
                    username.SetEnabled(true);
                    password.SetEnabled(true);
                    server.SetEnabled(true);
                    loginButton.SetEnabled(true);
                    token.SetEnabled(true);
                    error.text = "Invalid server address";
                    return;
                }

                AVR.Debug.Log(":" + token.value + ":");

                // login with token
                if (token.value.Length > 0)
                {
                    error.text = "Logging in (with token)...";
                    var userMe = new AVR.UserMe() { token = token.value, server = server.value }.GetUserMe();
                    if (user == null)
                    {
                        error.text = "Impossible to login with token.";
                        username.SetEnabled(true);
                        password.SetEnabled(true);
                        server.SetEnabled(true);
                        loginButton.SetEnabled(true);
                        token.SetEnabled(true);
                        return;
                    }
                    else
                    {
                        error.text = user.username + " logged in.";
                        username.SetEnabled(true);
                        password.SetEnabled(true);
                        server.SetEnabled(true);
                        loginButton.SetEnabled(true);
                        token.SetEnabled(true);
                        user = userMe;
                        Panel.Config.server = userMe.server;
                        Panel.Config.token = token.value;
                        Panel.SaveConfig(Panel.Config);
                        Panel.LoadGUI();
                        return;
                    }
                }
                else
                {
                    // if valid username with regex
                    if (!new System.Text.RegularExpressions.Regex(@"^[A-Za-z][A-Za-z0-9_]{3,16}$").IsMatch(username.value))
                    {
                        username.SetEnabled(true);
                        password.SetEnabled(true);
                        server.SetEnabled(true);
                        loginButton.SetEnabled(true);
                        token.SetEnabled(true);
                        error.text = "Invalid username";
                        return;
                    }

                    // if valid password with regex
                    if (!new System.Text.RegularExpressions.Regex(@"^.{8,}$").IsMatch(password.value))
                    {
                        username.SetEnabled(true);
                        password.SetEnabled(true);
                        server.SetEnabled(true);
                        loginButton.SetEnabled(true);
                        token.SetEnabled(true);
                        error.text = "Invalid password";
                        return;
                    }

                    var session = new AVR.Session().GetSessionLogin(username.value, password.value, server.value);
                    if (session == null)
                    {
                        error.text = "Impossible to login.";
                        username.SetEnabled(true);
                        password.SetEnabled(true);
                        server.SetEnabled(true);
                        loginButton.SetEnabled(true);
                        token.SetEnabled(true);
                        return;
                    }
                    else
                    {
                        error.text = session.user.username + " logged in.";
                        username.SetEnabled(true);
                        password.SetEnabled(true);
                        server.SetEnabled(true);
                        loginButton.SetEnabled(true);
                        token.SetEnabled(true);
                        user = session.user;
                        Panel.Config.server = session.user.server;
                        Panel.Config.token = session.token;
                        Panel.SaveConfig(Panel.Config);
                        Panel.LoadGUI();
                        return;
                    }
                }
            }
        }
    }
}