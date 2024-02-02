using Cysharp.Threading.Tasks;

namespace AVR
{
    namespace Launchers
    {
        public class ServerLauncher : Base.Launcher
        {
            public string instance;
            public string token;
            public Instances.Instance Instance;

            private void Start()
            {
                Super();
                Type = "server";
                Initalize().Forget();
            }

            public async UniTaskVoid Initalize()
            {
                var config = Utils.ConfigManager.Open();
                config.Set("runtime", "server");
                config.Save();
                Instance = await Instances.InstanceManager.GetOrFetchInstance(instance);
            }
        }
    }
}