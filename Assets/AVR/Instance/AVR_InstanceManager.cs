using System.Collections.Generic;

namespace AVR
{
    public static class InstanceManager
    {
        public delegate void OnCacheUpdate();
        public static event OnCacheUpdate onCacheUpdate;
        public static List<AVR.Instance> instances = new();

        public static AVR.Instance GetInstance(string id, string server)
        {
            foreach (AVR.Instance instance in instances)
                if (instance.id == id && instance.server == server)
                    return instance;
            return null;
        }

        public static void SetInstance(AVR.Instance instance)
        {
            AVR.Instance existingInstance = GetInstance(instance.id, instance.server);
            if (existingInstance != null)
                instances.Remove(existingInstance);
            instances.Add(instance);
            onCacheUpdate?.Invoke();
        }

        public static void RemoveInstance(AVR.Instance instance)
        {
            if (instance != null)
                instances.Remove(instance);
            onCacheUpdate?.Invoke();
        }

        public static AVR.Instance GetConnectedInstance()
        {
            foreach (var instance in instances)
                if (instance.connected)
                    return instance;
            return null;
        }
    }
}