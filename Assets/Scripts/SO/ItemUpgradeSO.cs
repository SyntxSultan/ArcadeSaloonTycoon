using UnityEngine;

[CreateAssetMenu(fileName = "ItemUpgradeSO", menuName = "Scriptable Objects/ItemUpgradeSO")]
public class ItemUpgradeSO : ScriptableObject
{
    public int upgradeCost;
    public UpgradeType upgradeType;
}

[System.Serializable]
public enum UpgradeType
{
    
}
