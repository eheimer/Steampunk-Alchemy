using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3Item
{
    public Match3Item(Match3ItemType itemType, bool broken)
    {
        ItemType = itemType;
        Broken = broken;

    }

    private Match3ItemType _itemType;
    public Match3ItemType ItemType
    {
        get { return _itemType; }
        set
        {
            _itemType = value;
        }
    }

    private bool _broken;
    public bool Broken
    {
        get { return _broken; }
        set
        {
            _broken = value;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (Match3Item)obj;
        return ItemType == other.ItemType && Broken == other.Broken;
    }

    public override int GetHashCode()
    {
        return System.Tuple.Create(ItemType, Broken).GetHashCode();
    }
}
