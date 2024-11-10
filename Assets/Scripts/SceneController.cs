using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    //public GameOverScreen;

    public static SceneController instance;
    private PauseMenu MenuObj;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //public void GameOver()
    //{
    //    GameOverScreen.Setup(LevelManager.main.GetScore());
    //}
    public void NextLevel()
    {
        MenuObj.VictoryScreen();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}

