using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject potionPanel;
    public GameObject victoryPanel;
    public GameObject losePanel;
    public int goal;
    public int startingMoves;
    public int moves;
    public int points;

    public TMP_Text pointsText;
    public TMP_Text movesText;
    public TMP_Text goalText;
    public TMP_Text victoryDescription;
    public TMP_Text loseDescription;
    public string victoryText = "Congratulations, you won in {moves} moves and scored {points} points!";
    public string loseText = "Unfortunately, you only got {points} points in {moves} moves. Better luck next time!";

    public bool isGameEnded;
    private void Awake()
    {
        instance = this;
        moves = startingMoves;
    }

    public void Initialize(int moves, int goal)
    {
        this.moves = moves;
        this.goal = goal;
    }

    void Update()
    {
        pointsText.text = "Points: " + points.ToString();
        movesText.text = "Moves: " + moves.ToString();
        goalText.text = "Goal: " + goal.ToString();
    }

    public void ProcessTurn(int pointsToGain, bool subtractMoves)
    {
        points += pointsToGain;
        if (subtractMoves)
        {
            moves--;
        }
        if (points >= goal)
        {
            isGameEnded = true;
            // PotionBoard.Instance.potionParent.SetActive(false);
            potionPanel.SetActive(false);
            //backgroundPanel.SetActive(true);
            victoryDescription.text = victoryText.Replace("{moves}", (startingMoves - moves).ToString()).Replace("{points}", points.ToString());
            victoryPanel.SetActive(true);
            return;
        }
        if (moves <= 0)
        {
            isGameEnded = true;
            //PotionBoard.Instance.potionParent.SetActive(false);
            potionPanel.SetActive(false);
            //backgroundPanel.SetActive(true);
            loseDescription.text = loseText.Replace("{moves}", (startingMoves - moves).ToString()).Replace("{points}", points.ToString());
            losePanel.SetActive(true);
            return;
        }
    }

    public void WinGame()
    {
        SceneManager.LoadScene(0);
    }

    public void LoseGame()
    {
        SceneManager.LoadScene(0);
    }
}
