using AVR.SDK.Base;
using UnityEngine;

namespace AVR.SDK.Worlds
{
    public class WorldDescriptor : AVR.SDK.Base.Descriptor
    {
        public override DescriptorType AssetType { get; } = DescriptorType.WORLD;
        public string id;
        public GameObject[] spawns;
        public GameObject[] Spawns => spawns.Length == 0 ? new GameObject[] { gameObject } : spawns;
        public GameObject ChoiseSpawn() => Spawns[Random.Range(0, Spawns.Length)];
        
#if UNITY_EDITOR
        void OnDrawGizmos()
            {
                foreach (GameObject spawn in Spawns)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(spawn.transform.position, spawn.transform.position + spawn.transform.forward * 0.5f);
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(transform.position, spawn.transform.position);
                }

                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.position, 0.075f);

                foreach (GameObject spawn in Spawns)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(spawn.transform.position, 0.025f);
                }
            }
#endif
    }
}