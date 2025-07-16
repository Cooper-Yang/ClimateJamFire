using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    public Transform targetWorldObject;
    public Vector3 screenOffset = new Vector3(0f, 60f, 0f);

    private RectTransform rectTransform;
    private Canvas parentCanvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    void LateUpdate()
    {
        if (targetWorldObject == null || parentCanvas == null) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(targetWorldObject.position);
        rectTransform.position = screenPos + screenOffset;
    }
}
