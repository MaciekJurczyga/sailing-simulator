using UnityEngine;
using UnityEngine.UI.Extensions;

public class MinimapController : MonoBehaviour
{
    [Header("Obiekty UI (przeciągnij z Hierarchii)")]
    public Transform playerTransform;
    public RectTransform minimapBackground; 
    public RectTransform mapContainer;     
    public RectTransform playerIcon;
    public UILineRenderer lineRenderer;

    [Header("Ustawienia Wyglądu")]
    [Tooltip("Maksymalny rozmiar dłuższego boku minimapy w pikselach")]
    public float maxMinimapSize = 250f;
    [Tooltip("Odstęp ramki od krawędzi tła w pikselach")]
    public float padding = 10f;
    
    private readonly float minWorldX = -2923.38f;
    private readonly float maxWorldX = 3185.62f;
    private readonly float minWorldZ = -2262.37f;
    private readonly float maxWorldZ = 2701.6f;

    private float worldWidth;
    private float worldHeight;

    void Start()
    {
        worldWidth = maxWorldX - minWorldX;
        worldHeight = maxWorldZ - minWorldZ;
        
        SetupMinimapAppearance();
    }

    void SetupMinimapAppearance()
    {
        if (minimapBackground == null || mapContainer == null) return;
        
        float worldAspectRatio = worldWidth / worldHeight;
        
        float bgWidth, bgHeight;
        if (worldAspectRatio > 1) { 
            bgWidth = maxMinimapSize;
            bgHeight = bgWidth / worldAspectRatio;
        } else { 
            bgHeight = maxMinimapSize;
            bgWidth = bgHeight * worldAspectRatio;
        }
        
        minimapBackground.sizeDelta = new Vector2(bgWidth, bgHeight);
        
        mapContainer.sizeDelta = new Vector2(bgWidth - (padding * 2), bgHeight - (padding * 2));
        
        DrawBorders();
    }

    void DrawBorders()
    {
        if (lineRenderer == null) return;
        Rect containerRect = mapContainer.rect;
        Vector2 pivot = mapContainer.pivot;
        float x0 = -containerRect.width * pivot.x;
        float y0 = -containerRect.height * pivot.y;
        float x1 = containerRect.width * (1 - pivot.x);
        float y1 = containerRect.height * (1 - pivot.y);
        var points = new Vector2[] {
            new Vector2(x0, y0), new Vector2(x0, y1), new Vector2(x1, y1),
            new Vector2(x1, y0), new Vector2(x0, y0)
        };
        lineRenderer.Points = points;
        lineRenderer.LineThickness = 3.0f;
        lineRenderer.color = Color.red;
        lineRenderer.SetAllDirty();
    }

    void LateUpdate()
    {
        Vector3 playerPos = playerTransform.position;
        float percentX = (playerPos.x - minWorldX) / worldWidth;
        float percentZ = (playerPos.z - minWorldZ) / worldHeight;
        float mappedX = percentX - 0.5f;
        float mappedZ = percentZ - 0.5f;
        float iconX = mappedX * mapContainer.rect.width;
        float iconZ = mappedZ * mapContainer.rect.height;
        playerIcon.anchoredPosition = new Vector2(iconX, iconZ);
        playerIcon.localEulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);
    }
}