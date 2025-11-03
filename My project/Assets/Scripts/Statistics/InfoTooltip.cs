using UnityEngine;
using UnityEngine.EventSystems;

public class InfoTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject infoPanel;
    
    [TextArea(3, 10)]
    public string infoText;

    private TMPro.TextMeshProUGUI textComponent;

    void Start()
    {
        if (infoPanel != null)
        {
            textComponent = infoPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            infoPanel.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (infoPanel != null && textComponent != null)
        {
            textComponent.text = infoText;
            infoPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
}