﻿using Firesplash.GameDevAssets.SocketIO.Internal;
using System;
using UnityEngine;

namespace Firesplash.GameDevAssets.SocketIO
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Networking/Socket.IO/Socket.IO Communicator (v3 and v4)")]
    public partial class SocketIOCommunicator : MonoBehaviour
    {
        /// <summary>
        /// The Address of the SocketIO-Server
        /// If you specify a path, it has to be the complete absolute path to the service (the default is /socket.io/)
        /// WARNING: If you need to change this at runtime, make sure to do it BEFORE connecting, else the change will have no effect.
        /// </summary>
        [Tooltip("Enter the Socket.IO Address without protocol here. Example: sio.example.com:1234\nIf you need to change this at runtime, make sure to do it BEFORE connecting or accessing the \"Instance\", else the change will have no effect. If you add a path, it must be the complete path to the socket.io service (default is /socket.io/ - Do not confuse with namespaces)")]
        [Header("<Hostname>[:<Port>][/<path>]")]
        [Obsolete("You should not directly access this property! Use the Connect(...) methods on the instance to change the address")]
        public string socketIOAddress = "sio.example.com";

        /// <summary>
        /// If set to true, the connection will use wss/https
        /// WARNING: If you need to change this at runtime, make sure to do it BEFORE connecting, else the change will have no effect.
        /// </summary>
        [Header("Use SSL/TLS? Do NOT check this box, if you are not using a publicly trusted SSL certificate for your server.")]
        public bool secureConnection = false;

        /// <summary>
        /// If set to true, the behavior will connect to the server within Start() method. If set to false, you will have to call Connect() on the behavior.
        /// WARNING: If autoConnect is enabled, you can not change the target server address at runtime.
        /// </summary>
        [Header("Shall the communicator automatically connect on \"Start\"?")]
        public bool autoConnect = false;

        /// <summary>
        /// If set to true, the behavior will connect to the server within Start() method. If set to false, you will have to call Connect() on the behavior.
        /// WARNING: If autoConnect is enabled, you can not change the target server address at runtime.
        /// </summary>
        [Header("Shall the communicator automatically reconnect when the connection is lost?")]
        [Obsolete("You should not directly access this property! Use the Connect(...) methods on the instance to change this flag")]
        public bool autoReconnect = true;

        //The actual instance created
        private SocketIOInstance _instance;

        /// <summary>
        /// Use this field to access the Socket.IO interfaces
        /// </summary>
        public SocketIOInstance Instance
        {
            get
            {
                if (_instance == null)
                {
#pragma warning disable CS0618
                    _instance = SocketIOManager.Instance.CreateSIOInstance(gameObject.name, (secureConnection ? "https" : "http") + "://" + socketIOAddress, autoReconnect);
#pragma warning restore CS0618
                }
                return _instance;
            }
        }

        private void Awake()
        {
            //This must be called by our GameObject to ensure a dispatcher is available.
            if (Application.platform != RuntimePlatform.WebGLPlayer) SIODispatcher.Verify();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (autoConnect)
            {
                Instance.Connect();
            }
        }

        private void OnDestroy()
        {
            Instance.Close();
        }

#if UNITY_WEBGL
        //Receiver for JSLib-Events
        private void RaiseSIOEvent(string EventPayloadString)
        {
            SIOEventStructure ep = JsonUtility.FromJson<SIOEventStructure>(EventPayloadString);
            Instance.RaiseSIOEvent(ep.eventName, ep.data);
        }

        //Receiver for JSLib-Events
        private void UpdateSIOStatus(int statusCode)
        {
            ((SocketIOWebGLInstance)Instance).UpdateSIOStatus(statusCode);
        }

        //Receiver for JSLib-Events
        private void UpdateSIOSocketID(string currentSocketID)
        {
            ((SocketIOWebGLInstance)Instance).UpdateSIOSocketID(currentSocketID);
        }

        //Receiver for JSLib-Events
        private void SIOWarningRelay(string logMsg)
        {
            SocketIOManager.LogWarning(logMsg);
        }

        //Receiver for JSLib-Events
        private void SIOErrorRelay(string logMsg)
        {
            SocketIOManager.LogError(logMsg);
        }
#endif
    }
}
