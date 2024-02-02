using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AVR
{
    namespace Launchers
    {

        [CustomEditor(typeof(ClientLauncher))]
        public class ClientLauncher_Inspector : Editor
        {
            public static VisualElement root = null;

            public override VisualElement CreateInspectorGUI()
            {
                root ??= Resources.Load<VisualTreeAsset>("avr.inspector.startup").CloneTree();
                return root;
            }

            public void UpdateRoot() {
                
            }
        }
    }
}