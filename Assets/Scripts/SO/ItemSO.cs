
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public float cost;
    public int starAmount;
    public GameObject prefab;
    public Vector2Int size;
}

public enum ItemCategory
{
    Machine,
    Decoration
}