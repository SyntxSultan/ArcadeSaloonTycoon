using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image canUpgradeIcon;
    [SerializeField] private TextMeshProUGUI upgradeLevelText;

    public void Initialize(ItemUpgradeSO upgradeSO, int level, bool canUpgrade, System.Action<ItemUpgradeSO> callback)
    {
        SetCanUpgrade(canUpgrade);
        icon.sprite = upgradeSO.upgradeIcon;
        upgradeLevelText.text = level.ToString();
        GetComponent<Button>().onClick.AddListener(() => { callback?.Invoke(upgradeSO); });
    }

    public void SetCanUpgrade(bool canUpgrade)
    {
        canUpgradeIcon.gameObject.SetActive(canUpgrade);
    }
}
