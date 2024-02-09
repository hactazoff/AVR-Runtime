using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fr.hactazia.modtest
{
    public class Modtest : AVR.SDK.Modding.Mod
    {
        public override void OnLoad()
        {
            Id = "fr.hactazia.modtest";
            Name = "Modtest";
            Debug.Log("Modtest loaded!");
            base.OnLoad();
        }

        public override void OnUnload()
        {
            Debug.Log("Modtest unloaded!");
            base.OnUnload();
        }

        public override void OnClientStart()
        {
            Debug.Log("Modtest started on client!");
        }

        public override void OnUpdate()
        {
            Debug.Log("Modtest updated!");
        }

        public GameObject OnTab(AVR.SDK.UI.TabOptions tab)
        {
            Debug.Log("Modtest tabbed!");
            return null;
        }
    }
}