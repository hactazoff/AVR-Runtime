using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI.BodyUI;
namespace AVR
{
    namespace UI
    {
        public class MinimalMenu : MonoBehaviour
        {
            public GameObject Options => transform.Find("Options").gameObject;
            public bool refresh = false;

            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
            }

            void OnValidate()
            {
                AVR.Utils.Debug.Log("Validating");
                // check numebr of children
                var count = Options.transform.childCount;
                var radius = Options.GetComponent<RectTransform>().rect.width / 2f;
                var icount = count + (count % 2 == 0 ? 2 : 1);
                AVR.Utils.Debug.Log("Count: " + count);
                for (int i = 0; i < count; i++)
                {
                    var angle = ((float)i + 1 + (count % 2 == 0 && i >= count / 2 ? 1 : 0)) / (float)icount * Mathf.PI * 2f + Mathf.PI / 2f;
                    var child = Options.transform.GetChild(i);
                    AVR.Utils.Debug.Log("Validating " + angle, child);
                    child.localPosition = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                }
            }
        }
    }
}