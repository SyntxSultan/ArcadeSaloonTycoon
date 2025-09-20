using System;
using UnityEngine;
using UnityEngine.UI;

public class OfflineProgressUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private Button useCoinTo3xButton;
    [SerializeField] private TMPro.TextMeshProUGUI incomeText;

    private void Start()
    {
        closeButton.onClick.AddListener(ScreenManager.Instance.CloseOfflineIncomeUI);
    }
}
