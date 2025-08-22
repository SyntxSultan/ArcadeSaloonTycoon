using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCard : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject starContainer;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject emptyStarPrefab;

    private Button itemButton;
    private ItemSO cachedItemSO;
    
    private void Start()
    {
        itemButton = GetComponent<Button>();
        itemButton.onClick.AddListener(OnClickedItem);
    }

    public void InitializeItemCard(ItemSO itemSO)
    {
        cachedItemSO = itemSO;
        icon.sprite = itemSO.gridItemData.objectIcon;
        nameText.text = itemSO.gridItemData.objectName;
        costText.text = $"${itemSO.cost.ToString()}";
        foreach (Transform child in starContainer.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < itemSO.starAmount; i++)
        {
            Instantiate(starPrefab, starContainer.transform);
        }
        for (int i = 0; i < 5 - itemSO.starAmount; i++)
        {
            Instantiate(emptyStarPrefab, starContainer.transform);
        }
    }

    private void OnClickedItem()
    {
        ScreenManager.Instance.OpenItemDetailsPopup(cachedItemSO);
    }
}
