using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
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

    // Score is the player's running total.  This is treated like experience.
    // Each game board will be constructed based on the player's score.
    // If the player fails to meet the goals, the score is not updated.
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
