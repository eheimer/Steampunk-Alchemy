using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneName { Start, Game, GameOver }

static class SceneNameExtensions
{
    private static string[] names = new string[] { "StartScene", "GameScene", "GameOverScene" };
    public static string name(this SceneName scene) { return names[(int)scene]; }
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    // each scene must set this when it loads
    public Scene currentScene;
    public int startingGoal; // when a new game starts, this is the goal
    public int startingMoves; // number of moves per level

    public GameData gameData;

    private void Awake()
    {
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            DestroyAll();
            return;
        }

        gameData = new GameData(startingGoal, startingMoves);
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (gameData.GameInProgress)
        {
            SceneManager.LoadScene(SceneName.Game.name());
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        Debug.Log("PlayMusic, clip = " + clip.name);
        Debug.Log("enabled: " + gameData.music + ", " + gameObject.GetComponent<AudioSource>().enabled);
        if (gameData.music)
        {
            gameObject.GetComponent<AudioSource>().clip = clip;
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    void DestroyAll()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            Destroy(child.gameObject);
        }
        Destroy(gameObject);
    }


}
