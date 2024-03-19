using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Openworld.Scenes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Building, // building the board
    Idle, // waiting for player input
    Checking, // player input has been received, verifying valid move
    Matching, // cascading matches are being processed
    Win, // player has completed all objectives
    Fail, // player has run out of moves
    Menu, // player is in the menu
}

public class GameScene : StatefulScene<GameState>
{
    public AudioClip[] levelMusic;
    public AudioClip levelAmbient;
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
        return levelMusic.Length > 0;
    }

    public override AudioClip GetMusic()
    {
        return levelMusic[Random.Range(0, levelMusic.Length)];
    }

    public override bool HasAmbient()
    {
        return levelAmbient != null;
    }

    public override AudioClip GetAmbient()
    {
        return levelAmbient;
    }

    /// <summary>
    /// Processes a turn in the game.
    /// </summary>
    /// <param name="pointsToGain">The number of points to add to the score for this turn.</param>
    /// <param name="subtractMoves">If true, subtracts a move from the remaining moves. If false, the number of moves remains the same.</param>
    /// <returns>True if the level goal has been reached and the level is won. False otherwise.</returns>
    public void ProcessTurn(List<Match3Part> scoreItems)
    {
        // convert the scoreItems into a map of Match3Item to count
        Dictionary<Match3Item, int> scoreMap = new Dictionary<Match3Item, int>();
        foreach (Match3Part part in scoreItems)
        {
            if (scoreMap.ContainsKey(part.item))
            {
                scoreMap[part.item]++;
            }
            else
            {
                scoreMap[part.item] = 1;
            }
        }
        // reduce the goals by the scoreMap
        foreach (Match3Item item in scoreMap.Keys)
        {
            goalTracker.ReduceGoal(item, scoreMap[item]);
        }
        scoreTracker.Change(scoreTracker.Value + scoreItems.Count);
    }

    private bool matching = false;
    public void MatchMoveSound(AudioClip clip)
    {
        if (!matching)
        {
            matching = true;
            GameManager.instance.PlaySoundEffect(clip);
        }
    }

    public void MatchDone()
    {
        matching = false;
    }

    public bool CheckWin()
    {
        return goalTracker.AllGoalsMet();
    }

    public bool CheckFail()
    {
        return movesTracker.Value <= 0;
    }

    private IEnumerator WinLevel()
    {
        while (movesTracker.Value > 0)
        {
            movesTracker.Change(movesTracker.Value - 1);
            scoreTracker.Change(scoreTracker.Value + 5);
            yield return new WaitForSeconds(.25f);
        }
        yield return new WaitForSeconds(1f);
        promotionText.SetActive(GameManager.instance.gameData.EarnPromotion(scoreTracker.Value));
        victoryScoreText.text = scoreTracker.Value.ToString();
        GameManager.instance.gameData.AddExperience(scoreTracker.Value);
        victoryExpNeededText.text = GameManager.instance.gameData.ExpToNextLevel().ToString();
        GameManager.instance.StopMusic();
        GameManager.instance.StopAmbient();
        GameManager.instance.PlaySoundEffect(victoryClip);
        gameBoardPanel.SetActive(false);
        victoryPanel.SetActive(true);
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

    protected override GameState GetInitialState()
    {
        return GameState.Building;
    }

    protected override Dictionary<GameState, List<GameState>> GetStateTransitions()
    {
        return new Dictionary<GameState, List<GameState>>
        {
            { GameState.Building, new List<GameState> { GameState.Idle } },
            { GameState.Idle, new List<GameState> { GameState.Checking, GameState.Menu } },
            { GameState.Checking, new List<GameState> { GameState.Matching, GameState.Idle } },
            { GameState.Matching, new List<GameState> { GameState.Idle, GameState.Win, GameState.Fail } },
            { GameState.Menu, new List<GameState> { GameState.Idle } }
        };
    }

    protected override void HandleEnterStateLocal(GameState previousState, GameState newState)
    {
        switch (newState)
        {
            case GameState.Building:
                break;
            case GameState.Idle:
                Match3Board.Instance.WaitForInput();
                break;
            case GameState.Checking:
                StartCoroutine(Match3Board.Instance.ValidateSwap());
                break;
            case GameState.Matching:
                movesTracker.Change(movesTracker.Value - 1);
                StartCoroutine(Match3Board.Instance.ProcessTurnOnMatchedBoard());
                break;
            case GameState.Win:
                StartCoroutine(WinLevel());
                break;
            case GameState.Fail:
                Fail();
                break;
            case GameState.Menu:
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    protected override void HandleExitStateLocal(GameState previousState, GameState newState)
    {

    }
}
