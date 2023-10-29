using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

[HarmonyPatch(typeof(CheatsManager), "Start")]
public class CheatsPatcher {
    [HarmonyPostfix]
    public static void Postfix(CheatsManager __instance) {
        Debug.Log("Added cool cheat Added cool cheat Added cool cheat Added cool cheat Added cool cheat Added cool cheat Added cool cheat Added cool cheat Added cool cheat Added cool cheat Added cool cheat ");
        __instance.RebuildIcons();
        
        Dictionary<string, List<ICheat>> registeredCheats = new Dictionary<string, List<ICheat>> { { "speedrun", new List<ICheat>() } };
        registeredCheats["speedrun"].Add(new ShowZoneCheats());

        foreach (KeyValuePair<string, List<ICheat>> kvp in registeredCheats) {
            foreach (ICheat cheat in kvp.Value) {
                __instance.RegisterCheat(cheat, kvp.Key);
            }
        }

        CheatBinds.Instance.RestoreBinds(registeredCheats);
        __instance.RebuildMenu();
    }
}
