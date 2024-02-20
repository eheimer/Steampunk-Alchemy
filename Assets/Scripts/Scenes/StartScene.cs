using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : Scene
{
  public AudioClip startMusic;
  public TMP_Text experienceText;
  public AudioClip proceedSound;

  protected override void Start()
  {
    base.Start();
    experienceText.text = GameManager.instance.gameData.Experience.ToString();
  }

  public override bool HasMusic()
  {
    return true;
  }

  public override AudioClip GetMusic()
  {
    return startMusic;
  }
  public void PlayButtonAction()
  {
    GameManager.instance.PlaySoundEffect(proceedSound);
    SceneManager.LoadScene(SceneName.Game.name());
  }
}
