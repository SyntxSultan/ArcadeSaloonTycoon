using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "Scriptable Objects/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] public List<ItemSO> items = new List<ItemSO>();

    public List<ItemSO> GetItemList()
    {
        return items;
    }
}