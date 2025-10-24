using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class GrotMaterialSwapper : MonoBehaviour
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