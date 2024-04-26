using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

[HarmonyPatch(typeof(CheatsManager), "Start")]
public class CheatsPatcher {
    [HarmonyPrefix]
    public static void Prefix(CheatsManager __instance) {
        //__instance.RebuildIcons();
        
        Debug.Log("Yippe!");
        __instance.RegisterExternalCheat(new ShowZoneCheats());
    }
}
