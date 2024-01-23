using Cysharp.Threading.Tasks;

namespace AVR
{
    namespace Instance
    {
        public class InstanceManager : Base.Manager<Instance>
        {
            public static Instance GetInstance(string search)
            {
                var ip = InstancePatern.Parser(search);
                Instance ins = null;
                if (ip == null)
                    return null;
                if (ip.id != null)
                    ins = GetInstanceById(ip.id, ip.server);
                if (ip.name != null && ins == null)
                    ins = GetInstanceByName(ip.name, ip.server);
                return ins;
            }

            public static Instance GetInstanceById(string id, string address)
            {
                foreach (var instance in Cache)
                    if (instance.id == id && instance.server == address)
                        return instance;
                return null;
            }

            public static Instance GetInstanceByName(string name, string address)
            {
                foreach (var instance in Cache)
                    if (instance.name == name && instance.server == address)
                        return instance;
                return null;
            }

            public static void SetInstance(Instance instance)
            {
                RemoveInstance(instance);
                Cache.Add(instance);
            }

            public static void RemoveInstance(Instance instance)
            {
                if (GetInstanceById(instance.id, instance.server) != null)
                    Cache.Remove(instance);
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