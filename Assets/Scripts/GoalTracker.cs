using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTracker : MonoBehaviour
{
    [SerializeField] private GoalDisplay goalDisplayPrefab;

    [SerializeField] private GameObject scoreboardContainer;

    private Dictionary<Match3Item, GoalDisplay> goals = new Dictionary<Match3Item, GoalDisplay>();

    public bool HasUnmetGoal(Match3Item goalItem)
    {
        return goals.ContainsKey(goalItem) && goals[goalItem].Goal > 0;
    }

    public Vector3? GetGoalLocation(Match3Item goalItem)
    {
        if (goals.ContainsKey(goalItem))
        {
            return goals[goalItem].transform.position;
        }
        return null;
    }

    public void SetGoal(Match3Item item, Vector3 location, int goal)
    {
        if (!goals.ContainsKey(item))
        {
            GoalDisplay goalDisplay = Instantiate(goalDisplayPrefab, location, Quaternion.identity, transform);
            goalDisplay.Init(item, goal);
            goals.Add(item, goalDisplay);
        }
        else
        {
            goals[item].Goal = goal;
        }
    }

    public void ReduceGoal(Match3Item goalItem, int count)
    {
        if (goals.ContainsKey(goalItem))
        {
            goals[goalItem].Goal -= count;
        }
    }

    public void ReduceGoal(Match3ItemType itemType, bool broken, int count)
    {
        Match3Item goalItem = new Match3Item(itemType, broken);
        ReduceGoal(goalItem, count);
    }

    public void ReduceGoal(Part part, int count)
    {
        ReduceGoal(part.item, count);
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