using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        musicToggle.SetIsOnWithoutNotify(GameManager.instance.gameData.Music);
        soundToggle.SetIsOnWithoutNotify(GameManager.instance.gameData.Sound);
        button.SetActive(false);
        panel.SetActive(true);
        //disable swipes on the Match3Board while the settings panel is open
        Match3Board.Instance.ignoreInput = true;
    }

    public void SettingsCloseButtonAction()
    {
        button.SetActive(true);
        panel.SetActive(false);
        Match3Board.Instance.ignoreInput = false;
    }

    public void SettinsQuitButtonAction()
    {
        GameManager.instance.gameData.GameOver(); ;
        SceneManager.LoadScene(SceneName.Start.name());
    }

    public void ToggleMusicAction()
    {
        GameManager.instance.gameData.ToggleMusic();
    }

    public void ToggleSoundAction()
    {
        GameManager.instance.gameData.ToggleSound();
    }


}
