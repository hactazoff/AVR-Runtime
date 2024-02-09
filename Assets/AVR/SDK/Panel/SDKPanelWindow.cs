#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace AVR.SDK
{
    public class SDKPanelWindow : EditorWindow
    {
		private bool _isInitialized;

        private void OnGUI()
        {
	        Debug.Log("SDKPanelWindow.OnGUI1");
			if (_isInitialized) return;
            _isInitialized = true;
            Debug.Log("SDKPanelWindow.OnGUI");
			var visualTree = Resources.Load<VisualTreeAsset>("avr.sdk.panel");
			var uxml = visualTree.CloneTree();
			rootVisualElement.Clear();
	        rootVisualElement.Add(uxml);
			rootVisualElement.RegisterCallback<GeometryChangedEvent>(evt => OnResize());
			OnResize();

			var mods = Modding.SDKModManager.GetMods();
			var tabs = rootVisualElement.Q<TabView>("avr-tabs");
			foreach (var mod in mods)
			{
				Debug.Log("Mod: "+mod.Name);
				var tab = mod.Call<Tab>("OnSDKTab");
				if (tab == null) continue;
				Debug.Log("Tab: "+tab.label);
				tabs.Add(tab);
			}
        }

		private void OnResize()
		{
			var size = position.size;
			rootVisualElement[0].style.width = size.x;
			rootVisualElement[0].style.height = size.y;
		}
    }
}
#endif