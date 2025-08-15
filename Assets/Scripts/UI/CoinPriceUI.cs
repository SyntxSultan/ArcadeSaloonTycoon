using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinPriceUI : MonoBehaviour
{
    [SerializeField] private int minCoinPrice = 1;
    [SerializeField] private int maxCoinPrice = 20;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI sliderValueText;

    private void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnEnable()
    {
        slider.value = CurrencyManager.Instance.GetMoneyPerCoin();
        OnSliderValueChanged(CurrencyManager.Instance.GetMoneyPerCoin());
    }

    private void OnSliderValueChanged(float value)
    {
        sliderValueText.text = $"{value.ToString()}";
        CurrencyManager.Instance.SetMoneyPerCoin((int)value);
        if (AudioManager.Instance) AudioManager.Instance.PlaySFX(SFX.UIClick);
    }
}
