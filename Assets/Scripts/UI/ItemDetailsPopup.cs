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
    [SerializeField] private Button buyButton;
    [SerializeField] private Transform previewItemSpawnPoint;
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject emptyStarPrefab;

    private ItemSO itemSO;
    
    public void SetItemDetails(ItemSO item)
    {
        itemSO = item;
        MachineItemSO machineSO = item as MachineItemSO;
        if (machineSO == null)
        {
            Debug.LogError("Gelen item bir makine değil");
            return;
        }
        
        SpawnStars();
        itemNameText.text = item.itemName;
        maxCoinCapacityText.text = "Max Coin Capacity: " +machineSO.maxCoinCapacity.ToString();
        coinPerUseText.text = "Coin Per Use: " +machineSO.coinPerUse.ToString();
        durabilityText.text = "Durability: " +machineSO.durability.ToString();
        categoryText.text = "Category: " +machineSO.arcadeCategory.ToString();
        playTimeText.text = "Play Time: " + machineSO.playTime.ToString() + "sec";
        sizeText.text = $"Size: {machineSO.size.x}x{machineSO.size.y}y";
        priceText.text = $"${item.cost.ToString()}";
        
        buyButton.onClick.AddListener(OnBuyButtonClicked);
        SpawnPreviewPrefab(machineSO.prefab);
    }

    private void SpawnPreviewPrefab(GameObject prefab)
    {
        
        PreviewSpawner.Instance.SpawnPreview(prefab);
        /*
        if (prefab == null)
        {
            Debug.LogWarning($"Prefab null: {itemSO.name}");
            return;
        }
        
        void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
                SetLayerRecursively(child.gameObject, layer);
        }
        
        GameObject instantiatedPrefab = Instantiate(prefab, previewItemSpawnPoint);
        SetLayerRecursively(instantiatedPrefab, LayerMask.NameToLayer("PreviewLayer"));
        */
        //instantiatedPrefab.layer = LayerMask.NameToLayer("PreviewLayer");
    }

    private void OnBuyButtonClicked()
    {
        Debug.LogError($"Buy: {itemNameText.text}");
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
