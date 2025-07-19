using UnityEngine;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private GameObject itemCardPrefab;
    [SerializeField] private GameObject contentPanel;

    public void ListAllItems(ItemSO[] storeData)
    {
        //TODO: Separate items based on category
        foreach (Transform child in contentPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (ItemSO item in storeData)
        {
            GameObject instanceItem = Instantiate(itemCardPrefab, contentPanel.transform);
            instanceItem.GetComponent<ItemCard>().InitializeItemCard(item.icon, item.name, item.cost, item.starAmount);
        }
    }
}
