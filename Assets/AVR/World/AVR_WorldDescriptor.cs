using System.Collections.Generic;
using UnityEngine;

namespace AVR
{
    namespace SDK
    {
        public class WorldDescriptor : MonoBehaviour
        {
            [HideInInspector]
            public string ContentType = "world";
            public string LocalId;
            public GameObject[] Spawn;
            public string[] Tags;
            public float jumpHeight = 1f;
            public float forwardSpeed = 1f;
            public float backwardSpeed = .75f;

            [HideInInspector]
            public List<AVR.SDK.NetworkTransport> networkTransports;

            public GameObject[] Spawns => Spawn.Length == 0 ? new GameObject[] { gameObject } : Spawn;

            public GameObject GetSpawn()
            {
                if (Spawn.Length == 0) return gameObject;
                if (SpawnWorker != null)
                {
                    System.Reflection.MethodInfo methodInfo = SpawnWorker.GetType().GetMethod("GetSpawn");
                    if (methodInfo != null)
                    {
                        try
                        {
                            return (GameObject)methodInfo.Invoke(SpawnWorker, Spawns);
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError(e.ToString());
                        }
                    }
                }
                return Spawn[Random.Range(0, Spawn.Length)];
            }

            public MonoBehaviour SpawnWorker;

#if UNITY_EDITOR
            void OnDrawGizmos()
            {
                foreach (GameObject spawn in Spawns)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(spawn.transform.position, spawn.transform.position + spawn.transform.forward * 0.5f);
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(transform.position, spawn.transform.position);
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(spawn.transform.position, 0.05f);
                }
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.position, 0.075f);
            }
#endif

        }
    }
}