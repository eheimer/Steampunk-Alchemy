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

    [SerializeField]
    AudioSource musicPlayer;
    [SerializeField]
    AudioSource soundEffectsPlayer;
    [SerializeField]
    AudioSource ambientPlayer;

    public GameData gameData;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            DestroyAll();
            return;
        }

        gameData = new GameData();
        instance = this;
        DontDestroyOnLoad(gameObject);

        musicPlayer.enabled = gameData.Music;
        soundEffectsPlayer.enabled = gameData.Sound;
        ambientPlayer.enabled = gameData.Ambient;

        gameData.onValueChanged += (string key, int value) =>
        {
            if (key == "Music")
            {
                musicPlayer.enabled = value == 1;
                if (currentScene.HasMusic())
                {
                    PlayMusic();
                }
            }
            if (key == "Sound")
            {
                soundEffectsPlayer.enabled = value == 1;
            }
            if (key == "Ambient")
            {
                ambientPlayer.enabled = value == 1;
                if (currentScene.HasAmbient())
                {
                    PlayAmbient(currentScene.GetAmbient());
                }
            }
        };
    }

    /// <summary>
    /// Play the music if it is not already playing.
    /// </summary>
    public void PlayMusic()
    {
        if (musicPlayer.enabled && !musicPlayer.isPlaying && musicPlayer.clip != null)
        {
            musicPlayer.Play();
        }
    }

    /// <summary>
    /// Assign a new clip and start playing it
    /// </summary>
    /// <param name="clip"></param>
    public void PlayMusic(AudioClip clip)
    {
        if (musicPlayer.enabled && musicPlayer.isPlaying)
        {
            StopMusic();
        }
        musicPlayer.clip = clip;
        PlayMusic();
    }

    public void StopMusic()
    {
        musicPlayer.Stop();
    }

    public void PlayAmbient(AudioClip clip)
    {
        if (ambientPlayer.enabled && ambientPlayer.isPlaying)
        {
            StopAmbient();
        }
        ambientPlayer.clip = clip;
        ambientPlayer.Play();
    }

    public void StopAmbient()
    {
        ambientPlayer.Stop();
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        if (soundEffectsPlayer.enabled)
        {
            soundEffectsPlayer.PlayOneShot(clip);
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
