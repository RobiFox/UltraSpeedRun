using System;
using System.Collections.Generic;
using UnityEngine;

public class ShowZoneCheats : ICheat {
    private bool _active = false;

    public string LongName => "Show All Trigger Zones";

    public string Identifier => "ultraspeedrun.zones";

    public string ButtonEnabledOverride => "Hide Zones";

    public string ButtonDisabledOverride => "Show Zones";

    public string Icon => "noclip";

    public bool IsActive => _active;

    public bool DefaultState => false;

    public StatePersistenceMode PersistenceMode => StatePersistenceMode.Persistent;

    private List<MeshRenderer> objectsMadeVisible = new List<MeshRenderer>();

    public void Disable() {
        _active = false;
        foreach (var obj in objectsMadeVisible) {
            obj.enabled = false;
        }
    }

    public void Enable() {
        _active = true;
        PlayerActivator activator = SpeedRunMod.FindObjectOfType<PlayerActivator>();
        theMaterial = activator.GetComponent<MeshRenderer>().material;
        theMesh = activator.GetComponent<MeshFilter>().mesh;
        SetObjectsVisible<ObjectActivator>();
        SetObjectsVisible<ActivateArena>();
        SetObjectsVisible<DoorController>();
    }

    private Material theMaterial;
    private Mesh theMesh;
    
    private Dictionary<Type, Color> _colors = new Dictionary<Type, Color>();
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    public ShowZoneCheats() {
        _colors.Add(typeof(ActivateArena), new Color(0.23f, 0.5f, 0.2f, 0.25f));
        _colors.Add(typeof(DoorController), new Color(0.2f, 0.2f, 0.5f, 0.25f));
    }
    
    public void SetObjectsVisible<T>() where T : MonoBehaviour {
        foreach(var obj in Resources.FindObjectsOfTypeAll<T>()) {
            obj.gameObject.layer = 0;
            if(obj.TryGetComponent<MeshRenderer>(out MeshRenderer renderer)) {
                MakeVisible<T>(renderer);
            } else {
                // IDK i will fix it later
                MeshRenderer meshRenderer = obj.gameObject.AddComponent<MeshRenderer>();
                MeshFilter meshFilter;
                if (!obj.gameObject.TryGetComponent<MeshFilter>(out meshFilter)) {
                    meshFilter = obj.gameObject.AddComponent<MeshFilter>();
                }
                meshFilter.mesh = theMesh;
                MakeVisible<T>(meshRenderer);
            }
        }
    }

    public void MakeVisible<T>(MeshRenderer renderer) where T : MonoBehaviour {
        renderer.enabled = true;
        renderer.material = theMaterial;
        if (_colors.ContainsKey(typeof(T))) {
            //renderer.material.color = _colors[typeof(T)];
            renderer.material.SetColor(EmissionColor, _colors[typeof(T)]);
        }
        if (!objectsMadeVisible.Contains(renderer)) objectsMadeVisible.Add(renderer);
    }

    public void Update() {

    }
}
