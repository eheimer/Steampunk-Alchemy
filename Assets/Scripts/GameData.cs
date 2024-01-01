using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    private int startingGoal; // the goal for level 1
    public GameData(int startingGoal)
    {
        this.startingGoal = startingGoal;

        if (Level == 1)
        {
            Goal = startingGoal;
            Score = 0;
        }
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
    public int Score
    {
        get { return PlayerPrefs.GetInt("score", 0); }
        private set { PlayerPrefs.SetInt("score", value); }
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

    public void NextLevel(int goalAdd, int levelScore)
    {
        Level++;
        Goal += goalAdd;
        Score += levelScore;
    }

    public void Restart()
    {
        Level = 0;
        Goal = startingGoal;
        Score = 0;
    }

    public void GameOver(int levelScore)
    {
        Score += levelScore;
        if (bestScore < Score)
        {
            bestScore = Score;
        }
        if (bestLevel < Level)
        {
            bestLevel = Level;
        }
        Level = 1;
        Goal = startingGoal;
        Score = 0;
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
