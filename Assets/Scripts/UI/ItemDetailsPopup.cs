using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailsPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Transform starsParent;
    [SerializeField] private TextMeshProUGUI maxCoinCapacityText;
    [SerializeField] private TextMeshProUGUI coinPerUseText;
    [SerializeField] private TextMeshProUGUI durabilityText;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private TextMeshProUGUI playTimeText;
    [SerializeField] private TextMeshProUGUI sizeText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Color canBuyColor;
    [SerializeField] private Color cantBuyColor;
    [SerializeField] private Button buyButton;
    [SerializeField] private Transform previewItemSpawnPoint;
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject emptyStarPrefab;

    private ItemSO itemSO;
    
    public void SetItemDetails(ItemSO item)
    {
        itemSO = item;
        
        switch (item.category)
        {
            case ItemCategory.Machine:
                MachineItemSO machineSO = item as MachineItemSO;
                maxCoinCapacityText.text = "Max Coin Capacity: " + machineSO.maxCoinCapacity.ToString();
                coinPerUseText.text = "Coin Per Use: "           + machineSO.coinPerUse.ToString();
                durabilityText.text = "Durability: "             + machineSO.durability.ToString();
                categoryText.text = "Category: "                 + machineSO.arcadeCategory.ToString();
                playTimeText.text = "Play Time: "                + machineSO.playTime.ToString() + "sec";
                break;
            case ItemCategory.Decoration:
                maxCoinCapacityText.text = "";
                coinPerUseText.text = "";
                durabilityText.text = "";
                categoryText.text = "";
                playTimeText.text = "";
                break;
            case ItemCategory.Automation:
                break;
        }

        SpawnStars();
        itemNameText.text = item.gridItemData.objectName;
        sizeText.text = $"Size: {item.size.x}x{item.size.y}y";
        CheckMoneyAndSetUI(item);
        buyButton.onClick.AddListener(OnBuyButtonClicked);
        SpawnPreviewPrefab(item.gridItemData.ghostPrefab.gameObject);
    }

    private void CheckMoneyAndSetUI(ItemSO item)
    {
        if (CurrencyManager.Instance.CanBuy(item.cost))
        {
            priceText.text = $"${item.cost.ToString()}";
            priceText.color = canBuyColor;
            buyButton.interactable = true;
        }
        else
        {
            priceText.text = $"${item.cost.ToString()}";
            priceText.color = cantBuyColor;
            buyButton.interactable = false;
        }
    }

    private void SpawnPreviewPrefab(GameObject prefab)
    {
        PreviewSpawner.Instance.SpawnPreview(prefab);
    }

    private void OnBuyButtonClicked()
    {
        GridBridge.Instance.OnBuildableItemBought(itemSO.gridItemData);
        CurrencyManager.Instance.SetPlacingItemForMoneyDraw(itemSO);
        ScreenManager.Instance.CloseItemDetailsPopup();
        ScreenManager.Instance.ShowBuildingUI();
    }

    private void SpawnStars()
    {
        foreach (Transform child in starsParent.transform)
        {
            Destroy(child.gameObject);      
        }

        for (int i = 0; i < itemSO.starAmount; i++)
        {
            Instantiate(starPrefab, starsParent.transform);
        }
        for (int i = 0; i < 5 - itemSO.starAmount; i++)
        {
            Instantiate(emptyStarPrefab, starsParent.transform);
        }
    }
}
