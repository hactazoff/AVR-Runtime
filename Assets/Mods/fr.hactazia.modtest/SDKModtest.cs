#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace fr.hactazia.modtest
{
    [InitializeOnLoad]
    public class SDKModtest : AVR.SDK.Modding.SDKMod
    {
        static SDKModtest() => Super(typeof(SDKModtest));

        public override void OnLoad()
        {
            Id = "fr.hactazia.sdk.modtest";
            Name = "SDKModtest";
            Debug.Log("SDKModtest loaded!");
        }

        public Tab OnSDKTab()
        {
            var tab = new Tab(Name);
            tab.AddToClassList(Id.Replace(@".", "-"));
            // add button to start game
            var button = new Button(() => Debug.Log("SDKModtest button clicked!"));
            button.text = "Debug print";
            tab.Add(button);
            return tab;
        }
    }
}
#endif