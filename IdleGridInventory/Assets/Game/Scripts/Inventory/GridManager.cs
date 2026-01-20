using OmniGameTemplate.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class GridManager : MonoSingleton<GridManager>
{
    [Header("Grid Size")]
    [SerializeField] private int columns = 8;
    [SerializeField] private int rows = 6;

    [Header("Cell Layout (UI)")]
    [SerializeField] private float cellSize = 64f;
    [SerializeField] private float spacing = 6f;

    [Header("Grid Padding")]
    [SerializeField] private float paddingLeft = 6f;
    [SerializeField] private float paddingTop = 6f;

    [Header("References")]
    [SerializeField] private RectTransform gridRoot;
    [SerializeField] private RectTransform gridCellsRoot;
    [SerializeField] private RectTransform placedItemsRoot;
    [SerializeField] private RectTransform dragLayerRoot;
    [SerializeField] private GameObject cellPrefab;

    [Header("Placement Preview")]
    [SerializeField] private Color previewValidColor = new Color(0.2f, 1f, 0.2f, 1f);
    [SerializeField] private Color previewInvalidColor = new Color(1f, 0.2f, 0.2f, 1f);

    public int Columns => columns;
    public int Rows => rows;
    public float CellSize => cellSize;
    public float Spacing => spacing;

    public RectTransform GridRoot => gridRoot;
    public RectTransform PlacedItemsRoot => placedItemsRoot;
    public RectTransform DragLayerRoot => dragLayerRoot;

    public event Action<DraggableItem> ItemPlaced;

    private bool[,] occupied;
    private readonly Dictionary<DraggableItem, List<Vector2Int>> itemCells = new();

    // Preview data
    private Image[,] cellImages;
    private readonly List<Vector2Int> previewCells = new();
    private bool previewActive;

    private void Awake()
    {
        if (gridRoot == null)
            gridRoot = transform as RectTransform;

        occupied = new bool[columns, rows];
        cellImages = new Image[columns, rows];

        GenerateGridVisual();
    }

    private void GenerateGridVisual()
    {
        if (gridCellsRoot == null)
        {
            Debug.LogError("[GridManager] Missing references: gridCellsRoot");
            return;
        }
        else if (cellPrefab == null)
        {
            Debug.LogError("[GridManager] Missing references: cellPrefab");
            return;
        }

        ClearChildren(gridCellsRoot);

        Vector2 gridPixelSize = GetGridPixelSize();
        gridRoot.sizeDelta = gridPixelSize;
        gridCellsRoot.sizeDelta = gridPixelSize;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                GameObject cell = Instantiate(cellPrefab, gridCellsRoot);
                cell.name = $"Cell_{x}_{y}";

                RectTransform rt = cell.GetComponent<RectTransform>();
                if (rt == null) rt = cell.AddComponent<RectTransform>();

                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 1f);

                rt.sizeDelta = new Vector2(cellSize, cellSize);
                rt.anchoredPosition = GetCellAnchoredPosition(x, y);

                Image img = cell.GetComponent<Image>();
                if (img != null)
                    cellImages[x, y] = img;
            }
        }
    }

    private static void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    public Vector2 GetGridPixelSize()
    {
        float width = columns * cellSize + (columns - 1) * spacing + paddingLeft * 2f;
        float height = rows * cellSize + (rows - 1) * spacing + paddingTop * 2f;
        return new Vector2(width, height);
    }

    public Vector2 GetCellAnchoredPosition(int x, int y)
    {
        float px = paddingLeft + x * (cellSize + spacing);
        float py = -paddingTop - y * (cellSize + spacing);
        return new Vector2(px, py);
    }

    public Vector2 GetItemAnchoredPosition(int startX, int startY)
    {
        return GetCellAnchoredPosition(startX, startY);
    }

    public bool CanPlace(DraggableItem item, int startX, int startY)
    {
        if (item == null) return false;
        if (startX < 0 || startY < 0) return false;

        IReadOnlyList<Vector2Int> shape = item.ShapeCells;
        if (shape == null || shape.Count == 0) return false;

        for (int i = 0; i < shape.Count; i++)
        {
            Vector2Int c = shape[i];

            int gx = startX + c.x;
            int gy = startY + c.y;

            if (gx < 0 || gx >= columns || gy < 0 || gy >= rows)
                return false;

            if (occupied[gx, gy])
                return false;
        }

        return true;
    }

    public void Place(DraggableItem item, int startX, int startY)
    {
        if (item == null) return;

        Clear(item);

        IReadOnlyList<Vector2Int> shape = item.ShapeCells;
        var cells = new List<Vector2Int>(shape.Count);

        for (int i = 0; i < shape.Count; i++)
        {
            Vector2Int c = shape[i];

            int gx = startX + c.x;
            int gy = startY + c.y;

            occupied[gx, gy] = true;
            cells.Add(new Vector2Int(gx, gy));
        }

        itemCells[item] = cells;
        item.SetGridPosition(startX, startY);

        ItemPlaced?.Invoke(item);
    }

    public void Clear(DraggableItem item)
    {
        if (item == null) return;

        if (!itemCells.TryGetValue(item, out List<Vector2Int> cells))
            return;

        for (int i = 0; i < cells.Count; i++)
        {
            Vector2Int c = cells[i];
            if (c.x >= 0 && c.x < columns && c.y >= 0 && c.y < rows)
                occupied[c.x, c.y] = false;
        }

        itemCells.Remove(item);
        item.ClearGridPosition();
    }

    public bool TryGetCellIndex(Vector2 screenPosition, Camera uiCamera, out int x, out int y)
    {
        x = -1;
        y = -1;

        if (gridRoot == null) return false;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridRoot, screenPosition, uiCamera, out Vector2 localPoint))
            return false;

        float full = cellSize + spacing;

        float adjustedX = localPoint.x - paddingLeft;
        float adjustedY = (-localPoint.y) - paddingTop;

        int cx = Mathf.FloorToInt(adjustedX / full);
        int cy = Mathf.FloorToInt(adjustedY / full);

        if (cx < 0 || cx >= columns || cy < 0 || cy >= rows)
            return false;

        float insideX = adjustedX - cx * full;
        float insideY = adjustedY - cy * full;

        if (insideX > cellSize || insideY > cellSize)
            return false;

        x = cx;
        y = cy;
        return true;
    }

    // ---------------------------
    // Placement Preview API
    // ---------------------------

    public void ShowPlacementPreview(DraggableItem item, int startX, int startY)
    {
        if (item == null)
        {
            ClearPlacementPreview();
            return;
        }

        IReadOnlyList<Vector2Int> shape = item.ShapeCells;
        if (shape == null || shape.Count == 0)
        {
            ClearPlacementPreview();
            return;
        }

        bool valid = CanPlace(item, startX, startY);

        ClearPlacementPreviewInternal();

        Color color = valid ? previewValidColor : previewInvalidColor;

        for (int i = 0; i < shape.Count; i++)
        {
            Vector2Int c = shape[i];
            int gx = startX + c.x;
            int gy = startY + c.y;

            if (gx < 0 || gx >= columns || gy < 0 || gy >= rows)
                continue;

            Image img = cellImages[gx, gy];
            if (img == null) continue;

            img.color = color;
            previewCells.Add(new Vector2Int(gx, gy));
        }

        previewActive = previewCells.Count > 0;
    }

    public void ClearPlacementPreview()
    {
        ClearPlacementPreviewInternal();
        previewActive = false;
    }

    private void ClearPlacementPreviewInternal()
    {
        if (!previewActive && previewCells.Count == 0)
            return;

        for (int i = 0; i < previewCells.Count; i++)
        {
            Vector2Int c = previewCells[i];
            Image img = cellImages[c.x, c.y];
            if (img == null) continue;

            // Reset to prefab default by setting to white (tintless).
            // Keep your base cell color in the prefab (Image color),
            // and use a child overlay if you want perfect reset.
            img.color = Color.white;
        }

        previewCells.Clear();
    }
}
