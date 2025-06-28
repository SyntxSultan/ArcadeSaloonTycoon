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

    public void InitializeItemCard(Sprite icon, string name, float cost, int starAmount)
    {
        this.icon.sprite = icon;
        nameText.text = name;
        costText.text = $"${cost.ToString()}";
        foreach (Transform child in starContainer.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < starAmount; i++)
        {
            Instantiate(starPrefab, starContainer.transform);
        }
    }
}
