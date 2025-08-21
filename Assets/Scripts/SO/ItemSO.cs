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
    public string itemName;
    public Sprite icon;
    public int cost;
    public int starAmount;
    public GameObject prefab;
    public Vector2Int size;
    public ItemCategory category;
    public BuildableGridObjectTypeSO gridItemData;
}

