using System;
using UnityEngine;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private GameObject itemCardPrefab;
    [SerializeField] private GameObject machinesContentPanel;
    [SerializeField] private GameObject decorationsContentPanel;

    [SerializeField] private ItemDatabase itemDB;

    private void OnEnable()
    {
        ListAllItems(itemDB.GetItemList());
    }

    public void ListAllItems(ItemSO[] storeData)
    {
        ClearAllChildren(machinesContentPanel.transform);
        ClearAllChildren(decorationsContentPanel.transform);
        
        foreach (ItemSO item in storeData)
        {
            Transform contentPanel = item.category == ItemCategory.Machine ? machinesContentPanel.transform : decorationsContentPanel.transform;
            GameObject instanceItem = Instantiate(itemCardPrefab, contentPanel);
            instanceItem.GetComponent<ItemCard>().InitializeItemCard(item);
        }
    }

    private void ClearAllChildren(Transform parent)
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
