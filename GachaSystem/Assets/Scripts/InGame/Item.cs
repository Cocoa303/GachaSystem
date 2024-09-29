using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ItemType = global::Common.Enum.GachaRewardItemType;

[System.Serializable]
public class Item
{
    [SerializeField] protected Data.ItemList data;
    [SerializeField] protected int hasCount;
    [SerializeField] protected int level;
    [SerializeField] protected ItemType type;

    public Data.ItemList Data { get => data; set=> data = value; }
    public int HasCount { get => hasCount; set => hasCount = value; }
    public int Level { get => level; set => level = value; }
    public ItemType Type { get => type; set=> type = value; }

    public Item() { }
    public Item(Data.ItemList data,int hasCount, int level, ItemType type)
    {
        this.data = data;
        this.hasCount = hasCount;
        this.level = level;
        this.type = type;
    }
}
