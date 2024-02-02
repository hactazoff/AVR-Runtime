using UnityEngine;
using UnityEngine.UI;
namespace AVR
{
    namespace UI
    {
        public class Menu : MonoBehaviour
        {
            public Text hoclock;
            void Start()
            {
                hoclock = Find("avr-navs-top-time-text", true, true)?.GetComponent<Text>();
            }

            void Update()
            {
                // detect time change and update time in the menu
                if (hoclock != null)
                    hoclock.text = System.DateTime.Now.ToString("HH:mm:ss");
            }

            public GameObject Find(string name, bool recursive = false, bool includeInactive = false, GameObject gameObject = null)
            {
                if (!includeInactive && !gameObject.activeSelf)
                    return null;
                if (gameObject == null)
                    gameObject = this.gameObject;
                foreach (Transform child in gameObject.transform)
                {
                    if (!includeInactive)
                        continue;
                    if (child.name == name)
                        return child.gameObject;
                    if (recursive)
                    {
                        GameObject result = Find(name, recursive, includeInactive, child.gameObject);
                        if (result != null)
                            return result;
                    }
                }
                return null;
            }
        }
    }
}