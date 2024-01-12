using UnityEditor;
using UnityEngine.UIElements;

namespace AVR
{
    namespace SDK
    {
        public class Plugin
        {

            public static AVR.SDK.Plugin Instance = null;

            public string Title = "Plugin";

            public virtual void OnPanelOpen()
            {
                return;
            }

            public virtual VisualElement OnPanel(AVR.SDK.Panel panel)
            {
                return null;
            }
        }
    }
}