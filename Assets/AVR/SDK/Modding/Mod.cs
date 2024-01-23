namespace AVR
{
    namespace SDK
    {
        namespace Modding
        {
            public class Mod
            {
                public string Name;
                public string Id;

                public virtual void OnStart() { }
                public virtual void OnUpdate() { }
                public virtual void OnLoad() { }
                public virtual void OnUnload() { }
            }
        }
    }
}