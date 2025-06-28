using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyTextbox;
    [SerializeField] private TextMeshProUGUI coinTextbox;
    
    public void UpdateMoneyText(float money)
    {
        moneyTextbox.text = $"{money.ToString()}";
    }
    public void UpdateCoinText(float coin)
    {
        coinTextbox.text = $"{coin.ToString()}";
    }
}
