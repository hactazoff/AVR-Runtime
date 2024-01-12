using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AVR
{
    namespace SDK
    {
        public class NetworkTransport : MonoBehaviour
        {
            public AVR.SDK.WorldDescriptor worldDescriptor;

            void Start()
            {
                transform.hasChanged = false;
                if (worldDescriptor == null)
                    worldDescriptor = AVR.Utils.GetComponent<AVR.SDK.WorldDescriptor>(gameObject.scene);
                worldDescriptor.networkTransports.Add(this);
            }

            void Update()
            {
                if (transform.hasChanged)
                {
                    SendTransform();
                    transform.hasChanged = false;
                }
            }

            void SetTransform(Transform transform, Rigidbody rigidbody)
            {
                // set transform from server
                this.transform.SetPositionAndRotation(transform.position, transform.rotation);

                // set velocity from server
                if (rigidbody != null)
                    if (TryGetComponent<Rigidbody>(out var rb))
                    {
                        rb.velocity = rigidbody.velocity;
                        rb.angularVelocity = rigidbody.angularVelocity;
                    }
            }

            void SendTransform()
            {
                var instance = AVR.InstanceManager.GetConnectedInstance();
                AVR.SocketManager.GetSocketInstance?.Emit<NetworkTransform>("instance:" + instance.id, new()
                {
                    command = "transform",
                    subgroup = "avr",
                    data = new NetworkTransform()
                    {
                        i = AVR.Utils.GetGameObjectPath(gameObject),
                        p = new double[] { transform.position.x, transform.position.y, transform.position.z },
                        r = new double[] { transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w },
                        v = TryGetComponent<Rigidbody>(out var rb) ? new double[] { rb.velocity.x, rb.velocity.y, rb.velocity.z } : null,
                        a = TryGetComponent<Rigidbody>(out var rb2) ? new double[] { rb2.angularVelocity.x, rb2.angularVelocity.y, rb2.angularVelocity.z } : null
                    }
                });
            }

            [System.Serializable]
            public class NetworkTransform
            {
                public string i;
                public double[] p;
                public double[] r;
                public double[] v;
                public double[] a;
            }
        }
    }
}