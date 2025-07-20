using UnityEngine;

public enum ArcadeCategory
{
    Racing,
    Fighter
}

[CreateAssetMenu(fileName = "MachineData", menuName = "Scriptable Objects/MachineData")]
public class MachineItemSO : ItemSO
{
    public int maxCoinCapacity;
    public int coinPerUse;
    public int durability;
    public ArcadeCategory arcadeCategory;
    public int playTime;
}

