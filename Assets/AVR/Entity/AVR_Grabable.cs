using System.Collections.Generic;
using UnityEngine;

namespace AVR
{
    public class GrabEntity : AVR.Entity
    {
        public bool isGrabaable = true;
        public bool allowMultipleGrab = false;

        public List<AVR.Grabber> grabbers = new();

        public AVR.Grabber[] Grabbers
        {
            get
            {
                foreach (var grabber in grabbers)
                    if (grabber.grabber != null)
                    {
                        grabber.relpos = grabber.grabber.transform.localPosition;
                        grabber.relrot = grabber.grabber.transform.localRotation;
                        grabber.zoneSize = grabber.grabber.transform.localScale;
                    }
                return grabbers.ToArray();
            }
        }
        // add gizmo
        private void OnDrawGizmos()
        {
            foreach (var grabber in Grabbers)
            {
                // add plane gizmo
                Gizmos.color = Color.red;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(grabber.relpos, grabber.zoneSize);
            }
        }
    }

    [System.Serializable]
    public class Grabber
    {
        public GameObject grabber;
        public Vector3 relpos;
        public Quaternion relrot;
        public Quaternion angleOffset;
        public Vector3 zoneSize;
    }
}
