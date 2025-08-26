using UnityEngine;

[CreateAssetMenu(fileName = "ItemUpgradeSO", menuName = "Scriptable Objects/ItemUpgradeSO")]
public class ItemUpgradeSO : ScriptableObject
{
    public string upgradeName;
    public Sprite upgradeIcon;
    public string upgradeDescription;
    public int upgradeCost;
    public int upgradeValue; 
    public UpgradeType upgradeType;
}

[System.Serializable]
public enum UpgradeType
{
    CoinPerUse,
    Durability,
    OfflineIncome
}
