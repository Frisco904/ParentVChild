using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] public AK.Wwise.Switch Menu;
    [SerializeField] public AK.Wwise.Event LevelMusicStart;

    void Start() {
        // Wwise call to start music.
        Menu.SetValue(gameObject);
        LevelMusicStart.Post(gameObject);
    }
    void Update()
    {
        if (Input.anyKeyDown)  SceneManager.LoadScene(1);
    }
}
