using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace AVR
{

    public class Startup : MonoBehaviour
    {

        void OnLoadScene(AVR.Scene.LoadSceneEventArgs scene)
        {
            AVR.Utils.ConfigRaw config = AVR.Utils.Config;
            if (scene.scene == default && scene.hash == config.fallback_world)
            {
                AVR.Debug.Log("AVR Startup: Failed to load default scene");
                config.fallback_world = null;
                AVR.Utils.Config = config;
                AVR.Scene.LoadDefaultSceneAsync((UnityEngine.SceneManagement.Scene scene) =>
                {
                    if (scene == default)
                    {
                        AVR.Debug.Log("AVR Startup: Fatal error, failed to load default scene");
                        Application.Quit(1);
                        return;
                    }
                });
            }

            if (scene.scene == default)
            {
                AVR.Debug.Log("AVR Startup: Failed to load scene");
                return;
            }

            if (scene.scene != default && (scene.hash == config.fallback_world || scene.hash == AVR.Scene.DefaultScene))
            {
                AVR.Debug.Log("AVR Startup: Loaded default scene");
                Initialize();
                return;
            }
        }

        public GameObject tmp = null;

        void Start()
        {
            AVR.Debug.Log("AVR Startup");
            tmp = new GameObject("AVRTmp");
            tmp.transform.parent = transform;

            AVR.PluginManager.Start(this);
            AVR.Scene.onLoadSceneSync += OnLoadScene;
            DontDestroyOnLoad(gameObject);
            AVR.Scene.LoadDefaultSceneAsync((UnityEngine.SceneManagement.Scene scene) => { });
        }

        public void Initialize()
        {
            AVR.Debug.Log("AVR Initialize");
            AVR.Debug.Log("Storage path: " + AVR.Utils.LocalPath);

            void SetTab(string id) => AVR.PluginManager.Message(new()
            {
                evt = "avr.settab",
                data = new() {
                { "for", "avr:all" },
                { "id", id }
            }
            });
            SetTab("avr-default");



            AVR.Utils.ConfigRaw config = AVR.Utils.Config;

            if (config.server_gateway == null)
            {
                AVR.Debug.Log("AVR Initialize: No server set");
                SetTab("avr-welcome");
                return;
            }

            if (config.token == null)
            {
                AVR.Debug.Log("AVR Initialize: No token set");
                SetTab("avr-welcome");
                return;
            }

            var server = new AVR.Server() { gateways = new() { http = config.server_gateway } };
            Debug.Log("AVR Initialize: Server: " + server.gateways.http);

            server.GetInfoAsync((AVR.Server info) =>
            {
                AVR.Debug.Log("**************************************************************************************************************************");
                if (info == null)
                {
                    AVR.Debug.Log("AVR Initialize: Failed to get server info");
                    SetTab("avr-welcome");
                    return;
                }

                AVR.Debug.Log("AVR Initialize: Server info: " + info.title);

                AVR.UserMe user = new() { token = config.token, server = info.address };

                user.GetUserMeAsync((AVR.UserMe info) =>
                {
                    if (info == null)
                    {
                        AVR.Debug.Log("AVR Initialize: Failed to get user info");
                        SetTab("avr-welcome");
                        return;
                    }


                    AVR.SocketManager.GetOrFetch(server.address, (AVR.Socket socket) =>
                    {
                        if (socket == null)
                        {
                            AVR.Debug.Log("AVR Initialize: Failed to get socket");
                            SetTab("avr-welcome");
                            return;
                        }

                        AVR.Debug.Log("AVR Initialize: Socket: " + socket.server);
                        socket.Connect();
                    });

                    AVR.Debug.Log("AVR Initialize: User info: " + info.username);
                    SetTab("avr-home");

                    // download home scene
                    user.GetHomeWorldAsync((AVR.World home) =>
                    {
                        if (home == null || home.Assets == null)
                        {
                            AVR.Debug.Log("AVR Initialize: Failed to load home scene");
                            return;
                        }
                        AVR.Debug.Log("AVR Initialize: Home scene: " + home.title);
                        var home_asset = home.Assets.FirstOrDefault();
                        if (home_asset == null)
                        {
                            AVR.Debug.Log("AVR Initialize: Failed to load home scene");
                            return;
                        }

                        home_asset.DownloadAssetAsync((bool success) =>
                        {
                            if (!success)
                            {
                                AVR.Debug.Log("AVR Initialize: Failed to load home scene");
                                return;
                            }
                            AVR.Scene.LoadSceneAsync(home_asset.hash, (UnityEngine.SceneManagement.Scene scene) =>
                            {
                                if (scene == default)
                                {
                                    AVR.Debug.Log("AVR Initialize: Failed to load home scene");
                                    return;
                                }
                                AVR.Debug.Log("AVR Initialize: Loaded home scene");

                                AVR.Instance instance = new() { server = info.server, world = home.id };
                                instance.CreateInstanceAsync((AVR.Instance info) =>
                                {
                                    if (info == null)
                                    {
                                        AVR.Debug.Log("AVR Initialize: Failed to create instance");
                                        return;
                                    }
                                    AVR.Debug.Log("AVR Initialize: Created instance: " + info.id);
                                    instance.ConnectSocket();
                                });
                            });
                        });
                    });
                }, info);
            });
        }

        void Update()
        {
            AVR.PluginManager.Update();
        }
    }
}