using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject settingsPanel;
    public GameObject settingsButton;
    public GameObject scorePanel;
    public GameObject potionPanel;
    public GameObject victoryPanel;
    public GameObject gameOverPanel;
    public int startingGoal; // when a new game starts, this is the goal
    public int startingMoves; // number of moves per level
    public int remainingMoves; // number of moves remaining for current level
    public int remainingGoal; // number of points needed for current level
    public int levelScore; // number of points scored in current level
    public TMP_Text pointsText;
    public TMP_Text movesText;
    public TMP_Text goalText;
    public TMP_Text levelText;
    public TMP_Text victoryLevel;
    public TMP_Text gameOverLevel;
    public TMP_Text gameOverScore;
    public TMP_Text gameOverBestLevel;
    public TMP_Text gameOverBestScore;
    public GameData gameData;

    public AudioClip[] levelMusic;
    public AudioClip levelWinClip;
    public AudioClip gameOverClip;

    private void Awake()
    {
        gameData = new GameData(startingGoal);
        remainingGoal = gameData.Goal;
        remainingMoves = startingMoves; // every level has the same number of moves
        levelScore = 0;
        instance = this;
        foreach (Toggle t in settingsPanel.GetComponentsInChildren<Toggle>())
        {
            if (t.name == "MusicToggle") t.SetIsOnWithoutNotify(gameData.music);
            if (t.name == "SoundToggle") t.SetIsOnWithoutNotify(gameData.sound);
        }
        PlayMusic();
    }

    private void PlayMusic()
    {
        if (gameData.music)
        {
            gameObject.GetComponent<AudioSource>().clip = levelMusic[UnityEngine.Random.Range(0, levelMusic.Length)];
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (gameObject.GetComponent<AudioSource>().isPlaying) gameObject.GetComponent<AudioSource>().Stop();
        if (gameData.sound)
        {
            gameObject.GetComponent<AudioSource>().clip = clip;
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    void Update()
    {
        pointsText.text = (levelScore + gameData.Score).ToString();
        movesText.text = remainingMoves.ToString();
        goalText.text = Math.Max(remainingGoal, 0).ToString();
        levelText.text = gameData.Level.ToString();
    }

    public void ProcessTurn(int pointsToGain, bool subtractMoves)
    {
        levelScore += pointsToGain;
        remainingGoal -= pointsToGain;

        if (subtractMoves)
        {
            remainingMoves--;
        }
        if (remainingGoal <= 0) WinLevel();
    }

    public void CheckGameOver()
    {
        if (remainingMoves <= 0) GameOver();
    }

    public void WinLevel()
    {
        PlaySound(levelWinClip);
        potionPanel.SetActive(false);
        victoryLevel.text = gameData.Level.ToString();
        victoryPanel.SetActive(true);
        return;
    }

    public void GameOver()
    {
        PlaySound(gameOverClip);
        potionPanel.SetActive(false);
        scorePanel.SetActive(false);
        gameOverLevel.text = gameData.Level.ToString();
        gameOverScore.text = (gameData.Score + levelScore).ToString();
        gameOverBestLevel.text = gameData.bestLevel.ToString();
        gameOverBestScore.text = gameData.bestScore.ToString();

        gameData.GameOver(levelScore);
        levelScore = 0;
        gameOverPanel.SetActive(true);
        return;
    }

    public void NextLevelButtonAction()
    {
        gameData.NextLevel(UnityEngine.Random.Range(5, 10), levelScore);
        levelScore = 0;
        SceneManager.LoadScene(0);
    }

    public void RestartButtonAction()
    {
        SceneManager.LoadScene(0);
    }

    public void SettingsButtonAction()
    {
        settingsPanel.SetActive(true);
        potionPanel.SetActive(false);
        settingsButton.SetActive(false);
    }

    public void SettingsCloseButtonAction()
    {
        settingsPanel.SetActive(false);
        potionPanel.SetActive(true);
        settingsButton.SetActive(true);
    }

    public void ToggleMusicAction()
    {
        gameData.ToggleMusic();
        gameObject.GetComponent<AudioSource>().enabled = gameData.music;
        if (gameData.music) PlayMusic();
    }

    public void ToggleSoundAction()
    {
        gameData.ToggleSound();
        potionPanel.GetComponent<AudioSource>().enabled = gameData.sound;
    }

    // PlayerPrefs:
    //   goal: int - score needed to win the current level
    //     (default: startingGoal)
    //     this gets increased by a random amt with each level
    //   score: int - running score
    //     this gets increased by the number of points scored each level
    //   level: int - current level
    //     (default: 1)
    //   bestScore: int - best score
    //   bestLevel: int - best level
}
