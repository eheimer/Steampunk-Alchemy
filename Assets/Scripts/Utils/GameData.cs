using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameData
{
    public struct LevelDefinition
    {
        public int moves;
        public Dictionary<Match3Item, int> goals;
    }
    public static SortedDictionary<int, LevelDefinition> levels = new()
    {
        { 0, new() { moves = 7, goals = new() { { new Match3Item(Match3ItemType.Blue, false), 15 } } } },
        { 200, new() { moves = 10, goals = new() {
            {new Match3Item(Match3ItemType.Blue, false), 15},
            {new Match3Item(Match3ItemType.Red, false), 15}}}
        },
        { 500, new() { moves = 10, goals = new() {
            {new Match3Item(Match3ItemType.Blue, false), 20},
            {new Match3Item(Match3ItemType.Yellow, false), 15}}}
        },
        { 800, new() { moves = 12, goals = new() {
            {new Match3Item(Match3ItemType.Blue, false), 15},
            {new Match3Item(Match3ItemType.Red, false), 15},
            {new Match3Item(Match3ItemType.Yellow, false), 15}}}
        },
        { 1200, new() { moves = 15, goals = new() {
            {new Match3Item(Match3ItemType.Blue, false), 15},
            {new Match3Item(Match3ItemType.Red, false), 15},
            {new Match3Item(Match3ItemType.Yellow, false), 15},
            {new Match3Item(Match3ItemType.Green, false), 15}}}
        },
        { 1800, new() { moves = 18, goals = new() {
            {new Match3Item(Match3ItemType.Blue, false), 30},
            {new Match3Item(Match3ItemType.Red, false), 30},
            {new Match3Item(Match3ItemType.Yellow, false), 30},
            {new Match3Item(Match3ItemType.Green, false), 30}}}
        },
        { 2300, new() { moves = 20, goals = new() {
            {new Match3Item(Match3ItemType.Blue, false), 30},
            {new Match3Item(Match3ItemType.Red, false), 30},
            {new Match3Item(Match3ItemType.Yellow, false), 30},
            {new Match3Item(Match3ItemType.Green, false), 30},
            {new Match3Item(Match3ItemType.Purple, false), 20}}}
        },
        { 3000, new() { moves = 20, goals = new() {
            {new Match3Item(Match3ItemType.Blue, false), 35},
            {new Match3Item(Match3ItemType.Red, false), 35},
            {new Match3Item(Match3ItemType.Yellow, false), 35},
            {new Match3Item(Match3ItemType.Green, false), 35},
            {new Match3Item(Match3ItemType.Purple, false), 35}}}
        }
    };

    public delegate void OnValueChanged(string key, int value);
    public event OnValueChanged onValueChanged;

    public GameData() { }

    private int GetInt(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    private void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        onValueChanged?.Invoke(key, value);
    }

    public int Experience
    {
        get { return GetInt("Experience", 0); }
        private set { SetInt("Experience", value); }
    }

    public void AddExperience(int value)
    {
        Experience += value;
    }

    public void ResetExperience()
    {
        Experience = 0;
    }

    public LevelDefinition GetLevelDefinition() { return GetLevelDefinitionForExperience(Experience); }

    public LevelDefinition GetLevelDefinitionForExperience(int checkExp)
    {
        LevelDefinition def = levels.Last(x => x.Key <= checkExp).Value;
        if (def.Equals(levels.Last().Value))
        {
            //increase each goal by 5 for each 1000 points above the last definition
            var goals = new Dictionary<Match3Item, int>(def.goals);
            for (int i = levels.Last().Key + 1000; i <= checkExp; i += 1000)
            {
                foreach (var goal in def.goals.Keys)
                {
                    goals[goal] += 5;
                }
            }
            def.goals = goals;
        }
        return def;
    }

    public bool EarnPromotion(int expToAdd)
    {
        if (Experience >= levels.Last().Key)
        {
            return (int)(Experience / 1000) < (int)((Experience + expToAdd) / 1000);
        }
        LevelDefinition def = GetLevelDefinition();
        LevelDefinition next = GetLevelDefinitionForExperience(Experience + expToAdd);
        return !next.Equals(def);
    }

    public int ExpToNextLevel()
    {
        var level = levels.First();
        try
        {
            level = levels.First(x => x.Key > Experience);
        }
        catch (InvalidOperationException)
        {
            return 1000 - (Experience % 1000);
        }
        return level.Key - Experience;
    }

    public bool Music
    {
        get { return GetInt("Music", 1) == 1; }
        private set { SetInt("Music", value ? 1 : 0); }
    }

    public bool Sound
    {
        get { return GetInt("Sound", 1) == 1; }
        private set { SetInt("Sound", value ? 1 : 0); }
    }

    public void ToggleMusic()
    {
        Music = !Music;
    }

    public void ToggleSound()
    {
        Sound = !Sound;
    }
}
