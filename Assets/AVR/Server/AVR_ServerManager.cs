using System;
using System.Collections.Generic;

namespace AVR
{
    public static class ServerManager
    {
        public static List<AVR.Server> servers = new();

        public delegate void OnCacheUpdate();
        public static event OnCacheUpdate onCacheUpdate;

        public static AVR.Server ServerMe
        {
            get => GetServerByAddress(AVR.UserManager.UserMe?.server);
        }

        public static AVR.Server GetServerById(string id)
        {
            foreach (AVR.Server server in servers)
                if (server.id == id)
                    return server;
            return null;
        }

        public static AVR.Server GetServerByAddress(string address)
        {
            foreach (AVR.Server server in servers)
                if (server.address == address)
                    return server;
            return null;
        }

        public static void SetServer(AVR.Server server)
        {
            AVR.Server existingServer = GetServerById(server.id);
            if (existingServer != null)
                servers.Remove(existingServer);
            servers.Add(server);
            onCacheUpdate?.Invoke();
        }

        public static void RemoveServer(AVR.Server server)
        {
            if (server != null)
                servers.Remove(server);
            onCacheUpdate?.Invoke();
        }

        public static void GetOrFetch(string address, Action<AVR.Server> callback)
        {
            AVR.Server server = GetServerByAddress(address);
            if (server != null)
            {
                callback(server);
                return;
            }
            server = new AVR.Server() { address = address };
            server.GetInfoAsync(callback);
        }
    }
}