using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool gameIsPaused = false;

    [Header("UI Components")]
    [SerializeField] public GameObject pauseMenuUI;
    [SerializeField] public GameObject settingsMenuUI;
    [SerializeField] private GameObject victoryScreenUI;
    [SerializeField] private GameObject defeatScreenUI;

    [Header("Wwise")]
    [SerializeField] public AK.Wwise.State Paused;
    [SerializeField] public AK.Wwise.State Playing;
    [SerializeField] public AK.Wwise.State Victory;
    [SerializeField] public AK.Wwise.State Failed;
    [SerializeField] public AK.Wwise.Event Button_Click;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (gameIsPaused)
            {
                Resume();
            } else {
                Pause();
            }
        }  
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Playing.SetValue(); // WWise state change
        gameIsPaused = false;

    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Paused.SetValue(); // Wwise state change
        gameIsPaused = true;
    }

    public void Settings()
    {
       settingsMenuUI.SetActive(!settingsMenuUI.activeSelf);
       pauseMenuUI.SetActive(!pauseMenuUI.activeSelf);
    }
    public void VictoryScreen()
    {
        victoryScreenUI.SetActive(true);
        Victory.SetValue();
    }
    public void Defeat()
    {
        defeatScreenUI.SetActive(true);
        Failed.SetValue();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 == SceneManager.sceneCountInBuildSettings) { Application.Quit(); }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayButtonClickAudio()
    {
        Button_Click.Post(gameObject);
    }
    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
