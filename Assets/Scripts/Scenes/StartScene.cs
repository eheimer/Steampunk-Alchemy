using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : Scene
{
  public AudioClip startMusic;
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
    GameManager.instance.gameData.Restart();
    SceneManager.LoadScene(SceneName.Game.name());
  }
}
