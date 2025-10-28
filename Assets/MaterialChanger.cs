using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;   // Your object's renderer
    [SerializeField] private Material newMaterial;       // The material you want to assign

    void Start()
    {
        // This assigns the material reference directly — no new instance created
        targetRenderer.sharedMaterial = newMaterial;
    }
}
