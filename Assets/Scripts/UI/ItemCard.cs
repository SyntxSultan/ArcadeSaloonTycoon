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
    private ItemSO itemSO;
    
    private void Start()
    {
        itemButton = GetComponent<Button>();
        itemButton.onClick.AddListener(OnClickedItem);
    }

    public void InitializeItemCard(ItemSO itemSO)
    {
        this.itemSO = itemSO;
        icon.sprite = itemSO.icon;
        nameText.text = itemSO.itemName;
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
        ScreenManager.Instance.OpenItemDetailsPopup(itemSO);
    }
}
