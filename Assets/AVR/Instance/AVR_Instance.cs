using System;
using System.Collections;
using System.Net.Sockets;
using UnityEngine;

namespace AVR
{
    [Serializable]
    public class Instance
    {
        public string id = null;
        public string name = null;
        public int capacity;
        public string world;
        public string owner;
        public string server;
        public string[] tags;
        public string[] users;
        public string master;
        public bool connected = false;

        public AVR.Server Server { get => AVR.ServerManager.GetServerByAddress(server); }
        public AVR.User Owner
        {
            get
            {
                string[] owner = this.owner.Split('@');
                if (owner.Length == 1)
                    return AVR.UserManager.GetUser(owner[0], server);
                return AVR.UserManager.GetUser(owner[0], owner[1]);
            }
        }

        public AVR.User[] Users
        {
            get
            {
                AVR.User[] users = new AVR.User[this.users.Length];
                for (int i = 0; i < this.users.Length; i++)
                {
                    string[] user = this.users[i].Split('@');
                    if (user.Length == 1)
                        users[i] = AVR.UserManager.GetUser(user[0], server);
                    else users[i] = AVR.UserManager.GetUser(user[0], user[1]);
                }
                return users;
            }
        }

        public AVR.User Master
        {
            get
            {
                if (this.master == null)
                    return null;
                string[] master = this.master.Split('@');
                if (master.Length == 1)
                    return AVR.UserManager.GetUser(master[0], server);
                return AVR.UserManager.GetUser(master[0], master[1]);
            }
        }

        public class InstanceInputCreate
        {
            public string world;
            public string[] tags;
        }

        public delegate void OnInstanceCreated(AVR.Instance instance);
        public static event OnInstanceCreated onInstanceCreated;

        public void CreateInstanceAsync(Action<AVR.Instance> callback, AVR.Server Server = null)
        {
            Server ??= AVR.ServerManager.ServerMe;
            string path = "/api/instances";
            if (server != Server?.address)
                path += "@" + server;
            AVR.Downloader.FetchAsync<AVR.Instance, object>(path, instance => AVR.Utils.GetStartup()
                .StartCoroutine(ParserInstanceAsync(instance, callback)),
                Server, "PUT", new InstanceInputCreate
                {
                    world = world,
                    tags = tags,
                });
        }

        public IEnumerator ParserInstanceAsync(AVR.Instance instance, Action<AVR.Instance> callback)
        {
            if (instance == null)
            {
                callback(null);
                yield break;
            }
            id = instance.id;
            name = instance.name;
            capacity = instance.capacity;
            world = instance.world;
            owner = instance.owner;
            server = instance.server;
            tags = instance.tags;
            users = instance.users;
            master = instance.master;
            AVR.InstanceManager.SetInstance(this);
            onInstanceCreated?.Invoke(this);
            callback(this);
        }

        public void ConnectSocket()
        {
            AVR.SocketManager.GetOrFetch(server, socket =>
            {
                if (socket == null)
                {
                    Debug.Log("Socket not found");
                    return;
                }
                if (!socket.Ready)
                {
                    if (!socket.Communicator.Instance.IsConnected())
                        socket.Connect();
                    socket.onReady += (bool authenticated) =>
                    {
                        if (authenticated)
                            SocketConnected(socket);
                    };
                }
                else if (socket.Ready)
                    SocketConnected(socket);
                else socket.onReady += (bool authenticated) =>
                {
                    if (authenticated)
                        SocketConnected(socket);
                };
            });
        }

        private void SocketConnected(AVR.Socket socket)
        {
            Debug.Log("Socket connected");
            connected = true;
            socket.Emit("local", new AVR.Socket.SocketRequest<JoinInstanceRequest>()
            {
                command = "instance:join",
                subgroup = "avr",
                data = new JoinInstanceRequest() { instance = id }
            });
        }

        // request for "instance:join"
        [Serializable]
        public class JoinInstanceRequest
        {
            public string instance;
        }
        [Serializable]
        public class JoinInstanceResponse
        {
            public bool success;
            public string message;
        }
    }
}