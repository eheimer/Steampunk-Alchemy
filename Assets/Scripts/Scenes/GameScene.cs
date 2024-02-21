using System.Linq;
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
    [SerializeField] private TMP_Text victoryScoreText;
    [SerializeField] private TMP_Text victoryExpNeededText;
    [SerializeField] private GameObject promotionText;

    public bool settings;

    public Spinach.Grid<NamedValue> goalsGrid;

    public int movesPerLevel = 10;

    protected override void Start()
    {
        base.Start();
        GameData.LevelDefinition level = GameManager.instance.gameData.GetLevelDefinition();
        movesTracker.Value = level.moves;
        //determine how many goals we need to display for this level.  For now we'll hard-code it to 4.
        int goalCount = level.goals.Count;
        goalsGrid = Spinach.Grid<NamedValue>.DockedGrid(goalCount, 1, Spinach.DockPosition.Bottom, 1f);
        for (int i = 0; i < goalCount; i++)
        {
            Match3Item goalItem = level.goals.Keys.ElementAt(i);
            goalTracker.SetGoal(goalItem, goalsGrid.GetCellCenter(i, 0), level.goals[goalItem]);
        }
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
        scoreTracker.Value += movesTracker.Value * 5;
        promotionText.SetActive(GameManager.instance.gameData.EarnPromotion(scoreTracker.Value));
        victoryScoreText.text = scoreTracker.Value.ToString();
        GameManager.instance.gameData.AddExperience(scoreTracker.Value);
        victoryExpNeededText.text = GameManager.instance.gameData.ExpToNextLevel().ToString();
        GameManager.instance.PlaySoundEffect(victoryClip);
        gameBoardPanel.SetActive(false);
        victoryPanel.SetActive(true);
        return;
    }

    private void Fail()
    {
        GameManager.instance.PlaySoundEffect(failClip);
        gameBoardPanel.SetActive(false);
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
