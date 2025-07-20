using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : ScriptableObject
{
    [SerializeField] private ItemSO[] items;

    public ItemSO[] GetItemList()
    {
        return items;
    }
}