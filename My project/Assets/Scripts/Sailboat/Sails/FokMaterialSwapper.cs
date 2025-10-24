
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FokMaterialSwapper : MonoBehaviour
{
    private Renderer objectRenderer;

    public void Initialize()
    {
    }
    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    public void SetMaterial(Material newMaterial)
    {
        if (objectRenderer != null && newMaterial != null)
        {
            objectRenderer.material = newMaterial;
        }
    }
}