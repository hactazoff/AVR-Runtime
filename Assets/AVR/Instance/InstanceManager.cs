using Cysharp.Threading.Tasks;

namespace AVR
{
    namespace Instances
    {
        public class InstanceManager : Base.Manager<Instance>
        {
            public delegate void OnInstanceUpdateEvent(Instance instance);
            public static event OnInstanceUpdateEvent OnInstanceUpdate;
            public static void OnUpdate(Instance instance) => OnInstanceUpdate?.Invoke(instance);

            public delegate void OnInstanceRemoveEvent(Instance instance);
            public static event OnInstanceRemoveEvent OnInstanceRemove;
            public static void OnRemove(Instance instance) => OnInstanceRemove?.Invoke(instance);

            public delegate void OnInstanceAddEvent(Instance instance);
            public static event OnInstanceAddEvent OnInstanceAdd;
            public static void OnAdd(Instance instance) => OnInstanceAdd?.Invoke(instance);

            public static Instance GetInstance(string search)
            {
                var ip = InstancePatern.Parser(search);
                if (ip == null)
                    return null;
                foreach (var instance in Cache)
                    if (instance.id == ip.id && instance.server == ip.server)
                        return instance;
                    else if (instance.name == ip.name && instance.server == ip.server)
                        return instance;
                return null;
            }

            public static void SetInstance(Instance instance)
            {
                if (GetInstance(instance.id) != null)
                {
                    Cache.Add(instance);
                    OnUpdate(instance);
                }
                else
                {
                    Cache.Add(instance);
                    OnAdd(instance);
                }
            }

            public static void RemoveInstance(Instance instance)
            {
                Cache.Remove(instance);
                OnRemove(instance);
            }

            public static async UniTask<Instance> GetOrFetchInstance(string search)
            {
                var instance = GetInstance(search);
                if (instance != null)
                    return instance;
                var ip = InstancePatern.Parser(search);
                if (ip == null)
                    return null;
                instance = new Instance() { id = ip.id, name = ip.name, server = ip.server };
                return await instance.Fetch();
            }
        }
    }
}