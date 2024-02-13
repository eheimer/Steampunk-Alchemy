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
    public NamedValue movesPrefab;
    public GameObject scoreboardContainer;
    [SerializeField] private GameObject steamPrefab;
    [SerializeField] private Part partGoalPrefab;

    private int score;  // the player's score for this game board

    private Dictionary<Match3ItemType, GoalItem> goalItems = new Dictionary<Match3ItemType, GoalItem>();

    public bool settings;

    public Spinach.Grid<NamedValue> goalsGrid;

    public int movesPerLevel = 10;
    public int movesRemaining = 10;

    protected override void Start()
    {
        base.Start();
        goalsGrid = Spinach.Grid<NamedValue>.DockedGrid(4, 1, Spinach.DockPosition.Bottom, 1f);
        goalItems.Clear();
        for (int i = 0; i < System.Enum.GetNames(typeof(Match3ItemType)).Length; i++)
        {
            Match3ItemType type = (Match3ItemType)i;
            Part goalItem = Instantiate(partGoalPrefab, goalsGrid.GetCellCenter(i, 0), Quaternion.identity, scoreboardContainer.transform);
            goalItem.Init(type, false);
            //goalItems.Add(type, new GoalItem(goalItem,false));
        }

        // determine moves based on gameData.Score
        movesPrefab.Value = movesPerLevel;
        movesRemaining = movesPerLevel;
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
        score += pointsToGain;
        if (subtractMoves) movesRemaining--;
        // check goals for this game board.
        // if all goals are met, call WinLevel() and return true.
        // if not, return false.
        return false;
    }

    public void CheckFail()
    {
        if (movesRemaining <= 0) Fail();
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
