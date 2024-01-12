using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AVR
{
    [CustomEditor(typeof(AVR.Startup)), CanEditMultipleObjects]
    public class Startup_Inspector : Editor
    {
        VisualElement root;

        void UpdateUI()
        {
            AVR.Debug.Log("AVR Startup Inspector: Update UI");
            root.Q<VisualElement>("arv-server-list").Clear();
            foreach (AVR.Server server in AVR.ServerManager.servers)
            {
                VisualElement serverElement = Resources.Load<VisualTreeAsset>("avr.inspector.startup.item").CloneTree();
                var text = server.title ?? server.id;
                serverElement.Q<Label>("avr-st").text = text != null ? $"{text} ({server.address})" : server.address;
                serverElement.Q<Image>("avr-si").image = server.Icon;
                root.Q<VisualElement>("arv-server-list").Add(serverElement);
            }

            root.Q<VisualElement>("arv-user-list").Clear();
            foreach (AVR.User user in AVR.UserManager.users)
            {
                VisualElement userElement = Resources.Load<VisualTreeAsset>("avr.inspector.startup.item").CloneTree();
                userElement.Q<Label>("avr-st").text = (user.display ?? user.username ?? user.id) + "@" + user.server;
                userElement.Q<Image>("avr-si").image = user.Thumbnail;
                root.Q<VisualElement>("arv-user-list").Add(userElement);
            }

            root.Q<VisualElement>("arv-socket-list").Clear();
            foreach (AVR.Socket socket in AVR.SocketManager.sockets)
                if (socket.Communicator.Instance != null)
                {
                    VisualElement socketElement = Resources.Load<VisualTreeAsset>("avr.inspector.startup.item").CloneTree();
                    socketElement.Q<Label>("avr-st").text = socket.server
                        + $" ({socket.Communicator.Instance.SocketID}) - {(socket.Communicator.Instance.IsConnected() ? "Connected" : "Disconnected")}";
                    root.Q<VisualElement>("arv-socket-list").Add(socketElement);
                }

            root.Q<VisualElement>("arv-instance-list").Clear();
            foreach (AVR.Instance instance in AVR.InstanceManager.instances)
            {
                VisualElement instanceElement = Resources.Load<VisualTreeAsset>("avr.inspector.startup.item").CloneTree();
                instanceElement.Q<Label>("avr-st").text = "#" + instance.name + "@" + instance.server;
                root.Q<VisualElement>("arv-instance-list").Add(instanceElement);
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            if (root == null)
            {
                AVR.ServerManager.onCacheUpdate += () => UpdateUI();
                AVR.UserManager.onCacheUpdate += () => UpdateUI();
                AVR.SocketManager.onCacheUpdate += () => UpdateUI();
                AVR.InstanceManager.onCacheUpdate += () => UpdateUI();

                root = Resources.Load<VisualTreeAsset>("avr.inspector.startup").CloneTree();
            }
            UpdateUI();
            return root;
        }
    }
}