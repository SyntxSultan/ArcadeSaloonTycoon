using SoulGames.EasyGridBuilderPro;
using UnityEngine;

public enum ItemCategory
{
    Machine,
    Decoration,
    Automation
}

public class ItemSO : ScriptableObject
{
    public int cost;
    public int starAmount;
    public Vector2Int size;
    public ItemCategory category;
    public BuildableGridObjectTypeSO gridItemData;
}

