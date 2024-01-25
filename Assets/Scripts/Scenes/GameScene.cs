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
    public NamedValue scorePrefab;
    public GameObject scoreboardContainer;
    [SerializeField] private GameObject steamPrefab;

    public bool settings;

    private NamedValue[] scoreboard;
    public Spinach.Grid<NamedValue> grid;

    protected override void Start()
    {
        base.Start();
        grid = Spinach.Grid<NamedValue>.DockedGrid(4, 1, Spinach.DockPosition.Top, 1.5f);

        scoreboard = new NamedValue[4];
        for (int x = 0; x < 4; x++)
        {
            scoreboard[x] = Instantiate(scorePrefab, grid.GetWorldPosition(x, 0) + new Vector3(.75f, .75f, 0), Quaternion.identity);
            scoreboard[x].transform.SetParent(scoreboardContainer.transform, false);
        }
        scoreboard[0].Title = "score";
        scoreboard[1].Title = "moves";
        scoreboard[2].Title = "goal";
        scoreboard[3].Title = "level";

        scoreboard[0].Value = GameManager.instance.gameData.GameScore;
        scoreboard[1].Value = GameManager.instance.gameData.LevelMovesRemaining;
        scoreboard[2].Value = GameManager.instance.gameData.LevelGoalRemaining;
        scoreboard[3].Value = GameManager.instance.gameData.Level;

        //update the scoreboard values when the gameData values change
        GameManager.instance.gameData.onValueChanged += (string key, int value) =>
        {
            switch (key)
            {
                case "GameScore":
                    scoreboard[0].Value = value;
                    // blast of steam
                    // if (value > 0 && scoreboard[0]?.transform?.position != null)
                    // {
                    //     Quaternion rotation = Quaternion.Euler(9.364f, 26.941f, -107.752f);
                    //     GameObject steam = Instantiate(steamPrefab, new Vector3(scoreboard[0].transform.position.x, scoreboard[0].transform.position.y, 7), rotation);
                    // }
                    break;
                case "LevelGoalRemaining":
                    scoreboard[2].Value = value;
                    break;
                case "LevelMovesRemaining":
                    scoreboard[1].Value = value;
                    break;
                case "Level":
                    scoreboard[3].Value = value;
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

    public void ProcessTurn(int pointsToGain, bool subtractMoves)
    {
        GameManager.instance.gameData.AddScore(pointsToGain, subtractMoves);

        if (pointsToGain > 0)
        {
            // play the swell animation on the score text one time
            // load the BumpScore transition in the animator
            // pointsText.GetComponent<Animator>().SetTrigger("BumpScore");
        }
        if (GameManager.instance.gameData.LevelGoalRemaining <= 0) Invoke("WinLevel", 0.5f);
    }

    public void CheckGameOver()
    {
        if (GameManager.instance.gameData.LevelMovesRemaining <= 0) Invoke("GameOver", 0.5f);
    }

    public void WinLevel()
    {
        GameManager.instance.StopMusic();
        GameManager.instance.PlaySoundEffect(levelWinClip);
        gameBoardPanel.SetActive(false);
        victoryLevel.text = GameManager.instance.gameData.Level.ToString();
        victoryPanel.SetActive(true);
        return;
    }

    public void GameOver()
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
