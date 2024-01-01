using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : Scene
{
    public TMP_Text pointsText;
    public TMP_Text movesText;
    public TMP_Text goalText;
    public TMP_Text levelText;
    public AudioClip[] levelMusic;
    public AudioClip levelWinClip;
    public AudioClip gameOverClip;
    public GameObject potionPanel;
    public GameObject scorePanel;
    public GameObject victoryPanel;
    public GameObject gameOverPanel;
    public TMP_Text victoryLevel;
    public TMP_Text gameOverLevel;
    public TMP_Text gameOverScore;
    public TMP_Text gameOverBestLevel;
    public TMP_Text gameOverBestScore;


    public override bool HasMusic()
    {
        return true;
    }

    public override AudioClip GetMusic()
    {
        return levelMusic[Random.Range(0, levelMusic.Length)];
    }

    void Update()
    {
        pointsText.text = GameManager.instance.gameData.GameScore.ToString();
        movesText.text = GameManager.instance.gameData.LevelMovesRemaining.ToString();
        goalText.text = GameManager.instance.gameData.LevelGoalRemaining.ToString();
        levelText.text = GameManager.instance.gameData.Level.ToString();
    }

    public void ProcessTurn(int pointsToGain, bool subtractMoves)
    {
        GameManager.instance.gameData.AddScore(pointsToGain, subtractMoves);

        if (pointsToGain > 0)
        {
            // play the swell animation on the score text one time
            // load the BumpScore transition in the animator
            pointsText.GetComponent<Animator>().SetTrigger("BumpScore");
        }
        if (GameManager.instance.gameData.LevelGoalRemaining <= 0) WinLevel();
    }

    public void CheckGameOver()
    {
        if (GameManager.instance.gameData.LevelMovesRemaining <= 0) GameOver();
    }

    public void WinLevel()
    {
        PlaySound(levelWinClip);
        potionPanel.SetActive(false);
        victoryLevel.text = GameManager.instance.gameData.Level.ToString();
        victoryPanel.SetActive(true);
        return;
    }

    public void GameOver()
    {
        PlaySound(gameOverClip);
        potionPanel.SetActive(false);
        scorePanel.SetActive(false);
        gameOverLevel.text = GameManager.instance.gameData.Level.ToString();
        gameOverScore.text = GameManager.instance.gameData.GameScore.ToString();
        gameOverBestLevel.text = GameManager.instance.gameData.bestLevel.ToString();
        gameOverBestScore.text = GameManager.instance.gameData.bestScore.ToString();

        GameManager.instance.gameData.GameOver();
        gameOverPanel.SetActive(true);
        return;
    }

    public void NextLevelButtonAction()
    {
        GameManager.instance.gameData.NextLevel(GameManager.instance.gameData.Goal + UnityEngine.Random.Range(5, 10), GameManager.instance.startingMoves);
        SceneManager.LoadScene(SceneName.Game.name());
    }



    public void RestartButtonAction()
    {
        GameManager.instance.gameData.Restart();
        SceneManager.LoadScene(SceneName.Start.name());
    }

    private void PlaySound(AudioClip clip)
    {
        if (gameObject.GetComponent<AudioSource>().isPlaying) gameObject.GetComponent<AudioSource>().Stop();
        if (GameManager.instance.gameData.sound)
        {
            gameObject.GetComponent<AudioSource>().clip = clip;
            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
