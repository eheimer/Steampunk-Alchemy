using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.SceneManagement;

public class GameScene : Scene
{
    public AudioClip[] levelMusic;
    public AudioClip levelWinClip;
    public AudioClip gameOverClip;
    public GameObject gameBoardPanel;
    public GameObject victoryPanel;
    public GameObject gameOverPanel;
    public TMP_Text victoryLevel;
    public TMP_Text gameOverLevel;
    public TMP_Text gameOverScore;
    public TMP_Text gameOverBestLevel;
    public TMP_Text gameOverBestScore;
    public NamedValue movesPrefab;
    public NamedValue goalPrefab;
    public GameObject scoreboardContainer;
    [SerializeField] private GameObject steamPrefab;

    public bool settings;

    // private NamedValue[] scoreboard;
    public Spinach.Grid<NamedValue> grid;

    protected override void Start()
    {
        base.Start();
        grid = Spinach.Grid<NamedValue>.DockedGrid(4, 1, Spinach.DockPosition.Top, 1.5f);
        goalPrefab.Value = GameManager.instance.gameData.LevelGoalRemaining;
        movesPrefab.Value = GameManager.instance.gameData.LevelMovesRemaining;

        //update the scoreboard values when the gameData values change
        GameManager.instance.gameData.onValueChanged += (string key, int value) =>
        {
            switch (key)
            {
                // case "GameScore":
                //     scoreboard[0].Value = value;
                //     // blast of steam
                //     // if (value > 0 && scoreboard[0]?.transform?.position != null)
                //     // {
                //     //     Quaternion rotation = Quaternion.Euler(9.364f, 26.941f, -107.752f);
                //     //     GameObject steam = Instantiate(steamPrefab, new Vector3(scoreboard[0].transform.position.x, scoreboard[0].transform.position.y, 7), rotation);
                //     // }
                //     break;
                case "LevelGoalRemaining":
                    goalPrefab.Value = value;
                    // scoreboard[2].Value = value;
                    break;
                case "LevelMovesRemaining":
                    movesPrefab.Value = value;
                    // scoreboard[1].Value = value;
                    break;
            }
        };
    }

    public override bool HasMusic()
    {
        return true;
    }

    public override AudioClip GetMusic()
    {
        return levelMusic[Random.Range(0, levelMusic.Length)];
    }

    /// <summary>
    /// Processes a turn in the game.
    /// </summary>
    /// <param name="pointsToGain">The number of points to add to the score for this turn.</param>
    /// <param name="subtractMoves">If true, subtracts a move from the remaining moves. If false, the number of moves remains the same.</param>
    /// <returns>True if the level goal has been reached and the level is won. False otherwise.</returns>
    public bool ProcessTurn(int pointsToGain, bool subtractMoves)
    {
        GameManager.instance.gameData.AddScore(pointsToGain, subtractMoves);
        if (GameManager.instance.gameData.LevelGoalRemaining <= 0)
        {
            WinLevel();
            return true;
        }
        return false;
    }

    public void CheckGameOver()
    {
        if (GameManager.instance.gameData.LevelMovesRemaining <= 0) GameOver();
    }

    private void WinLevel()
    {
        GameManager.instance.StopMusic();
        GameManager.instance.PlaySoundEffect(levelWinClip);
        gameBoardPanel.SetActive(false);
        victoryLevel.text = GameManager.instance.gameData.Level.ToString();
        victoryPanel.SetActive(true);
        return;
    }

    private void GameOver()
    {
        GameManager.instance.StopMusic();
        GameManager.instance.PlayMusic(gameOverClip);
        gameBoardPanel.SetActive(false);
        gameOverLevel.text = GameManager.instance.gameData.Level.ToString();
        gameOverScore.text = GameManager.instance.gameData.GameScore.ToString();
        gameOverBestLevel.text = GameManager.instance.gameData.BestLevel.ToString();
        gameOverBestScore.text = GameManager.instance.gameData.BestScore.ToString();

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
}
