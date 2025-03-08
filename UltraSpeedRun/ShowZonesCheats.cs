using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ShowZoneCheats : ICheat {
    private bool _active = false;

    public IEnumerator Coroutine(CheatsManager manager) {
        yield return null;
    }

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

    public void Enable(CheatsManager manager) {
        _active = true;
        if (theMaterial == null) {
            Shader shader = null; // cant do shader.find for whatever reason
            Material mat = null;
            foreach (PlayerActivator pa in Resources.FindObjectsOfTypeAll<PlayerActivator>()) {
                MeshRenderer renderer = pa.GetComponent<MeshRenderer>();
                if (renderer != null && renderer.material != null && renderer.material.name.Contains("Trigger")) {
                    mat = renderer.material;
                    shader = renderer.material.shader;
                    break;
                }
            }
            theMaterial = new Material(shader);
            theMaterial.SetFloat(BlendMode, 2);
            theMaterial.SetFloat(VertexLighting, 0);
            theMaterial.SetFloat(Opacity, 0.5f);
            theMaterial.SetOverrideTag("RenderType", "Transparent");
            theMaterial.SetFloat(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            theMaterial.SetFloat(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            theMaterial.SetFloat(ZWrite, 0);
            theMaterial.DisableKeyword("ALPHA_TEST");
            theMaterial.EnableKeyword("TRANSPARENCY");
            theMaterial.EnableKeyword("_FOG_ON");
            theMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            theMaterial.EnableKeyword("_EMISSION");
            theMaterial.EnableKeyword("_GLITCHMODE_NONE");
            theMaterial.EnableKeyword("_USEALBEDOASEMISSIVE_ON");
            theMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        SetObjectsVisible<ObjectActivator>();
        SetObjectsVisible<ActivateArena>();
        SetObjectsVisible<DoorController>();
        SetObjectsVisible<OutOfBounds>();
        SetObjectsVisible<DeathZone>(true);
    }

    private Material theMaterial = null;
    
    private Dictionary<Type, Color> _colors = new Dictionary<Type, Color>();
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int BlendMode = Shader.PropertyToID("_BlendMode");
    private static readonly int VertexLighting = Shader.PropertyToID("_VertexLighting");
    private static readonly int Opacity = Shader.PropertyToID("_Opacity");
    private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
    private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
    private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

    public ShowZoneCheats() {
        _colors.Add(typeof(ActivateArena), new Color(0.23f, 0.5f, 0.2f, 1f));
        _colors.Add(typeof(DoorController), new Color(0.2f, 0.2f, 0.5f,15f));
        _colors.Add(typeof(DeathZone), new Color(0.7f, 0.2f, 0.2f, 1f));
        _colors.Add(typeof(ObjectActivator), new Color(0.7f, 0.4f, 0.7f, 1f));
    }
    
    public void SetObjectsVisible<T>(bool includeChildren = false) where T : MonoBehaviour {
        foreach (T obj in Resources.FindObjectsOfTypeAll<T>()) {
            SetObjectsVisibleGameObject<T>(obj.gameObject);
            if (includeChildren) {
                foreach (Transform child in obj.transform) {
                    SetObjectsVisibleGameObject<T>(child.gameObject);
                }
            }
        }
    }
    
    public void SetObjectsVisibleGameObject<T>(GameObject obj) where T : MonoBehaviour {
        obj.gameObject.layer = 0;
        if(obj.TryGetComponent<MeshRenderer>(out MeshRenderer renderer)) {
            MakeVisible<T>(renderer);
        } else {
            if (obj.GetComponent<Collider>() == null) return;
            if (!obj.TryGetComponent<BoxCollider>(out BoxCollider boxCollider)) return;
            MeshRenderer meshRenderer = obj.gameObject.AddComponent<MeshRenderer>();
            MeshFilter meshFilter;
            if (!obj.gameObject.TryGetComponent<MeshFilter>(out meshFilter)) {
                meshFilter = obj.gameObject.AddComponent<MeshFilter>();
            }
            GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Mesh cubeMesh = tempCube.GetComponent<MeshFilter>().sharedMesh;
            Object.Destroy(tempCube);
            Mesh scaledMesh = Object.Instantiate(cubeMesh);
            Vector3[] vertices = scaledMesh.vertices;
            for (int i = 0; i < vertices.Length; i++) {
                vertices[i] = Vector3.Scale(vertices[i], boxCollider.size);
            }
            scaledMesh.vertices = vertices;
            scaledMesh.RecalculateBounds();
            meshFilter.mesh = cubeMesh;
            MakeVisible<T>(meshRenderer);
        }
        
    }

    public void MakeVisible<T>(MeshRenderer renderer) where T : MonoBehaviour {
        renderer.enabled = true;
        renderer.material = theMaterial;
        if (_colors.ContainsKey(typeof(T))) {
            //renderer.material.color = _colors[typeof(T)];
            //renderer.material.SetColor(EmissionColor, _colors[typeof(T)]);
            renderer.material.SetColor(Color1, _colors[typeof(T)]);
            renderer.material.color = _colors[typeof(T)];
        }
        if (!objectsMadeVisible.Contains(renderer)) objectsMadeVisible.Add(renderer);
    }
}
