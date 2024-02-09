#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
namespace AVR.SDK
{
    public class SDKPanel
    {
        [MenuItem("AVR/SDK Panel")]
        public static void ShowSDKPanel()
        {
            SDKPanelWindow window = (SDKPanelWindow)EditorWindow.GetWindow(typeof(SDKPanelWindow));
            window.titleContent = new GUIContent("SDK Panel");
            window.Show();
        }
    }
}
#endif