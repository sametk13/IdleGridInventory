using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class DraggableItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
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

    public ItemDefinitionSO Definition => definition;

    public void AssignToQueue(ItemQueueManager queue)
    {
        ownerQueue = queue;
        isQueueItem = true;
        isOnGrid = false;
    }

    public IReadOnlyList<Vector2Int> ShapeCells
        => definition != null ? definition.ShapeCells : new[] { Vector2Int.zero };

    public RectInt ShapeBounds
        => definition != null ? definition.GetShapeBounds() : new RectInt(0, 0, 1, 1);

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

        RectInt bounds = ShapeBounds;

        float w = bounds.width * size + (bounds.width - 1) * space;
        float h = bounds.height * size + (bounds.height - 1) * space;

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

        if (GridManager.Instance.TryGetCellIndex(eventData.position, uiCamera, out int x, out int y))
            GridManager.Instance.ShowPlacementPreview(this, x, y);
        else
            GridManager.Instance.ClearPlacementPreview();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.LogError(11);

        if (!isDragging) return;
        Debug.LogError(2);

        Vector3 pointerWorld = GetPointerWorld(eventData.position);
        rectTransform.position = pointerWorld + worldDragOffset;

        if (GridManager.Instance.TryGetCellIndex(eventData.position, uiCamera, out int x, out int y))
            GridManager.Instance.ShowPlacementPreview(this, x, y);
        else
            GridManager.Instance.ClearPlacementPreview();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;

        ItemQueueManager queue = ownerQueue != null ? ownerQueue : FindFirstObjectByType<ItemQueueManager>();

        bool isOverGrid = GridManager.Instance.TryGetCellIndex(eventData.position, uiCamera, out int x, out int y);

        if (isOverGrid && GridManager.Instance.CanPlace(this, x, y))
        {
            rectTransform.SetParent(GridManager.Instance.PlacedItemsRoot, worldPositionStays: false);
            rectTransform.anchoredPosition = GridManager.Instance.GetItemAnchoredPosition(x, y);

            GridManager.Instance.TryPlaceWithKick(this, x, y, queue);

            if (isQueueItem)
            {
                isQueueItem = false;
                ownerQueue?.NotifyConsumedFromQueue(this);
            }

            // Always re-layout the queue after a successful placement.
            queue?.RefreshLayout();
        }
        else
        {
            if (queue != null)
            {
                queue.ReturnToQueue(this);
            }
            else
            {
                rectTransform.SetParent(originalParent, worldPositionStays: false);
                rectTransform.anchoredPosition = originalAnchoredPos;
            }
        }

        GridManager.Instance.ClearPlacementPreview();

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
