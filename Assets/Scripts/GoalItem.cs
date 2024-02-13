using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalItem : Part
{
    public GoalItem(Match3ItemType type, bool broken, int goal)
    {
        itemType = type;
        this.broken = broken;
        Goal = goal;

    }
    public int goal;
    public TMPro.TMP_Text goalText;

    private int _goal;
    public int Goal
    {
        get { return _goal; }
        set
        {
            _goal = value;
            goalText.text = value.ToString();
        }
    }
}
