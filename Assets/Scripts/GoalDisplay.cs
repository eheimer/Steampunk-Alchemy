using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// a part that has a goal in the form of an integer
// this is what will go on the GoalDisplay prefab, instead of GoalItem
public class GoalDisplay : Part
{
    [SerializeField] private TMP_Text goalText;

    public void Init(Match3ItemType type, bool broken, int goal)
    {
        base.Init(type, broken);
        goalText.text = goal.ToString();
    }

    public int Goal
    {
        get { return int.Parse(goalText.text); }
        set
        {
            if (value < 0) value = 0;
            goalText.text = value.ToString();
        }
    }
}
