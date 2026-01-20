using OmniGameTemplate.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        if (gridRoot == null)
            gridRoot = transform as RectTransform;

        occupied = new bool[columns, rows];
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

        int w = item.Width;
        int h = item.Height;

        if (w <= 0 || h <= 0) return false;
        if (startX < 0 || startY < 0) return false;
        if (startX + w > columns) return false;
        if (startY + h > rows) return false;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int gx = startX + x;
                int gy = startY + y;

                if (occupied[gx, gy])
                    return false;
            }
        }

        return true;
    }

    public void Place(DraggableItem item, int startX, int startY)
    {
        if (item == null) return;

        Clear(item);

        int w = item.Width;
        int h = item.Height;

        var cells = new List<Vector2Int>(w * h);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int gx = startX + x;
                int gy = startY + y;

                occupied[gx, gy] = true;
                cells.Add(new Vector2Int(gx, gy));
            }
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
}
