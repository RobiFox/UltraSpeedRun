using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.PlayerLoop;

[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
public class SpeedRunMod : BaseUnityPlugin {
    public const string pluginGuid = "robi.uk.speedrun";
    public const string pluginName = "Speedrun Thingy";
    public const string pluginVersion = "1.3.0";

    private static Harmony _harmony;

    private void Awake() {
        Debug.Log("Loaded mod.");
    }

    void Start() {
        _harmony = new Harmony(pluginGuid);
        _harmony.PatchAll();

        Debug.Log("Harmony? " + _harmony == null);
    }

    private void OnDestroy() {
        _harmony?.UnpatchSelf();
        Debug.Log("Bye");
    }
}