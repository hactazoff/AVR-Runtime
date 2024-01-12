using System;
using System.Net.Sockets;
using Firesplash.GameDevAssets.SocketIO;
using Firesplash.GameDevAssets.SocketIO.Internal;
using UnityEngine;
using static Firesplash.GameDevAssets.SocketIO.SocketIOInstance;
namespace AVR
{
    public class Socket
    {
        public GameObject context = null;
        public SocketIOInstance Instance = null;
        public string server = null;
        public AVR.Server Server { get => AVR.ServerManager.GetServerByAddress(server); }
        public SocketIOCommunicator Communicator { get => context.GetComponent<SocketIOCommunicator>(); }
        public string token = null;
        public bool IsServerMe { get => Server.address == AVR.ServerManager.ServerMe.address; }
        public bool IsInstance = false;


        public void Create()
        {
            context = new("socket-" + server);
            context.AddComponent<SocketIOCommunicator>();
            context.name = "socket-" + Communicator.GetInstanceID();
            context.transform.parent = AVR.Utils.GetStartup().tmp.transform;
            Communicator.Instance.On("ping", (string e) =>
            {
                int client = (int)(Time.time * 1000);
                PingResponse response = JsonUtility.FromJson<PingResponse>(e);
                ping = client - response.client;
                AVR.PluginManager.Message(new()
                {
                    evt = "avr.ping.response",
                    data = new() {
                    { "address", server },
                    { "ping", ping },
                    { "infos", e }
                     }
                });
            });
            Communicator.Instance.On("connect", (string e) =>
            {
                AVR.Debug.Log("Connected to " + Server.gateways.ws);
                AVR.PluginManager.Message(new()
                {
                    evt = "avr.socket.connect",
                    data = new() {
                    { "address", server },
                    { "infos", e }
                     }
                });
            });
            Communicator.Instance.On("disconnect", (string e) =>
            {
                AVR.Debug.Log("Disconnected from " + Server.gateways.ws);
                AVR.PluginManager.Message(new()
                {
                    evt = "avr.socket.disconnect",
                    data = new() {
                    { "address", server },
                    { "infos", e }
                     }
                });
            });
            Communicator.Instance.On("error", (string e) =>
            {
                AVR.Debug.Log("Error from " + Server.gateways.ws);
                AVR.PluginManager.Message(new()
                {
                    evt = "avr.socket.error",
                    data = new() {
                    { "address", server },
                    { "infos", e }
                     }
                });
            });

            Communicator.Instance.OnAny((string e, string b) =>
            {
                if (e != "ping")
                    AVR.Debug.Log("Socket event from " + Server.gateways.ws + ": " + e + " " + b);
            });
        }

        public delegate void OnReady(bool authenticated);
        public event OnReady onReady;

        public void Connect()
        {
            var n = new SIOAuthPayload();
            AVR.Debug.Log("Connecting to " + Server.gateways.ws);
            On("local", (SocketResponse<AuthenticateResponse> e) =>
            {
                if (e.data.success)
                {
                    AVR.Debug.Log("Authenticated with socket to " + Server.gateways.ws);
                    Ready = true;
                    onReady?.Invoke(true);
                }
                else
                {
                    AVR.Debug.Log("Failed to authenticate with socket to " + Server.gateways.ws);
                    Ready = true;
                    token = null;
                    onReady?.Invoke(false);
                }
            });


            Communicator.Instance.On("connect", (string e) =>
            {
                AVR.Debug.Log("Connected with socket to " + Server.gateways.ws);
                if (Server.address != AVR.ServerManager.ServerMe.address)
                    AVR.ServerManager.ServerMe.GetIntegrityAsync(Server.address, (AVR.ServerIntegrity integrity) =>
                    {
                        if (integrity == null)
                        {
                            Ready = true;
                            token = null;
                            onReady?.Invoke(false);
                            return;
                        }
                        Debug.LogWarning("Integrity add to socket: " + integrity.token);
                        Emit("local", new SocketRequest<AuthenticateRequest>()
                        {
                            command = "authenticate",
                            subgroup = "avr",
                            data = new AuthenticateRequest() { integrity = integrity.token, token = null }
                        });
                    });
                else
                {
                    Debug.LogWarning("UserMe add to socket: " + AVR.UserManager.UserMe.token);
                    Emit("local", new SocketRequest<AuthenticateRequest>()
                    {
                        command = "authenticate",
                        subgroup = "avr",
                        data = new AuthenticateRequest() { token = AVR.UserManager.UserMe.token, integrity = null }
                    });
                }
            });
            Communicator.Instance.Connect(Server.gateways.ws, true);
        }

        public void Emit<T>(string room, SocketRequest<T> data = default)
        {
            // AVR.Debug.Log("Emitting " + data.command + " to " + Server.gateways.ws);
            Communicator.Instance.Emit(room, JsonUtility.ToJson(data), false);
        }

        public void On<T>(string room, Action<SocketResponse<T>> callback)
        {
            AVR.Debug.Log("Listening to " + room + " from " + Server.gateways.ws);
            Communicator.Instance.On(room, c => callback(JsonUtility.FromJson<SocketResponse<T>>(c)));
        }

        public void Disconnect() => Communicator.Instance.Close();

        int ping = 0;
        public bool Ready { get; private set; } = false;
        public void Ping() => Communicator.Instance.Emit("ping", JsonUtility.ToJson(new PingRequest() { time = (int)(Time.time * 1000) }), false);

        // base class for all socket requests
        [Serializable]
        public class SocketRequest<T>
        {
            public string state;
            public string command;
            public string subgroup;
            public T data;
        }
        [Serializable]
        public class SocketResponse<T>
        {
            public string state;
            public string command;
            public string subgroup;
            public T data;
        }

        // for "authentication" requests
        [Serializable]
        public class AuthenticateRequest
        {
            public string token;
            public string integrity;
        }
        [Serializable]
        public class AuthenticateResponse
        {
            public bool success;
        }

        // for "ping" requests
        class PingRequest
        {
            public int time;
        }
        class PingResponse
        {
            public int client;
            public int server;
        }
    }
}