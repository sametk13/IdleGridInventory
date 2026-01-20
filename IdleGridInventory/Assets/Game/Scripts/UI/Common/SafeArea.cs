using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    void Awake()
    {
        Apply();
    }

    void Apply()
    {
        RectTransform rt = GetComponent<RectTransform>();
        Rect safe = Screen.safeArea;

        Vector2 min = safe.position;
        Vector2 max = safe.position + safe.size;

        min.x /= Screen.width;
        min.y /= Screen.height;
        max.x /= Screen.width;
        max.y /= Screen.height;

        rt.anchorMin = min;
        rt.anchorMax = max;
    }
}
