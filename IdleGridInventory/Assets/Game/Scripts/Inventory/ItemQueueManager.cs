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

    private void Start()
    {
        SpawnNewBatch(clearConsumedCounter: true);
    }

    public void NotifyConsumedFromQueue(DraggableItem item)
    {
        if (item == null) return;

        activeQueueItems.Remove(item);
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
        // Refresh only the items still in the queue area.
        // Items already placed on the grid must not be affected.
        SpawnNewBatch(clearConsumedCounter: true);
    }

    private void SpawnNewBatch(bool clearConsumedCounter)
    {
        if (clearConsumedCounter)
            consumedSinceLastSpawn = 0;

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
        // Destroy only items still under the queue root.
        // Items moved to the grid should never be destroyed here.
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
