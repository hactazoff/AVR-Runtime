using UnityEngine;

namespace AVR
{
    public class Entity : MonoBehaviour
    {
        public virtual void Teleport(Transform target)
        {
            transform.SetPositionAndRotation(target.position, target.rotation);
        }
    }
}