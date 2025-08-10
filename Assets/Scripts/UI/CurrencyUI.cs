using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyTextbox;
    [SerializeField] private TextMeshProUGUI coinTextbox;

    private void Start()
    {
        CurrencyManager.Instance.OnMoneyChanged += UpdateMoneyText;
        CurrencyManager.Instance.OnCoinChanged += UpdateCoinText;
    }

    private void UpdateMoneyText(int money)
    {
        moneyTextbox.text = $"{money.ToString()}";
    }
    private void UpdateCoinText(int coin)
    {
        coinTextbox.text = $"{coin.ToString()}";
    }
}
