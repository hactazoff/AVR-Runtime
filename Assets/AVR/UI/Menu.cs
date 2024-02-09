using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace AVR
{
    namespace UI
    {
        public class Menu : MonoBehaviour
        {
            private Text hoclock;
            private Text hocfps;
            void Start()
            {
                hoclock = Find("avr-navs-top-time-text", true, true)?.GetComponent<Text>();
                hocfps = Find("avr-navs-top-fps-text", true, true)?.GetComponent<Text>();
            }

            private DateTime lastTime = DateTime.Now;
            void Update()
            {
                if (hoclock != null)
                    hoclock.text = System.DateTime.Now.ToString("HH:mm");
                if (hocfps != null && (DateTime.Now - lastTime).TotalSeconds > 1)
                {
                    hocfps.text = (1.0f / Time.deltaTime).ToString("0 FPS");
                    lastTime = DateTime.Now;
                }
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
            
            
            public bool SetTab(AVR.SDK.UI.TabOptions options)
            {
                AVR.SDK.Modding.Mod mod = null;
                GameObject tab = null;
                foreach(var m in AVR.Modding.Manager.LoadedMods())
                {
                    var o = m.Call<GameObject>("OnTab", options);
                    if(tab != null)
                    {
                        mod = m;
                        tab = o;
                        break;
                    }
                }
                if(mod == null)
                    return false;
                tab.transform.SetParent(Content.transform);
                
                return true;
            }  
            public GameObject Content => Find("avr-ontent", true, false); 
        }
        

    }
}