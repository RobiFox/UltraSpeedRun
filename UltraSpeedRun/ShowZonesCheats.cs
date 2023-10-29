using System;
using UnityEngine;

public class ShowZoneCheats : ICheat {

    public string LongName => "Show All Trigger Zones";

    public string Identifier => "ultraspeedrun.zones";

    public string ButtonEnabledOverride => "Don't Show Zones Next Restart";

    public string ButtonDisabledOverride => "Show Zones";

    public string Icon => "noclip";

    public bool IsActive => false;

    public bool DefaultState => false;

    public StatePersistenceMode PersistenceMode => StatePersistenceMode.NotPersistent;

    public void Disable()  {
    }

    public void Enable() {
        theMaterial = SpeedRunMod.FindObjectOfType<PlayerActivator>().GetComponent<MeshRenderer>().material;
        SetObjectsVisible<ObjectActivator>();
        SetObjectsVisible<ActivateArena>();
    }

    private Material theMaterial;

    public void SetObjectsVisible<T>() where T : MonoBehaviour {
        foreach(var obj in Resources.FindObjectsOfTypeAll<T>()) {
            obj.gameObject.layer = 0;
            if(obj.TryGetComponent<MeshRenderer>(out MeshRenderer renderer)) {
                renderer.enabled = true;
                if(renderer.material.shader.name != theMaterial.shader.name) renderer.material = theMaterial;
            }
        }
    }

    public void Update() {

    }
}
