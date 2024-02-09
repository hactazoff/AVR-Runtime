using UnityEngine;
using UnityEditor;

namespace AVR.SDK.Modding
{
    public class SDKMod
    {
        public string Name { get; set; } = "default";
        public string Id { get; set; } = "default";
        
        public static void Super(System.Type classType)
        {
            var i = (SDKMod)System.Activator.CreateInstance(classType);
            i.OnLoad();
            SDKModManager.AddMod(i);
        }
        
        public virtual void OnLoad() { }
        
        public T Call<T>(string method, params object[] args)
        {
            var type = GetType();
            var methodInfo = type.GetMethod(method);
            if (methodInfo == null) return default;
            return (T)methodInfo.Invoke(this, args);
        }
    }
}