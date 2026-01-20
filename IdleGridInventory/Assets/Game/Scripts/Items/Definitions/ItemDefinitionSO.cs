using UnityEngine;

public abstract class ItemDefinitionSO : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string itemId = "item_id";
    [SerializeField] private string displayName = "Item";

    [Header("Visual")]
    [SerializeField] private Sprite icon;

    [Header("Shape (Cells)")]
    [Min(1)]
    [SerializeField] private int width = 1;

    [Min(1)]
    [SerializeField] private int height = 1;

    public string ItemId => itemId;
    public string DisplayName => displayName;
    public Sprite Icon => icon;

    public int Width => width;
    public int Height => height;
}
