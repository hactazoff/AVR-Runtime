using System;
using System.Collections.Generic;
namespace AVR
{
    public static class SocketManager
    {
        public static List<AVR.Socket> sockets = new();

        public delegate void OnCacheUpdate();
        public static event OnCacheUpdate onCacheUpdate;

        public static AVR.Socket SocketMe
        {
            get => GetSocket(AVR.ServerManager.ServerMe?.id);
        }

        public static AVR.Socket GetSocket(string server)
        {
            foreach (AVR.Socket socket in sockets)
                if (socket.server == server)
                    return socket;
            return null;
        }

        public static void SetSocket(AVR.Socket socket)
        {
            AVR.Socket existingSocket = GetSocket(socket.server);
            if (existingSocket != null)
                sockets.Remove(existingSocket);
            sockets.Add(socket);
            onCacheUpdate?.Invoke();
        }

        public static void RemoveSocket(string server)
        {
            AVR.Socket existingSocket = GetSocket(server);
            if (existingSocket != null)
                sockets.Remove(existingSocket);
            onCacheUpdate?.Invoke();
        }

        public static AVR.Socket CreateSocket(string server, bool connect = true)
        {
            AVR.Socket socket = new() { server = server };
            socket.Create();
            SetSocket(socket);
            if (connect)
                socket.Connect();
            return socket;
        }

        public static void GetOrFetch(string server, Action<AVR.Socket> callback)
        {
            AVR.Socket socket = GetSocket(server);
            if (socket != null)
            {
                callback(socket);
                return;
            }
            socket = CreateSocket(server, false);
            callback(socket);
        }

        public static AVR.Socket GetSocketInstance
        {
            get
            {
                foreach (var socket in sockets)
                    if (socket.server == AVR.InstanceManager.GetConnectedInstance()?.server)
                        return socket;
                return null;
            }
        }

        public static AVR.Socket GetSocketServerMe
        {
            get => GetSocket(AVR.ServerManager.ServerMe?.address);
        }
    }
}