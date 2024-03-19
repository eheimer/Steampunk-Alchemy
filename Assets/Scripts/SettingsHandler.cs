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

    private GameScene gameScene;
    private void Start()
    {
        gameScene = FindObjectOfType<GameScene>();
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
    }

    public void ToggleSoundAction()
    {
        GameManager.instance.gameData.ToggleSound();
    }

    public void ToggleAmbientAction()
    {
        GameManager.instance.gameData.ToggleAmbient();
    }


}
