using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class DraggableItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IBrainModule
{
    private InventoryItemView itemView;
    private PlacedItemCooldownController cooldownController;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 originalAnchoredPos;
    private Transform originalParent;

    private bool isDragging;
    private bool isOnGrid;

    private ItemDefinitionSO definition;
    private bool isQueueItem;
    private ItemQueueManager ownerQueue;

    private Vector3 worldDragOffset;
    private Camera uiCamera;

    // New: which cell inside the item the user grabbed (relative to ShapeBounds top-left)
    private Vector2Int grabbedCellOffset;
    private bool hasGrabbedCellOffset;

    public ItemDefinitionSO Definition => definition;

    private IBrain brain;

    public void Inject(IBrain brain)
    {
        this.brain = brain;

        itemView = brain.Get<InventoryItemView>();
        cooldownController = brain.Get<PlacedItemCooldownController>();
    }

    public void AssignToQueue(ItemQueueManager queue)
    {
        ownerQueue = queue;
        isQueueItem = true;
        isOnGrid = false;

        // Requirement: when returned to queue, cooldown must reset and overlay must be hidden.
        cooldownController?.ResetToQueue();
    }

    public IReadOnlyList<Vector2Int> ShapeCells
        => definition != null ? definition.ShapeCells : new[] { Vector2Int.zero };

    public RectInt ShapeBounds
        => definition != null ? definition.GetShapeBounds() : new RectInt(0, 0, 1, 1);

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Keep top-left pivot/anchors for grid alignment.
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
        ApplyCooldown();
        ApplySize();

        // If this instance starts in queue, ensure cooldown is idle.
        if (isQueueItem)
            cooldownController?.ResetToQueue();
    }

    private void ApplyCooldown()
    {
        if (cooldownController == null) return;
        if (definition == null) return;

        cooldownController.SetCooldownSeconds(definition.CooldownSeconds);
    }

    private void ApplyVisual()
    {
        if (itemView == null) return;
        if (definition == null) return;

        if (definition.Icon != null)
            itemView.SetIcons(definition.Icon);
    }

    private void ApplySize()
    {
        if (GridManager.Instance == null || itemView == null) return;

        float size = GridManager.Instance.CellSize;
        float space = GridManager.Instance.Spacing;

        RectInt bounds = ShapeBounds;

        float w = bounds.width * size + (bounds.width - 1) * space;
        float h = bounds.height * size + (bounds.height - 1) * space;

        itemView.SetImageSizes(w, h);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GridManager.Instance == null) return;

        // Requirement: when held in hand, cooldown must pause (not reset).
        cooldownController?.PauseCooldown();

        isDragging = true;
        uiCamera = eventData.pressEventCamera;

        originalParent = rectTransform.parent;
        originalAnchoredPos = rectTransform.anchoredPosition;

        // New: compute grabbed cell offset inside the item (so placement aligns correctly)
        ComputeGrabbedCellOffset(eventData.position);

        if (isOnGrid)
            GridManager.Instance.Clear(this);

        Vector3 pointerWorld = GetPointerWorld(eventData.position);
        worldDragOffset = rectTransform.position - pointerWorld;

        rectTransform.SetParent(GridManager.Instance.DragLayerRoot, worldPositionStays: true);
        rectTransform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;

        if (GridManager.Instance.TryGetCellIndex(eventData.position, uiCamera, out int x, out int y))
        {
            ApplyPlacementPreviewWithGrabOffset(x, y);
        }
        else
        {
            GridManager.Instance.ClearPlacementPreview();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector3 pointerWorld = GetPointerWorld(eventData.position);
        rectTransform.position = pointerWorld + worldDragOffset;

        if (GridManager.Instance.TryGetCellIndex(eventData.position, uiCamera, out int x, out int y))
        {
            ApplyPlacementPreviewWithGrabOffset(x, y);
        }
        else
        {
            GridManager.Instance.ClearPlacementPreview();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;

        ItemQueueManager queue = ownerQueue != null ? ownerQueue : FindFirstObjectByType<ItemQueueManager>();

        bool isOverGrid = GridManager.Instance.TryGetCellIndex(eventData.position, uiCamera, out int x, out int y);
        int placeX = x;
        int placeY = y;

        if (isOverGrid)
        {
            ApplyGrabOffsetToPlacement(ref placeX, ref placeY);
        }

        if (isOverGrid && GridManager.Instance.CanPlace(this, placeX, placeY))
        {
            rectTransform.SetParent(GridManager.Instance.PlacedItemsRoot, worldPositionStays: false);
            rectTransform.anchoredPosition = GridManager.Instance.GetItemAnchoredPosition(placeX, placeY);

            GridManager.Instance.TryPlaceWithKick(this, placeX, placeY, queue);

            // Requirement: when dropped back to grid, resume from where it left off.
            // Only start from 0 the very first time it gets placed.
            if (cooldownController != null)
            {
                if (cooldownController.HasStartedOnce)
                    cooldownController.ResumeCooldown();
                else
                    cooldownController.StartCooldown();
            }

            if (isQueueItem)
            {
                isQueueItem = false;
                ownerQueue?.NotifyConsumedFromQueue(this);
            }

            queue?.RefreshLayout();
        }
        else
        {
            if (queue != null)
            {
                // Returning to queue: reset cooldown and hide overlay.
                queue.ReturnToQueue(this);
                cooldownController?.ResetToQueue();
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

        hasGrabbedCellOffset = false;
        grabbedCellOffset = Vector2Int.zero;
    }

    private void ApplyPlacementPreviewWithGrabOffset(int gridX, int gridY)
    {
        int x = gridX;
        int y = gridY;

        ApplyGrabOffsetToPlacement(ref x, ref y);

        GridManager.Instance.ShowPlacementPreview(this, x, y);
    }

    private void ApplyGrabOffsetToPlacement(ref int x, ref int y)
    {
        if (!hasGrabbedCellOffset)
            return;

        x -= grabbedCellOffset.x;
        y -= grabbedCellOffset.y;
    }

    private void ComputeGrabbedCellOffset(Vector2 screenPos)
    {
        hasGrabbedCellOffset = false;
        grabbedCellOffset = Vector2Int.zero;

        if (GridManager.Instance == null)
            return;

        RectInt bounds = ShapeBounds;
        if (bounds.width <= 0 || bounds.height <= 0)
            return;

        // Determine pointer position inside this RectTransform (local space).
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPos, uiCamera, out Vector2 local))
            return;

        // With pivot (0,1): local.x goes [0..width], local.y goes [0..-height]
        float px = local.x;
        float py = -local.y;

        float step = GridManager.Instance.CellSize + GridManager.Instance.Spacing;
        if (step <= 0.0001f)
            return;

        int approxX = Mathf.FloorToInt(px / step);
        int approxY = Mathf.FloorToInt(py / step);

        approxX = Mathf.Clamp(approxX, 0, bounds.width - 1);
        approxY = Mathf.Clamp(approxY, 0, bounds.height - 1);

        // If shape is not a full rectangle, ensure we pick an occupied cell.
        // We select the closest occupied cell (Manhattan distance) in the shape.
        Vector2Int best = new Vector2Int(approxX, approxY);
        int bestDist = int.MaxValue;

        IReadOnlyList<Vector2Int> cells = ShapeCells;
        if (cells == null || cells.Count == 0)
        {
            grabbedCellOffset = best;
            hasGrabbedCellOffset = true;
            return;
        }

        for (int i = 0; i < cells.Count; i++)
        {
            Vector2Int c = cells[i];
            // Convert shape cell (in bounds space) to local index relative to bounds top-left.
            int lx = c.x - bounds.x;
            int ly = c.y - bounds.y;

            int dist = Mathf.Abs(lx - approxX) + Mathf.Abs(ly - approxY);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = new Vector2Int(lx, ly);
                if (bestDist == 0)
                    break;
            }
        }

        grabbedCellOffset = best;
        hasGrabbedCellOffset = true;
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
