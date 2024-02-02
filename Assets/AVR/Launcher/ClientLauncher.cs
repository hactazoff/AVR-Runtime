using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AVR
{
    namespace Launchers
    {
        public class ClientLauncher : Base.Launcher
        {
            private void Start()
            {
                Super();
                Type = "client";

                Utils.Debug.Log("Starting client...");
                // run a task to fetch the server infos
                Initalize().Forget();
            }

            async UniTaskVoid Initalize()
            {
                var config = Utils.ConfigManager.Open();
                Utils.Debug.Log("token " + config.Get<string>("token"));
                Utils.Debug.Log("server " + config.Get<string>("server"));
                var userMe = await new Users.UserMe()
                {
                    token = config.Get<string>("token"),
                    server = config.Get<string>("server")
                }.FetchMe();

                if (userMe == null)
                {
                    Utils.Debug.Log("Failed to fetch user infos.");
                    return;
                }
                Utils.Debug.Log("User infos fetched. " + userMe.ToString());
                Debug.Log(userMe.Home);
                var w = await userMe.Home.Fetch();
                if (w == null)
                {
                    Utils.Debug.Log("Failed to fetch home world.");
                    return;
                }
                Utils.Debug.Log("Home world fetched. " + w.ToString());
            }
        }
    }
}