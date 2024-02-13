using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    private int startingGoal; // the goal for level 1
    private int startingMoves; // the number of moves for level 1

    public delegate void OnValueChanged(string key, int value);
    public event OnValueChanged onValueChanged;

    public GameData(int startingGoal, int startingMoves)
    {
        this.startingGoal = startingGoal;
        this.startingMoves = startingMoves;
    }

    private int GetInt(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }
    private void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        onValueChanged?.Invoke(key, value);
    }

    public int GameScore
    {
        get { return GetInt("GameScore", 0); }
        private set { SetInt("GameScore", value); }
    }
    public int LevelScore
    {
        get { return GetInt("LevelScore", 0); }
        private set
        {
            SetInt("LevelScore", value);
            LevelGoalRemaining = Goal - value;
        }
    }
    public int LevelGoalRemaining
    {
        get { return Mathf.Max(GetInt("LevelGoalRemaining", 0), 0); }
        private set { SetInt("LevelGoalRemaining", value); }

    }
    public int LevelMovesRemaining
    {
        get { return GetInt("LevelMovesRemaining", 0); }
        private set { SetInt("LevelMovesRemaining", value); }
    }
    // NOTE: If a game is not in progress, GameData may contain the values from the previous game.
    public bool GameInProgress
    {
        get { return GetInt("GameInProgress", 0) == 1; }
        private set { SetInt("GameInProgress", value ? 1 : 0); }
    }
    public int Level
    {
        get { return GetInt("Level", 1); }
        private set { SetInt("Level", value); }
    }
    public int Goal
    {
        get { return GetInt("Goal", startingGoal); }
        private set
        {
            SetInt("Goal", value);
            LevelGoalRemaining = value - LevelScore;
        }
    }
    public int BestScore
    {
        get { return GetInt("BestScore", 0); }
        private set { SetInt("BestScore", value); }
    }
    public int BestLevel
    {
        get { return GetInt("BestLevel", 0); }
        private set { SetInt("BestLevel", value); }
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

    public void AddScore(int score, bool moves = true)
    {
        LevelScore += score;
        GameScore += score;
        if (moves) LevelMovesRemaining--;
    }

    public void NextLevel(int goal, int moves)
    {
        Level++;
        Goal = goal;
        LevelScore = 0;
        LevelMovesRemaining = moves;
    }

    public void Restart()
    {
        GameInProgress = true;
        Goal = startingGoal;
        GameScore = 0;
        LevelScore = 0;
        Level = 1;
        LevelMovesRemaining = startingMoves;
    }

    public void GameOver()
    {
        GameInProgress = false;
        if (BestScore < GameScore)
        {
            BestScore = GameScore;
        }
        if (BestLevel < Level)
        {
            BestLevel = Level;
        }
    }

    public void ToggleMusic()
    {
        Music = !Music;
    }

    public void ToggleSound()
    {
        Sound = !Sound;
    }

    // public Dictionary<int, GoalItem[]> LevelGoals = new Dictionary<int, GoalItem[]>(){
    //     {1, new GoalItem[]{
    //         new GoalItem(Match3ItemType.Blue, false, 10),
    //         new GoalItem(Match3ItemType.Green, false, 10),
    //         new GoalItem(Match3ItemType.Red, false, 10),
    //         new GoalItem(Match3ItemType.Yellow, false, 10)
    //     }},
    //     {2, new GoalItem[]{
    //         new GoalItem(Match3ItemType.Blue, false, 15),
    //         new GoalItem(Match3ItemType.Green, false, 15),
    //         new GoalItem(Match3ItemType.Red, false, 15),
    //         new GoalItem(Match3ItemType.Yellow, false, 15)
    //     }},
    //     {3, new GoalItem[]{
    //         new GoalItem(Match3ItemType.Blue, false, 20),
    //         new GoalItem(Match3ItemType.Green, false, 20),
    //         new GoalItem(Match3ItemType.Red, false, 20),
    //         new GoalItem(Match3ItemType.Yellow, false, 20)
    //     }},
    //     {4, new GoalItem[]{
    //         new GoalItem(Match3ItemType.Blue, false, 25),
    //         new GoalItem(Match3ItemType.Green, false, 25),
    //         new GoalItem(Match3ItemType.Red, false, 25),
    //         new GoalItem(Match3ItemType.Yellow, false, 25)
    //     }},
    //     {5, new GoalItem[]{
    //         new GoalItem(Match3ItemType.Blue, false, 30),
    //         new GoalItem(Match3ItemType.Green, false, 30),
    //         new GoalItem(Match3ItemType.Red, false, 30),
    //         new GoalItem(Match3ItemType.Yellow, false, 30)
    //     }},
    //     {6, new GoalItem[]{
    //         new GoalItem(Match3ItemType.Blue, false, 35),
    //         new GoalItem(Match3ItemType.Green, false, 35),
    //         new GoalItem(Match3ItemType.Red, false, 35),
    //         new GoalItem(Match3ItemType.Yellow, false, 35)
    //     }},
    //     {7, new GoalItem[]{
    //         new GoalItem(Match3ItemType.Blue, false, 35),
    //         new GoalItem(Match3ItemType.Green, false, 35),
    //         new GoalItem(Match3ItemType.Red, false, 35),
    //         new GoalItem(Match3ItemType.Yellow, false, 35)
    //     }} };
}
