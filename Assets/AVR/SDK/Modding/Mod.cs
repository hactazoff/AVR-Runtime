using System.Reflection;
using UnityEngine;

namespace AVR
{
    namespace SDK
    {
        namespace Modding
        {
            public class Mod
            {
                public void Setup(Assembly assembly, string name, string path)
                {
                    if(setupped) return;
                    Assembly = assembly;
                    Debug.Log("z "+string.Join(" ", assembly.GetManifestResourceNames()));
                    setupped = true;
                }

                public bool setupped { get; private set; } = false;
                private Assembly Assembly;
                public string Name;
                public string Id;
                
                public bool loaded { get; private set; } = false;

                public virtual void OnClientStart() { }
                public virtual void OnServerStart() { }
                public virtual void OnUpdate() { }

                public virtual void OnLoad()
                {
                    loaded = true;
                }

                public virtual void OnUnload()
                {
                    loaded = false;
                }
                
                public T Call<T>(string method, params object[] args)
                {
                    var type = GetType();
                    var methodInfo = type.GetMethod(method);
                    if (methodInfo == null) return default;
                    return (T)methodInfo.Invoke(this, args);
                }
            }
        }
    }
}