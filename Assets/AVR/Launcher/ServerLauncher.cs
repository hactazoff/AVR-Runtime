using Cysharp.Threading.Tasks;

namespace AVR
{
    namespace Launcher
    {
        public class ServerLauncher : Base.Launcher
        {
            public string instance;
            public string token;
            public Instance.Instance Instance;

            private void Start()
            {
                Super();
                Initalize().Forget();
            }

            public async UniTaskVoid Initalize()
            {
                Instance = await AVR.Instance.InstanceManager.GetOrFetchInstance(instance);
            }
        }
    }
}