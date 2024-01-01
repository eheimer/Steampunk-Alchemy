using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    private int startingGoal; // the goal for level 1
    private int startingMoves; // the number of moves for level 1
    public GameData(int startingGoal, int startingMoves)
    {
        this.startingGoal = startingGoal;
        this.startingMoves = startingMoves;
    }

    public int GameScore
    {
        get { return PlayerPrefs.GetInt("gameScore", 0); }
        private set { PlayerPrefs.SetInt("gameScore", value); }
    }
    public int LevelScore
    {
        get { return PlayerPrefs.GetInt("levelScore", 0); }
        private set { PlayerPrefs.SetInt("levelScore", value); }
    }
    public int LevelGoalRemaining
    {
        get { return Mathf.Max(Goal - LevelScore, 0); }
    }
    public int LevelMovesRemaining
    {
        get { return PlayerPrefs.GetInt("levelMovesRemaining", 0); }
        private set { PlayerPrefs.SetInt("levelMovesRemaining", value); }
    }
    // NOTE: If a game is not in progress, GameData may contain the values from the previous game.
    public bool GameInProgress
    {
        get { return PlayerPrefs.GetInt("gameInProgress", 0) == 1; }
        private set { PlayerPrefs.SetInt("gameInProgress", value ? 1 : 0); }
    }
    public int Level
    {
        get { return PlayerPrefs.GetInt("level", 1); }
        private set { PlayerPrefs.SetInt("level", value); }
    }
    public int Goal
    {
        get { return PlayerPrefs.GetInt("goal", startingGoal); }
        private set { PlayerPrefs.SetInt("goal", value); }
    }
    public int bestScore
    {
        get { return PlayerPrefs.GetInt("bestScore", 0); }
        private set { PlayerPrefs.SetInt("bestScore", value); }
    }
    public int bestLevel
    {
        get { return PlayerPrefs.GetInt("bestLevel", 0); }
        private set { PlayerPrefs.SetInt("bestLevel", value); }
    }
    public bool music
    {
        get { return PlayerPrefs.GetInt("music", 1) == 1; }
        private set { PlayerPrefs.SetInt("music", value ? 1 : 0); }
    }

    public bool sound
    {
        get { return PlayerPrefs.GetInt("sound", 1) == 1; }
        private set { PlayerPrefs.SetInt("sound", value ? 1 : 0); }
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
        if (bestScore < GameScore)
        {
            bestScore = GameScore;
        }
        if (bestLevel < Level)
        {
            bestLevel = Level;
        }
    }

    public void ToggleMusic()
    {
        music = !music;
    }

    public void ToggleSound()
    {
        sound = !sound;
    }
}
