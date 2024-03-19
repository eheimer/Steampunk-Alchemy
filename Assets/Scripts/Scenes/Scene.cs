using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Scene : MonoBehaviour
{
    public GameObject settingsPrefab;

    protected virtual void OnEnable()
    {
        // this check is here to ensure that the start scene is always loaded first to initialize the game manager
        if (GameManager.instance == null)
        {
            SceneManager.LoadScene(SceneName.Start.name());
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        GameManager.instance.currentScene = this;
        //find a canvas object named "UI" in the scene
        GameObject canvas = GameObject.Find("UI");

        Instantiate(settingsPrefab, canvas.transform);
        if (HasMusic())
        {
            GameManager.instance.PlayMusic(GetMusic());
        }
        if (HasAmbient())
        {
            GameManager.instance.PlayAmbient(GetAmbient());
        }
    }

    /// <summary>
    /// Subclasses must implement this method.  If the method returns true, you should also override GetMusic() to return the music clip to play.
    /// </summary>
    /// <returns></returns>
    public abstract bool HasMusic();

    public virtual AudioClip GetMusic()
    {
        return null;
    }

    public abstract bool HasAmbient();
    public virtual AudioClip GetAmbient()
    {
        return null;
    }
}
