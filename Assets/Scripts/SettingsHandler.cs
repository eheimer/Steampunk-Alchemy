using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour
{
    public GameObject button;
    public GameObject panel;
    public Toggle musicToggle;
    public Toggle soundToggle;

    public void SettingsButtonAction()
    {
        // set up the panel so that it shows the current settings
        musicToggle.SetIsOnWithoutNotify(GameManager.instance.gameData.music);
        soundToggle.SetIsOnWithoutNotify(GameManager.instance.gameData.sound);
        button.SetActive(false);
        panel.SetActive(true);
    }

    public void SettingsCloseButtonAction()
    {
        button.SetActive(true);
        panel.SetActive(false);
    }

    public void SettinsQuitButtonAction()
    {
        GameManager.instance.gameData.GameOver(); ;
        SceneManager.LoadScene(SceneName.Start.name());
    }

    public void ToggleMusicAction()
    {
        GameManager.instance.gameData.ToggleMusic();
        GameManager.instance.GetComponent<AudioSource>().enabled = GameManager.instance.gameData.music;
        if (GameManager.instance.gameData.music)
        {
            GameManager.instance.currentScene.PlayMusic();
        }
    }

    public void ToggleSoundAction()
    {
        GameManager.instance.gameData.ToggleSound();
        GameManager.instance.currentScene.SetSound(GameManager.instance.gameData.sound);
    }


}
