using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static string scene;
    public static Action<string> sceneChanged;
    static GameManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
            instance = this;
            scene = SceneManager.GetActiveScene().name;
            SceneManager.activeSceneChanged += SceneChanged;
        }
    }

    private void SceneChanged(Scene current, Scene next)
    {// to be made into something useful
        //Debug.Log(current.name);
        //Debug.Log(next.name);
    }
}
