using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTracker : MonoBehaviour
{
    [SerializeField] private GoalDisplay goalDisplayPrefab;

    [SerializeField] private GameObject scoreboardContainer;

    private Dictionary<GoalItem, GoalDisplay> goals = new Dictionary<GoalItem, GoalDisplay>();

    public void SetGoal(GoalItem goalItem, Vector3 location, int goal)
    {
        if (!goals.ContainsKey(goalItem))
        {
            GoalDisplay goalDisplay = Instantiate(goalDisplayPrefab, location, Quaternion.identity, transform);
            goalDisplay.Init(goalItem.ItemType, goalItem.Broken, goal);
            goals.Add(goalItem, goalDisplay);
        }
        else
        {
            goals[goalItem].Goal = goal;
        }
    }

    public void ReduceGoal(GoalItem goalItem, int count)
    {
        if (goals.ContainsKey(goalItem))
        {
            goals[goalItem].Goal -= count;
        }
    }

    public void ReduceGoal(Match3ItemType itemType, bool broken, int count)
    {
        GoalItem goalItem = new GoalItem(itemType, broken);
        ReduceGoal(goalItem, count);
    }

    public void ReduceGoal(Part part, int count)
    {
        ReduceGoal(part.itemType, part.broken, count);
    }

    public bool AllGoalsMet()
    {
        foreach (GoalDisplay goalDisplay in goals.Values)
        {
            if (goalDisplay.Goal > 0)
            {
                return false;
            }
        }
        return true;
    }
}