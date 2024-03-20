using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour
{
    public GameObject button;
    public GameObject panel;
    public Toggle musicToggle;
    public Toggle soundToggle;
    public Toggle ambientToggle;
    public Slider musicSlider;
    public Slider soundSlider;
    public Slider ambientSlider;
    public GameObject abortButton;
    public GameObject resetButton;

    private GameScene gameScene;
    private void Start()
    {
        gameScene = FindObjectOfType<GameScene>();
        panel.SetActive(false);
        button.SetActive(true);
    }

    public void SettingsButtonAction()
    {
        if (gameScene != null)
        {
            gameScene.StateMachine.ChangeState(GameState.Menu);
        }

        // set up the panel so that it shows the current settings
        musicToggle.SetIsOnWithoutNotify(GameManager.instance.gameData.Music);
        soundToggle.SetIsOnWithoutNotify(GameManager.instance.gameData.Sound);
        ambientToggle.SetIsOnWithoutNotify(GameManager.instance.gameData.Ambient);
        if (GameManager.instance.gameData.Music)
        {
            musicSlider.SetValueWithoutNotify(GameManager.instance.gameData.MusicVolume);
        }
        if (GameManager.instance.gameData.Sound)
        {
            soundSlider.SetValueWithoutNotify(GameManager.instance.gameData.SoundVolume);
        }
        if (GameManager.instance.gameData.Ambient)
        {
            ambientSlider.SetValueWithoutNotify(GameManager.instance.gameData.AmbientVolume);
        }
        if (GameManager.instance.currentScene.GetType() == typeof(GameScene))
        {
            abortButton.SetActive(true);
            resetButton.SetActive(false);
        }
        else
        {
            abortButton.SetActive(false);
            resetButton.SetActive(true);
        }
        button.SetActive(false);
        panel.SetActive(true);
    }

    public void SettingsCloseButtonAction()
    {
        button.SetActive(true);
        panel.SetActive(false);
        if (gameScene != null)
        {
            gameScene.StateMachine.ChangeState(GameState.Idle);
        }
    }

    public void SettinsQuitButtonAction()
    {
        SceneManager.LoadScene(SceneName.Start.name());
    }

    public void SettingsResetButtonAction()
    {
        GameManager.instance.gameData.ResetExperience();
        SceneManager.LoadScene(SceneName.Start.name());
    }

    public void ToggleMusicAction()
    {
        GameManager.instance.gameData.ToggleMusic();
        if (!GameManager.instance.gameData.Music)
        {
            musicSlider.SetValueWithoutNotify(0);
        }
        else
        {
            musicSlider.SetValueWithoutNotify(GameManager.instance.gameData.MusicVolume);
        }
    }

    public void ToggleSoundAction()
    {
        GameManager.instance.gameData.ToggleSound();
        if (!GameManager.instance.gameData.Sound)
        {
            soundSlider.SetValueWithoutNotify(0);
        }
        else
        {
            soundSlider.value = GameManager.instance.gameData.SoundVolume;
        }
    }

    public void ToggleAmbientAction()
    {
        GameManager.instance.gameData.ToggleAmbient();
        if (!GameManager.instance.gameData.Ambient)
        {
            ambientSlider.SetValueWithoutNotify(0);
        }
        else
        {
            ambientSlider.SetValueWithoutNotify(GameManager.instance.gameData.AmbientVolume);
        }
    }

    public void AmbientSliderAction(float value)
    {
        if (value > 0f && !GameManager.instance.gameData.Ambient)
        {
            ambientToggle.isOn = true;
        }
        if (value == 0f && GameManager.instance.gameData.Ambient)
        {
            ambientToggle.isOn = false;
        }
        GameManager.instance.gameData.AmbientVolume = value;
    }

    public void MusicSliderAction(float value)
    {
        if (value > 0f && !GameManager.instance.gameData.Music)
        {
            musicToggle.isOn = true;
        }
        if (value == 0f && GameManager.instance.gameData.Music)
        {
            musicToggle.isOn = false;
        }
        GameManager.instance.gameData.MusicVolume = value;
    }

    public void SoundSliderAction(float value)
    {
        if (value > 0f && !GameManager.instance.gameData.Sound)
        {
            soundToggle.isOn = true;
        }
        if (value == 0f && GameManager.instance.gameData.Sound)
        {
            soundToggle.isOn = false;
        }
        GameManager.instance.gameData.SoundVolume = value;
    }


}
