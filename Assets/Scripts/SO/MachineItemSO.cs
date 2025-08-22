using UnityEngine;

[CreateAssetMenu(fileName = "MachineData", menuName = "Scriptable Objects/MachineData")]
public class MachineItemSO : ItemSO
{
    public int maxCoinCapacity;
    /// <summary>
    /// This affects how many coins will be used from customer per playTime.
    /// This affects the income of machines.
    /// </summary>
    public int coinPerUse;
    /// <summary>
    /// This affects the failure rate of machines.
    /// </summary>
    public int durability;
    public string arcadeCategory;
    /// <summary>
    /// This determines how long can customer play this machine.
    /// </summary>
    public int playTime;
    public int offlineIncome;
}

