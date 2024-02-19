using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoalDisplay : Part
{
    [SerializeField] private TMP_Text goalText;

    public void Init(Match3Item type, int goal)
    {
        base.Init(type);
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
