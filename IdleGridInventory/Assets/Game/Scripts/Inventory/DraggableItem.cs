using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class DraggableItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Shape (Cells) - Fallback")]
    [SerializeField] private int width = 1;
    [SerializeField] private int height = 1;

    public int Width => definition != null ? definition.Width : width;
    public int Height => definition != null ? definition.Height : height;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image image;

    private Vector2 originalAnchoredPos;
    private Transform originalParent;

    private bool isDragging;
    private bool isOnGrid;

    private ItemDefinitionSO definition;
    private bool isQueueItem;
    private ItemQueueManager ownerQueue;

    private Vector3 worldDragOffset;
    private Camera uiCamera;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);

        ApplySize();
    }

    public void Initialize(ItemDefinitionSO itemDefinition, bool isQueueItem, ItemQueueManager ownerQueue)
    {
        definition = itemDefinition;
        this.isQueueItem = isQueueItem;
        this.ownerQueue = ownerQueue;

        ApplyVisual();
        ApplySize();
    }

    private void ApplyVisual()
    {
        if (image == null) return;
        if (definition == null) return;

        if (definition.Icon != null)
            image.sprite = definition.Icon;
    }

    private void ApplySize()
    {
        if (GridManager.Instance == null) return;

        float size = GridManager.Instance.CellSize;
        float space = GridManager.Instance.Spacing;

        float w = Width * size + (Width - 1) * space;
        float h = Height * size + (Height - 1) * space;

        rectTransform.sizeDelta = new Vector2(w, h);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GridManager.Instance == null) return;

        isDragging = true;

        uiCamera = eventData.pressEventCamera;

        originalParent = rectTransform.parent;
        originalAnchoredPos = rectTransform.anchoredPosition;

        if (isOnGrid)
            GridManager.Instance.Clear(this);

        Vector3 pointerWorld = GetPointerWorld(eventData.position);
        worldDragOffset = rectTransform.position - pointerWorld;

        rectTransform.SetParent(GridManager.Instance.DragLayerRoot, worldPositionStays: true);
        rectTransform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector3 pointerWorld = GetPointerWorld(eventData.position);
        rectTransform.position = pointerWorld + worldDragOffset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;

        bool placed =
            GridManager.Instance.TryGetCellIndex(eventData.position, uiCamera, out int x, out int y) &&
            GridManager.Instance.CanPlace(this, x, y);

        if (placed)
        {
            rectTransform.SetParent(GridManager.Instance.PlacedItemsRoot, worldPositionStays: false);
            rectTransform.anchoredPosition = GridManager.Instance.GetItemAnchoredPosition(x, y);

            GridManager.Instance.Place(this, x, y);

            if (isQueueItem)
            {
                isQueueItem = false;
                ownerQueue?.NotifyConsumedFromQueue(this);
            }
        }
        else
        {
            rectTransform.SetParent(originalParent, worldPositionStays: false);
            rectTransform.anchoredPosition = originalAnchoredPos;
        }

        canvasGroup.blocksRaycasts = true;
        worldDragOffset = Vector3.zero;
        uiCamera = null;
    }

    private Vector3 GetPointerWorld(Vector2 screenPos)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform, screenPos, uiCamera, out Vector3 worldPoint))
        {
            return worldPoint;
        }

        return rectTransform.position;
    }

    public void SetGridPosition(int x, int y)
    {
        isOnGrid = true;
    }

    public void ClearGridPosition()
    {
        isOnGrid = false;
    }
}
