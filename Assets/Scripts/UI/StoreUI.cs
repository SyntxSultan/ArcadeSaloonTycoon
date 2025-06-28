using UnityEngine;
using UnityEngine.UI;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private Button storeButton;
    [SerializeField] private Button closeStorePanelButton;
    [SerializeField] private GameObject storePanel;
    
    [SerializeField] private GameObject itemCardPrefab;
    [SerializeField] private GameObject contentPanel;
    
    
    void Start()
    {
        storeButton.onClick.AddListener(ToggleStore);
        closeStorePanelButton.onClick.AddListener(ToggleStore);
    }

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

    public void ToggleStore()
    {
        if (storePanel.activeSelf)
        {
            storePanel.SetActive(false);
        }
        else
        {
            storePanel.SetActive(true);
        }
    }
    
}
