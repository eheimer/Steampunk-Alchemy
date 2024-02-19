using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : Scene
{
    public AudioClip[] levelMusic;
    public AudioClip victoryClip;
    public AudioClip failClip;
    public GameObject gameBoardPanel;
    public GameObject victoryPanel;
    public GameObject failPanel;
    public NamedValue movesTracker;
    public NamedValue scoreTracker;
    [SerializeField] private GameObject steamPrefab;
    [SerializeField] private GoalTracker goalTracker;

    public bool settings;

    public Spinach.Grid<NamedValue> goalsGrid;

    public int movesPerLevel = 10;

    protected override void Start()
    {
        base.Start();
        //determine how many goals we need to display for this level.  For now we'll hard-code it to 4.
        int goalCount = 4;
        goalsGrid = Spinach.Grid<NamedValue>.DockedGrid(goalCount, 1, Spinach.DockPosition.Bottom, 1f);
        //define the set of goals.  Each goal will consist of a Match3ItemType, whether or not it's broken, and a count.
        for (int i = 0; i < goalCount; i++)
        {
            Match3Item goalItem = new Match3Item((Match3ItemType)i, false);
            goalTracker.SetGoal(goalItem, goalsGrid.GetCellCenter(i, 0), Random.Range(5, 30));
        }

        movesTracker.Value = movesPerLevel;
        scoreTracker.Value = 0;
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
        scoreTracker.Value += pointsToGain;
        if (subtractMoves)
        {
            movesTracker.Value--;
        }

        if (goalTracker.AllGoalsMet())
        {
            WinLevel();
            return true;
        }
        return false;
    }

    public void CheckFail()
    {
        if (movesTracker.Value <= 0) Fail();
    }

    private void WinLevel()
    {
        GameManager.instance.StopMusic();
        GameManager.instance.PlaySoundEffect(victoryClip);
        gameBoardPanel.SetActive(false);
        //victoryLevel.text = GameManager.instance.gameData.Level.ToString();
        victoryPanel.SetActive(true);
        return;
    }

    private void Fail()
    {
        GameManager.instance.StopMusic();
        GameManager.instance.PlayMusic(failClip);
        gameBoardPanel.SetActive(false);
        //gameOverLevel.text = GameManager.instance.gameData.Level.ToString();
        //gameOverScore.text = GameManager.instance.gameData.Score.ToString();
        // gameOverBestLevel.text = GameManager.instance.gameData.BestLevel.ToString();
        // gameOverBestScore.text = GameManager.instance.gameData.BestScore.ToString();

        //GameManager.instance.gameData.GameOver();
        failPanel.SetActive(true);
        return;
    }

    public void NextLevelButtonAction()
    {
        SceneManager.LoadScene(SceneName.Start.name());
    }



    public void RestartButtonAction()
    {
        SceneManager.LoadScene(SceneName.Start.name());
    }
}
