using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modtest : AVR.SDK.Modding.Mod
{
    public override void OnLoad()
    {
        Id = "fr.hactazia.modtest";
        Name = "Modtest";
        Debug.Log("Modtest loaded!");
    }

    public override void OnUnload()
    {
        Debug.Log("Modtest unloaded!");
    }

    public override void OnStart()
    {
        Debug.Log("Modtest started!");
    }

    public override void OnUpdate()
    {
        Debug.Log("Modtest updated!");
    }


}
