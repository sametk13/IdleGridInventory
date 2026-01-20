using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class InventoryItemView : MonoBehaviour, IBrainModule
{
    private IBrain brain;

    [Header("UI Refs")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image overlayIconImage;

    private RectTransform rectTransform;
    private RectTransform overlayRectTransform;

    private void Awake()
    {
        TryAssignReferances();
    }
    public void Inject(IBrain brain)
    {
        this.brain = brain;
        TryAssignReferances();
    }

    public void SetIcons(Sprite sprite)
    {
        if (iconImage == null) return;
        iconImage.sprite = sprite;
        iconImage.enabled = sprite != null;

        if (overlayIconImage == null) return;
        overlayIconImage.sprite = sprite;
        overlayIconImage.enabled = sprite != null;
    }
    public void SetImageSizes(float w, float h)
    {
        rectTransform.sizeDelta = new Vector2(w, h);
        overlayRectTransform.sizeDelta = new Vector2(w, h);
    }
    private void TryAssignReferances()
    {
        if (rectTransform == null)
            rectTransform = iconImage.gameObject.GetComponent<RectTransform>();
        if (overlayRectTransform == null)
            overlayRectTransform = overlayIconImage.gameObject.GetComponent<RectTransform>();

    }
}
