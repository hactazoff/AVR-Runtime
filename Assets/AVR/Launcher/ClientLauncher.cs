using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace AVR
{
    namespace Launcher
    {
        public class ClientLauncher : Base.Launcher
        {
            private void Start()
            {
                Super();

                AVR.Utils.Debug.Log("Starting client...");
                // run a task to fetch the server infos
                Initalize().Forget();
            }

            async UniTaskVoid Initalize()
            {
                var root = await User.UserManager.GetOrFetchUser("root", "127.0.0.1:3032");
                if (root == null)
                {
                    AVR.Utils.Debug.Log("Failed to fetch root user.");
                    return;
                }
                AVR.Utils.Debug.Log("Root user fetched. " + root.username);
            }
        }
    }
}