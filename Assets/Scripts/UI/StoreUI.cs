using UnityEngine;
using System.Collections.Generic;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private GameObject itemCardPrefab;
    [SerializeField] private GameObject machinesContentPanel;
    [SerializeField] private GameObject decorationsContentPanel;
    [SerializeField] private ItemDatabase itemDB;

    private bool initialized;
    private readonly Dictionary<ItemCategory, Transform> contentPanels = new Dictionary<ItemCategory, Transform>();

    private void Awake()
    {
        InitializeContentPanels();
    }

    private void InitializeContentPanels()
    {
        contentPanels[ItemCategory.Machine] = machinesContentPanel.transform;
        contentPanels[ItemCategory.Decoration] = decorationsContentPanel.transform;
    }

    private void OnEnable()
    {
        if (!initialized)
        {
            CreateAllItems(itemDB.GetItemList());
        }
    }

    private void CreateAllItems(ItemSO[] storeData)
    {
        ClearAllPanels();
        
        foreach (ItemSO item in storeData)
        {
            if (contentPanels.TryGetValue(item.category, out Transform contentPanel))
            {
                CreateItemCard(item, contentPanel);
            }
            else
            {
                Debug.LogWarning($"Bilinmeyen kategori: {item.category}");
            }
        }
        
        initialized = true;
    }

    private void CreateItemCard(ItemSO item, Transform parent)
    {
        GameObject instanceItem = Instantiate(itemCardPrefab, parent);
        instanceItem.GetComponent<ItemCard>().InitializeItemCard(item);
    }

    private void ClearAllPanels()
    {
        foreach (var panel in contentPanels.Values)
        {
            ClearChildren(panel);
        }
    }

    private void ClearChildren(Transform parent)
    {
        int childCount = parent.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
