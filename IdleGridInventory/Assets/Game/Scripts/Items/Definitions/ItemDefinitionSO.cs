using System.Collections.Generic;
using UnityEngine;

public abstract class ItemDefinitionSO : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string itemId = "item_id";
    [SerializeField] private string displayName = "Item";

    [Header("Visual")]
    [SerializeField] private Sprite icon;

    [Header("Shape (Cells)")]
    [Tooltip("Local occupied cells. Use non-negative coordinates. Example: (0,0) is top-left.")]
    [SerializeField] private Vector2Int[] shapeCells = new[] { Vector2Int.zero };

    public string ItemId => itemId;
    public string DisplayName => displayName;
    public Sprite Icon => icon;

    public IReadOnlyList<Vector2Int> ShapeCells => shapeCells;

    public RectInt GetShapeBounds()
    {
        if (shapeCells == null || shapeCells.Length == 0)
            return new RectInt(0, 0, 1, 1);

        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;

        for (int i = 0; i < shapeCells.Length; i++)
        {
            Vector2Int c = shapeCells[i];
            if (c.x < minX) minX = c.x;
            if (c.y < minY) minY = c.y;
            if (c.x > maxX) maxX = c.x;
            if (c.y > maxY) maxY = c.y;
        }

        int w = (maxX - minX) + 1;
        int h = (maxY - minY) + 1;
        return new RectInt(minX, minY, w, h);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (shapeCells == null || shapeCells.Length == 0)
            shapeCells = new[] { Vector2Int.zero };

        // Keep data safe: avoid negative by default (you can remove this if you need negatives later).
        for (int i = 0; i < shapeCells.Length; i++)
        {
            if (shapeCells[i].x < 0) shapeCells[i].x = 0;
            if (shapeCells[i].y < 0) shapeCells[i].y = 0;
        }
    }
#endif
}
