using System.Collections.Generic;
using UnityEngine;

public sealed class ItemQueueManager : MonoBehaviour
{
    [Header("Queue Setup")]
    [SerializeField] private RectTransform queueItemsRoot;
    [SerializeField] private DraggableItem itemPrefab;

    [Header("Pool (Weapon Definitions)")]
    [SerializeField] private List<WeaponDefinitionSO> weaponPool = new();

    [Header("Rules")]
    [SerializeField] private int spawnCount = 3;

    private readonly List<DraggableItem> activeQueueItems = new();
    private int consumedSinceLastSpawn;

    // Tracks which items were consumed from the current batch.
    // If one of them is returned back to the queue, we decrement the counter to prevent early respawn.
    private readonly HashSet<DraggableItem> consumedThisBatch = new();

    public RectTransform QueueItemsRoot => queueItemsRoot;

    private void Start()
    {
        SpawnNewBatch(clearConsumedCounter: true);
    }

    public void NotifyConsumedFromQueue(DraggableItem item)
    {
        if (item == null) return;

        activeQueueItems.Remove(item);

        // Only count once per item for this batch.
        if (!consumedThisBatch.Add(item))
            return;

        consumedSinceLastSpawn++;

        if (consumedSinceLastSpawn >= spawnCount)
        {
            SpawnNewBatch(clearConsumedCounter: true);
        }
        else
        {
            LayoutQueueItems();
        }
    }

    public void RerollQueue()
    {
        SpawnNewBatch(clearConsumedCounter: true);
    }

    public void ReturnToQueue(DraggableItem item)
    {
        if (item == null) return;
        if (queueItemsRoot == null) return;

        item.AssignToQueue(this);
        item.transform.SetParent(queueItemsRoot, worldPositionStays: false);

        if (!activeQueueItems.Contains(item))
            activeQueueItems.Add(item);

        // If this item was counted as "consumed" from the current batch, undo that consumption.
        if (consumedThisBatch.Remove(item))
        {
            consumedSinceLastSpawn = Mathf.Max(0, consumedSinceLastSpawn - 1);
        }

        LayoutQueueItems();
    }

    public void RefreshLayout()
    {
        LayoutQueueItems();
    }

    private void SpawnNewBatch(bool clearConsumedCounter)
    {
        if (clearConsumedCounter)
        {
            consumedSinceLastSpawn = 0;
            consumedThisBatch.Clear();
        }

        CleanupRemainingQueueVisuals();

        for (int i = 0; i < spawnCount; i++)
        {
            WeaponDefinitionSO def = GetRandomWeapon();
            if (def == null) break;

            DraggableItem instance = Instantiate(itemPrefab, queueItemsRoot);
            instance.name = $"QueueItem_{def.name}_{i}";
            instance.Initialize(def, isQueueItem: true, ownerQueue: this);

            activeQueueItems.Add(instance);
        }

        LayoutQueueItems();
    }

    private WeaponDefinitionSO GetRandomWeapon()
    {
        if (weaponPool == null || weaponPool.Count == 0)
        {
            Debug.LogError("[ItemQueueManager] weaponPool is empty.");
            return null;
        }

        int index = Random.Range(0, weaponPool.Count);
        return weaponPool[index];
    }

    private void CleanupRemainingQueueVisuals()
    {
        for (int i = activeQueueItems.Count - 1; i >= 0; i--)
        {
            DraggableItem item = activeQueueItems[i];
            if (item == null) continue;

            if (item.transform.parent == queueItemsRoot)
                Destroy(item.gameObject);
        }

        activeQueueItems.Clear();
    }

    private void LayoutQueueItems()
    {
        const float gap = 20f;

        float x = 0f;
        float y = 0f;

        for (int i = 0; i < activeQueueItems.Count; i++)
        {
            DraggableItem item = activeQueueItems[i];
            if (item == null) continue;

            if (item.transform.parent != queueItemsRoot)
                continue;

            RectTransform rt = item.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);

            rt.anchoredPosition = new Vector2(x, -y);
            x += rt.sizeDelta.x + gap;
        }
    }
}
