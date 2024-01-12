using Katas.Experimental;
using UnityEngine;
using UnityEngine.UIElements;

namespace AVR
{
    namespace SDK
    {
        public class  BaseUI : MonoBehaviour
        {
            public bool ready = false;
            public GameObject content => Find("avr-content");
            public Animator animator => GetComponent<Animator>();

            public GameObject Find(string name)
            {
                return transform.Find(name).gameObject;
            }

            public bool SetActiveNav(bool active)
            {
                Debug.Log("SetActiveNav " + active);
                animator.SetBool("avr-hide-navs", active);
                return active;
            }
        }
    }
}